using UnityEngine;

public class ChatBotButton : MonoBehaviour
{
    public GameObject chatCanvas;        // The chat panel to show
    public GameObject menuController;    // The menu controller to hide
    public GameObject arMenuCanvas;      // The AR menu canvas to hide

    public void ShowChat()
    {

        if (chatCanvas != null)
            chatCanvas.SetActive(true);

        if (menuController != null)
            menuController.SetActive(false);

        if (arMenuCanvas != null)
            arMenuCanvas.SetActive(false);
    }
}
