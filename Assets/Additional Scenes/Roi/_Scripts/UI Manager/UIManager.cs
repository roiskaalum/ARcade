using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject panelsParent;
    private GameObject[] panels;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }

    private void Start()
    {
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