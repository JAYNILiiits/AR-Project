using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARStateManager : MonoBehaviour
{
    private static ARStateManager _instance;

    void Awake()
    {
        // If another instance already exists, destroy duplicate
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        // Don’t destroy this manager when loading new scenes
        DontDestroyOnLoad(gameObject);

        // Also persist the ARSession and ARSessionOrigin
        var session = FindObjectOfType<ARSession>();
        if (session != null)
            DontDestroyOnLoad(session.gameObject);

        var origin = FindObjectOfType<ARSessionOrigin>();
        if (origin != null)
            DontDestroyOnLoad(origin.gameObject);
    }
}
