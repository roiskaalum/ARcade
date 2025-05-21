using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [SerializeField] private GameObject fullPauseUIRoot;
    [SerializeField] private GameObject pauseButtonCanvas;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        pauseButtonCanvas.SetActive(false);
    }

    public void ShowPauseButton()
    {
        pauseButtonCanvas.SetActive(true);
    }

    public void HidePauseButton()
    {
        pauseButtonCanvas.SetActive(false);
    }

    //husk er Time.timeScale ikke påvirker AR tracking 
    //ved ikke om vi har noget af relevans?= 


    public void PauseGame()
    {
        Time.timeScale = 0f;
        fullPauseUIRoot.SetActive(true);
        HidePauseButton();

        GameManager.Instance.SetState(GameManager.GameState.Pause);
        UIManager.Instance.HandlePause();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        fullPauseUIRoot.SetActive(false);
        ShowPauseButton();

        GameManager.Instance.SetState(GameManager.GameState.WaitingForInput);
    }
}
