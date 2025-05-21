using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class TouchClickEnforcer : MonoBehaviour
{
    private void Update()
    {
        if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Touchscreen.current.primaryTouch.position.ReadValue();

            // Raycast to find the button
            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (var result in results)
            {
                var button = result.gameObject.GetComponent<UnityEngine.UI.Button>();
                if (button != null && button.interactable)
                {
                    button.OnPointerClick(pointerData);
                    Debug.Log($"FORCE-CLICKED: {result.gameObject.name}");
                    break; // Stop after first button
                }
            }
        }
    }
}