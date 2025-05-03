using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TTSManager : MonoBehaviour
{
    public IEnumerator Speak(string text)
    {
        const string url = "http://localhost:5005/tts";
        var payload = JsonUtility.ToJson(new TextPayload { text = text });
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(payload);

        var request = new UnityWebRequest(url, "POST");
        request.uploadHandler   = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("TTS Error: " + request.error);
        }
        else
        {
            var wavData = request.downloadHandler.data;
            var clip = WavUtility.ToAudioClip(wavData, 0, "tts");
            AudioSource.PlayClipAtPoint(clip, Vector3.zero);
        }

        // **Dispose** to free the native buffers
        request.uploadHandler.Dispose();
        request.downloadHandler.Dispose();
        request.Dispose();
    }

    [System.Serializable]
    class TextPayload { public string text; }
}
