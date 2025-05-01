using UnityEngine;
using UnityEngine.SceneManagement;

public class ChatBotController : MonoBehaviour
{
    const string chatSceneName = "ChatBotScene";

    public void OnExitChat()
    {
        // Start unloading; ARScene stays alive and running
        SceneManager.UnloadSceneAsync(chatSceneName)
            .completed += handle =>
            {
                // (Optional) make ARScene active again
                Scene ar = SceneManager.GetSceneByName("ARScene");
                if (ar.IsValid())
                    SceneManager.SetActiveScene(ar);
            };
    }
}
