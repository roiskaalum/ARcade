using UnityEngine;
using UnityEngine.EventSystems;

public class ClickDetector : MonoBehaviour
{
    [Header("Settings")]
    public bool debugRaycast = true; // Draw debug ray in Scene view
    public LayerMask raycastMask = ~0; // Default: Include all layers

    private void Update()
    {
        // Detect left mouse button click (or touch on mobile)
        if (Input.GetMouseButtonDown(0))
        {
            bool isOverUI = EventSystem.current.IsPointerOverGameObject();

            // Check if the click/touch is blocked by UI
            if (isOverUI)
            {
                Debug.Log("Input blocked by UI element!");
            }
            else
            {
                // Raycast to check if the ball was clicked
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastMask))
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        Debug.Log("Ball was clicked directly!");
                    }
                    else
                    {
                        Debug.Log($"Clicked object: {hit.collider.name} (Not the ball)");
                    }
                }
                else
                {
                    Debug.Log("No collider hit.");
                }

                // Optional: Visualize the ray in Scene view
                if (debugRaycast)
                {
                    Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 1f);
                }
            }
        }
    }

    // Alternative: Built-in method (works if no UI is blocking)
    private void OnMouseDown()
    {
        // Debug.Log("OnMouseDown: Ball clicked (UI not blocking)");
    }
}