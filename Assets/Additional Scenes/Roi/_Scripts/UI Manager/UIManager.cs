using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Transform panelsParent;
    private GameObject[] panels;

    [SerializeField] private NameAuthenticator nameAuthenticator;

    [SerializeField] private ScoreboardManager scoreboardManager;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (nameAuthenticator == null)
        {
            nameAuthenticator = FindFirstObjectByType<NameAuthenticator>();
        }



        EventSystem eventSystem = FindAnyObjectByType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.transform.SetParent(null);
            eventSystem = eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
        }

        StartCoroutine(DelayedUIReady());
    }

    private IEnumerator DelayedUIReady()
    {
        yield return new WaitUntil(() => GameManager.Instance != null);
        GameManager.Instance.OnUIReady();
    }

    private void Start()
    {
        if (panelsParent == null)
        {
            Debug.LogError("panelsParent er ikke sat!");
            return;
        }

        panels = new GameObject[panelsParent.childCount];
        PopulatePanelArray();
        DisablePanels();

        if (panels.Length > 0)
        {
            panels[0].SetActive(true);
            Debug.Log($"Aktiverer panel: {panels[0].name}");
        }
        else
        {
            Debug.LogError("Ingen paneler fundet i panelsParent");
        }

        if (scoreboardManager == null)
        {
            scoreboardManager = FindFirstObjectByType<ScoreboardManager>();
            if (scoreboardManager == null)
            {
                Debug.LogError("ScoreboardManager not found in the scene. Critical error.");
            }
        }

        Debug.Log($"NameAuthenticator found: {nameAuthenticator != null}");

        if (EventSystem.current == null)
        {
            Debug.LogWarning("EventSystem mangler stadig i scenen efter Start() � dette kan p�virke UI-navigation.");
        }
    }

    private void PopulatePanelArray()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i] = panelsParent.transform.GetChild(i).gameObject;
        }
    }

    private void DisablePanels()
    {
        foreach (var panel in panels)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Et panel var NULL i DisablePanels � muligvis slettet i Hierarchy?");
            }
        }
    }

    public void OnButtonPressed(int panelIndex)
    {
        DisablePanels();

        if (panelIndex >= 0 && panelIndex < panels.Length)
        {
            panels[panelIndex].SetActive(true);
        }

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
        else
        {
            Debug.LogWarning("EventSystem.current er null � kunne ikke nulstille selected GameObject.");
        }
    }


    public void HandleMainMenu() => OnButtonPressed(0);
    public void HandleEnterName() => OnButtonPressed(1);
    public void HandleOptions() => OnButtonPressed(2);
    public void HandleScoreboard() => OnButtonPressed(3);
    public void HandleGameOver() => OnButtonPressed(4);
    public void HandlePause() => OnButtonPressed(5);

    #region Game Methods

    public void BeginGame(bool isGuest)
    {
        //det her skal tilf�jes i NameAuthenticator(if (existingEntry.score != null)): I begge if statements
        //UIManager.Instance.InitializeGame(enteredName);

        if (isGuest)
        {
            BeginGameAsGuest();
            return;
        }
            
        var nameAuthenticationResult = nameAuthenticator.ValidateName();
        Debug.Log(nameAuthenticationResult);

        if (nameAuthenticationResult.Item2)
            GameManager.Instance.StartGameplay(nameAuthenticationResult.Item1);
    }

    public void BeginGameAsGuest()
    {
        string guestName = "Guest " + Random.Range(1000, 9999).ToString();
        while (scoreboardManager.scoreData.scores.Find(x => x.playerName == guestName) != null)
        {
            guestName = "Guest " + Random.Range(1000, 9999).ToString();
        }
        Debug.Log($"Starter spil som gæst: {guestName}");


        GameManager.Instance.StartGameplay(guestName);
    }

    public void InitializeGame(string name)
    {
        GameManager.Instance.StartGameplay(name);
    }

    public void SelectScoreOption(bool continuePrevious)
    {
        GameManager.Instance.OnScoreOptionSelected(continuePrevious);
    }

    public void PauseGame()
    {
        // Pause game logic
        Time.timeScale = 0f; // Pause the game
                             // Show pause menu UI
        OnButtonPressed(5); // Assuming index 1 is the pause menu
    }

    public void ResumeGame()
    {
        // Resume game logic
        Time.timeScale = 1f; // Resume the game
        // Hide pause menu UI
        DisablePanels(); // Assuming index 0 is the main game UI
    }

    public void HandleChooseScoreOption()
    {
        Debug.LogWarning("HandleChooseScoreOption kaldt � men intet panel er implementreret endnu.");
    }

    public void QuitGame()
    {
        GameManager.Instance.ExitGame();
    }

    #endregion

}
