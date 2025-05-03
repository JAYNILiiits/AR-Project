using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class VoiceInput : MonoBehaviour
{
    private AudioClip clip;
    private string micDevice;

    public void StartRecording()
    {
        micDevice = Microphone.devices[0];
        clip = Microphone.Start(micDevice, false, 5, 44100);
    }

    public void StopAndRecognize(System.Action<string> callback)
    {
        Microphone.End(micDevice);
        StartCoroutine(SendToSTT(callback));
    }

    private IEnumerator SendToSTT(System.Action<string> callback)
    {
        byte[] wavData = WavUtility.FromAudioClip(clip);
        const string url = "http://localhost:5005/stt";

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.uploadHandler = new UploadHandlerRaw(wavData);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "audio/wav");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("STT Failed: " + www.error);
            }
            else
            {
                string json = www.downloadHandler.text;
                string text = JsonUtility.FromJson<ResponseText>(json).text;
                callback?.Invoke(text);
            }
        }
    }

    public void StopListening()
{
    if (Microphone.IsRecording(micDevice))
    {
        Microphone.End(micDevice);
        Debug.Log("Microphone recording stopped.");
    }
    else
    {
        Debug.Log("Microphone was not recording.");
    }
}


    [System.Serializable]
    public class ResponseText
    {
        public string text;
    }
}
