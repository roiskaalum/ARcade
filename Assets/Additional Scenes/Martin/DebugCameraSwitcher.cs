using UnityEngine;

public class DebugCameraSwitcher : MonoBehaviour
{
    public Camera arCamera; // drag MainCamera here manually if needed

#if UNITY_EDITOR
    void Start()
    {
        if (arCamera == null)
        {
            arCamera = Camera.main;
        }

        if (arCamera != null)
        {
            arCamera.enabled = false;
            Debug.Log("AR Camera disabled for editor play mode.");
        }
        else
        {
            Debug.LogWarning("AR Camera not found.");
        }

        Camera debugCam = GetComponent<Camera>();
        if (debugCam != null)
        {
            debugCam.enabled = true;
            Debug.Log("Debug Camera enabled.");
        }
        else
        {
            Debug.LogError("No Camera component found on this GameObject!");
        }
    }
#endif
}
