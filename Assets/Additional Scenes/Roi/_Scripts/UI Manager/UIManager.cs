using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class UIManager : MonoBehaviour
{
    //[SerializeField] private GameObject panelsParent;
    private GameObject panelsParent;
    private GameObject[] panels;

    private NameAuthenticator nameAuthenticator;

    [SerializeField] private ScoreboardManager scoreboardManager;

    //public static UIManager Instance { get; private set; } // Singleton instance, but not necessary, but could be good for the SEP Project.
    // // An Additional thing we can make, is to have the Awake portion just inside the getter of the Instance property.
    // // This way, we can ensure that the initialization of the manager is only called when it's needed, but this might lead to loading issues initially.
    // // But since the Start Method still runs, maybe it's not that big of an issue.

    private void Awake()
    {
    //    if (Instance != null)
    //    {
    //        Destroy(gameObject);
    //    }
    //    Instance = this;

        nameAuthenticator = FindFirstObjectByType<NameAuthenticator>();
    }

    private void Start()
    {
        EventSystem eventSystem = FindAnyObjectByType<EventSystem>();
        if (eventSystem == null)
        {
            // Instantiate a new EventSystem if it doesn't exist
            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.transform.SetParent(null); // Ensure it is placed at the root level
            eventSystem = eventSystemObject.AddComponent<EventSystem>();
            // Add the Input System UI Input Module to handle user input
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
        }
        panelsParent = this.gameObject;
        panels = new GameObject[panelsParent.transform.childCount];
        PopulatePanelArray();
        DisablePanels();
        panels[0].SetActive(true);
        if(scoreboardManager == null)
        {
            scoreboardManager = FindFirstObjectByType<ScoreboardManager>();
            if(scoreboardManager == null)
            {
                Debug.LogError("ScoreboardManager not found in the scene. Critical error.");
            }
        }

        Debug.Log($"NameAuthenticator found: {nameAuthenticator != null}");
    }

    public void OnButtonPressed(int panelNumber)
    {
        DisablePanels();
        panels[panelNumber].SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
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
            panel.SetActive(false);
        }
    }

// Consider if the below methods should go into a separate Either manager, or just button specific scripts.
#region Game Methods
    public void BeginGame()
    {
        nameAuthenticator.ValidateName();
    }

    public void BeginGameAsGuest()
    {
        // Logic for guest start
        string guestName = "Guest";
        while(guestName == "Guest" && scoreboardManager.scoreData.scores.Exists(entry => entry.playerName == guestName))
        {
            // Generate a random guest name
            guestName = "Guest " + Random.Range(1000, 9999).ToString();
        }
        
        Debug.Log($"Starting game as guest with name: {guestName}");
        scoreboardManager.AddScoreEntry(guestName, -1);

        // Add logic to start the game here
        // SomeStartGameMethod(guestName);
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

    public void OnApplicationQuit()
    {
        // Application quit logic / Menu or Image Target Reset logic.
    }
#endregion Game Methods

}