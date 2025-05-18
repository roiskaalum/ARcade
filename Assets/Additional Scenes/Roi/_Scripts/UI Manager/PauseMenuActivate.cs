using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuActivate : MonoBehaviour
{
    private UIManager uiManager;
    public TextMeshProUGUI warningText;

    private Canvas rootCanvas;
    private void Start()
    {
        uiManager = UIManager.Instance;
        if (uiManager == null)
        {
            warningText.gameObject.SetActive(true);
            Debug.LogError("UIManager not found in the scene.");
            return;
        }
        warningText.gameObject.SetActive(false);

        rootCanvas = gameObject.GetComponentInParent<Canvas>();
        if (rootCanvas == null)
        {
            Debug.LogError("No parent canvas found for PauseMenuActivate.");
            return;
        }
    }

    public void ActivatePauseMenu()
    {
        // uiManager.Register(rootCanvas);
        warningText.gameObject.SetActive(true);
        warningText.text = "Game Paused";
        uiManager.PauseGame();
        gameObject.SetActive(false);
    }
}
