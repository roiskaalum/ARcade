using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class UIManager : MonoBehaviour
{
    //[SerializeField] private GameObject panelsParent;
    private GameObject panelsParent;
    private GameObject[] panels;

    //public static UIManager Instance { get; private set; }

    //private void Awake()
    //{
    //    if (Instance != null)
    //    {
    //        Destroy(gameObject);
    //    }
    //    Instance = this;
    //}

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
        // Game start logic
    }

    public void BeginGameAsGuest()
    {
        // Guest start logic
    }

    public void OnApplicationQuit()
    {
        // Application quit logic / Menu or Image Target Reset logic.
    }
#endregion Game Methods

}