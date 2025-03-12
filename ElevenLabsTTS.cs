
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using TMPro;

public class ElevenLabsTTS : MonoBehaviour
{
    public string apiKey = "sk_db0bbcb93a2e1e17843fa5f29120f430c0fa187dea2e5a7d"; // Replace this with your new API key
    private string voiceId = "1qEiC6qsybMkmnNdVMbK"; // Eleven Labs voice ID
    public TMP_InputField inputField; // Assign in Unity Inspector
    public AudioSource audioSource;

    private string apiUrl = "https://api.elevenlabs.io/v1/text-to-speech/";

    public void ConvertTextToSpeech()
    {
        string text = inputField.text;
        if (!string.IsNullOrEmpty(text))
        {
            StartCoroutine(SendTextToSpeech(text));
        }
    }

    IEnumerator SendTextToSpeech(string text)
    {
        string url = apiUrl + voiceId;
        string jsonData = "{\"text\":\"" + text + "\",\"model_id\":\"eleven_monolingual_v1\"}";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("xi-api-key", apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            byte[] audioData = request.downloadHandler.data;
            AudioClip clip = WavUtility.ToAudioClip(audioData);
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }
}
