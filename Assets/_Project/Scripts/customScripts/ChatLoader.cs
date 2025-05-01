using UnityEngine;
using UnityEngine.SceneManagement;

public class ChatLoader : MonoBehaviour
{
    // Name must match exactly what’s in Build Settings
    const string chatSceneName = "Chat";

    public void OnChatButtonPressed()
    {
        // Asynchronously load ChatBotScene on top of ARScene
        SceneManager.LoadSceneAsync(chatSceneName, LoadSceneMode.Additive)
            .completed += handle =>
            {
                // (Optional) make ChatBotScene the active scene so its objects
                // will receive Start/Awake calls correctly
                Scene loaded = SceneManager.GetSceneByName(chatSceneName);
                SceneManager.SetActiveScene(loaded);
            };
    }
}
