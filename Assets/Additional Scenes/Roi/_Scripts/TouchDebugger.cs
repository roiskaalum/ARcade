using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TouchDebugger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI debugText; // Assign in Inspector
    [SerializeField] private InputActionReference uiClickAction; // Assign UI/Click action

    private bool _isClickColorToggled = false;
    private Color _originalColor = Color.white;

    private void Start()
    {
        if (debugText != null)
        {
            _originalColor = debugText.color;
        }
    }

    private void OnEnable()
    {
        if (uiClickAction != null)
        {
            uiClickAction.action.Enable();
            uiClickAction.action.performed += OnUIClicked;
        }
    }

    private void OnDisable()
    {
        if (uiClickAction != null)
        {
            uiClickAction.action.performed -= OnUIClicked;
        }
    }

    private void OnUIClicked(InputAction.CallbackContext ctx)
    {
        if (debugText != null)
        {
            debugText.color = _isClickColorToggled ? Color.blue : Color.green;
            _isClickColorToggled = !_isClickColorToggled;
        }
    }

    private void Update()
    {
        if (Touchscreen.current == null) return;

        var primaryTouch = Touchscreen.current.primaryTouch;
        string debugMessage = $"Touch: {primaryTouch.position.ReadValue()} | ";

        if (primaryTouch.press.isPressed)
        {
            debugMessage += "State=PRESSED | ";
        }
        else
        {
            debugMessage += "State=RELEASED | ";
        }

        debugMessage += $"UI Click Detected: {(uiClickAction?.action.triggered ?? false ? "YES" : "NO")}";

        if (debugText != null)
        {
            debugText.text = debugMessage;
        }
    }
}