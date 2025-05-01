using UnityEngine;

public class CloseButton : MonoBehaviour
{
    public GameObject chatCanvas;       // Chat UI panel
    public GameObject menuController;   // Main menu controller
    public GameObject arMenuCanvas;     // AR menu canvas

    public void ShowChat()
    {
        if (chatCanvas != null)
            chatCanvas.SetActive(true);

        if (menuController != null)
            menuController.SetActive(false);

        if (arMenuCanvas != null)
            arMenuCanvas.SetActive(false);
    }

    public void HideChat()
    {
        if (chatCanvas != null)
            chatCanvas.SetActive(false);

        if (menuController != null)
            menuController.SetActive(true);

        if (arMenuCanvas != null)
            arMenuCanvas.SetActive(true);
    }
}
