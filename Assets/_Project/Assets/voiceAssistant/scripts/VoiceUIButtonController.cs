using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;

public class VoiceUIButtonController : MonoBehaviour
{
    [Header("Voice Components")]
    public VoiceInput voiceInput;
    public TTSManager ttsManager;

    [Header("UI Components")]
    public GameObject messagePrefab;     // Drag your MessageTMP prefab here
    public Transform contentParent;      // Drag ChatScrollView→Viewport→Content here
    public ScrollRect scrollRect;        // Drag your ChatScrollView here (for auto‑scroll)

    bool isRecording = false;

    /// <summary>
    /// Hook this to MicButton.OnClick()
    /// </summary>
    public void OnMicButtonPressed()
    {
        if (!isRecording)
        {
            voiceInput.StartRecording();
            isRecording = true;
            Debug.Log("🔴 Recording...");
        }
        else
        {
            voiceInput.StopAndRecognize(OnTranscriptionResult);
            isRecording = false;
            Debug.Log("⏹️ Stopped recording");
        }
    }

    /// <summary>
    /// Called when STT returns the recognized text.
    /// </summary>
    private void OnTranscriptionResult(string text)
    {
        Debug.Log("🗣️ Recognized: " + text);

        // 1) Spawn user message
        SpawnMessage("You: " + text, Color.cyan);

        // 2) Send to chatbot API
        StartCoroutine(SendToChatbot(text));
    }

    /// <summary>
    /// Sends user input to FastAPI chatbot and gets response
    /// </summary>
    private IEnumerator SendToChatbot(string userText)
    {
        string url = "http://localhost:8000/chat"; // Change if hosted elsewhere
        ChatRequest payload = new ChatRequest { question = userText };
        string json = JsonUtility.ToJson(payload);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Chatbot request failed: " + request.error);
            SpawnMessage("Bot: [Error contacting bot]", Color.red);
        }
        else
        {
            string responseJson = request.downloadHandler.text;
            string reply = ExtractReply(responseJson);

            SpawnMessage("Bot: " + reply, Color.white);
            StartCoroutine(ttsManager.Speak(reply));
        }
    }

    /// <summary>
    /// Adds new message to chat UI
    /// </summary>
    private void SpawnMessage(string message, Color color)
    {
        var go = Instantiate(messagePrefab, contentParent);
        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = message;
        tmp.color = color;

        Canvas.ForceUpdateCanvases();

        if (scrollRect != null)
            scrollRect.verticalNormalizedPosition = 0f;
    }

    /// <summary>
    /// Parses the chatbot's response from JSON
    /// </summary>
    private string ExtractReply(string json)
    {
        return JsonUtility.FromJson<BotResponse>(json).response;
    }

    // Helper classes for JSON conversion
    [System.Serializable]
    public class ChatRequest
    {
        public string question;
    }

    [System.Serializable]
    public class BotResponse
    {
        public string response;
    }
}
