using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class ChatBotManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public Transform messageContainer;
    public GameObject userMessagePrefab;
    public GameObject botMessagePrefab;

    public string apiUrl = "http://<YOUR_LOCAL_IP>:8000/chat"; // Replace with actual IP if testing on device

    public void OnSendMessage()
    {
        string userText = inputField.text;
        if (string.IsNullOrWhiteSpace(userText)) return;

        AddMessageToChat(userText, true);
        StartCoroutine(SendToBot(userText));
        inputField.text = "";
    }

    void AddMessageToChat(string message, bool isUser)
    {
        GameObject prefab = isUser ? userMessagePrefab : botMessagePrefab;
        GameObject bubble = Instantiate(prefab, messageContainer);
        bubble.GetComponentInChildren<TMP_Text>().text = message;
    }

    IEnumerator SendToBot(string userInput)
    {
        string jsonBody = JsonUtility.ToJson(new ChatPayload(userInput));

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                BotResponse response = JsonUtility.FromJson<BotResponse>(request.downloadHandler.text);
                AddMessageToChat(response.response, false);
            }
            else
            {
                Debug.LogError("API error: " + request.error);
            }

            // ✅ Dispose handlers manually
            request.uploadHandler.Dispose();
            request.downloadHandler.Dispose();
        }
    }

}

[System.Serializable]
public class ChatPayload
{
    public string question;
    public ChatPayload(string q) { question = q; }
}

[System.Serializable]
public class BotResponse
{
    public string response;
}
