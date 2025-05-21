using UnityEngine;
using UnityEngine.InputSystem;

public class TouchUIEnabler : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;

    private void OnEnable()
    {
        var uiMap = inputActions.FindActionMap("UI", true);
        uiMap.Enable();
    }
}
