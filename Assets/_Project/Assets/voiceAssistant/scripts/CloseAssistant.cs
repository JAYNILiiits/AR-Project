using UnityEngine;

public class CloseAssistant : MonoBehaviour
{
    public GameObject voiceAssistantManager;  // Drag the VoiceAssistantManager object here
    public GameObject menuController;    // The menu controller to hide
    public GameObject arMenuCanvas;      // The AR menu canvas to hide

    public GameObject voiceAssistantCanvas;  // Drag the VoiceAssistantManager object here


    public void OnCloseButtonClicked()
    {

        if(voiceAssistantCanvas != null)
            voiceAssistantCanvas.SetActive(false);{
        }

        if (menuController != null)
            menuController.SetActive(true);

        if (arMenuCanvas != null)
            arMenuCanvas.SetActive(true);

        if (voiceAssistantManager != null)
        {
            // Stop voice input
            var voiceInput = voiceAssistantManager.GetComponent<VoiceInput>();
            if (voiceInput != null)
            {
                voiceInput.StopListening();  // You must implement this method in VoiceInput.cs
            }

            // Optionally hide or deactivate the whole UI
            voiceAssistantManager.SetActive(false);
        }
    }
}
