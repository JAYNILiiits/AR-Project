using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceActivation : MonoBehaviour
{
    public GameObject chatCanvas;        // The chat panel to show
    public GameObject menuController;    // The menu controller to hide
    public GameObject arMenuCanvas;      // The AR menu canvas to hide

    public GameObject voiceAssistantCanvas;  // Drag the VoiceAssistantManager object here

    public void showAssistant()
    {

        if(voiceAssistantCanvas != null)
            voiceAssistantCanvas.SetActive(true);{
        }

        if (chatCanvas != null)
            chatCanvas.SetActive(false);

        if (menuController != null)
            menuController.SetActive(false);

        if (arMenuCanvas != null)
            arMenuCanvas.SetActive(false);
    }
}
