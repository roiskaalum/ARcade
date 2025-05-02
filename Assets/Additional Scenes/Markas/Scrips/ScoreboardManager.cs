using UnityEngine;
using TMPro; // Required for TextMeshPro elements
using UnityEngine.UI; // Required for Button, InputField, and Image components
using System.Collections.Generic; // Required for Lists
using System.Linq; // Required for sorting (OrderByDescending) and FindIndex
using System.IO; // Required for reading/writing files (JSON)

//-----------------------------------------------------------------------------
// Data Structures - Define these OUTSIDE the ScoreboardManager class
//-----------------------------------------------------------------------------

// Holds data for a single score entry.
// [System.Serializable] allows Unity's JsonUtility to save/load this.
[System.Serializable]
public class ScoreEntry
{
    public string playerName;
    public int score;
}

// Wrapper class to hold the list of scores.
// JsonUtility works better with wrapping lists in an object.
[System.Serializable]
public class ScoreData
{
    // Initialize the list so it's never null
    public List<ScoreEntry> scores = new List<ScoreEntry>();
}

//-----------------------------------------------------------------------------
// Main Scoreboard Manager Class
//-----------------------------------------------------------------------------
public class ScoreboardManager : MonoBehaviour
{
    // --- UI References (Assign in Inspector) ---
    [Header("UI References")]
    [Tooltip("The input field where the player name is entered.")]
    public TMP_InputField nameInputField;
    [Tooltip("The button that triggers adding a test score.")]
    public Button testButton;
    [Tooltip("The GameObject containing the 'Place', 'Name', 'Score' header texts.")]
    public GameObject tableHeader;
    [Tooltip("Assign the 10 GameObjects representing the Top 10 rows. Ensure each has an Image component.")]
    public List<GameObject> topScoreRowsUI = new List<GameObject>(10); // Initialize list size for Inspector clarity
    [Tooltip("Assign the GameObject for the temporary row below the Top 10. Ensure it has an Image component.")]
    public GameObject temporaryScoreRowUI;

    // --- Score Data Settings ---
    [Header("Score Settings")]
    [Tooltip("The maximum number of scores to STORE internally. Can be more than displayed.")]
    [SerializeField] private int maxScoreboardEntries = 100;
    private const int DisplayRowCount = 10; // Fixed number of rows to display in the main list

    // --- Highlighting Settings (Assign in Inspector) ---
    [Header("Highlighting")]
    [Tooltip("The default background color for score rows.")]
    public Color defaultRowColor = Color.white;
    [Tooltip("The background color used to highlight the most recently added score.")]
    public Color highlightColor = Color.yellow;

    // --- Private Internal State ---
    public ScoreData scoreData = new ScoreData(); // Holds all loaded/saved score entries
    private string savePath; // Path to the JSON save file

    // State for tracking the last added score for highlighting and temporary row display
    private ScoreEntry lastAddedEntry = null; // Data of the last added entry (used for temp row)
    private int lastAddedRank = -1; // Rank of the last added entry (1-based, -1 = none/invalid)

    // State for managing the currently highlighted row's appearance
    private GameObject currentlyHighlightedRow = null; // Reference to the row GO being highlighted
    private Image highlightedImageComponent = null; // Cached Image component of the highlighted row

    // --- Initialization Methods ---

    void Awake()
    {
        // Determine the path for saving/loading score data
        savePath = Path.Combine(Application.persistentDataPath, "scoreboardData.json");

        // --- Validate Inspector Assignments (Basic Checks) ---
        if (topScoreRowsUI == null || topScoreRowsUI.Count != DisplayRowCount)
        {
            Debug.LogError($"ScoreboardManager: Please assign exactly {DisplayRowCount} GameObjects to the 'Top Score Rows UI' list in the Inspector.");
        }
        // Ensure list doesn't contain null entries accidentally
        topScoreRowsUI.RemoveAll(item => item == null);
        if (topScoreRowsUI.Count != DisplayRowCount)
        {
             Debug.LogError($"ScoreboardManager: One or more elements in 'Top Score Rows UI' are unassigned.");
        }

        if (temporaryScoreRowUI == null)
        {
            Debug.LogWarning("ScoreboardManager: Temporary Score Row UI is not assigned. Feature disabled.");
        }

        // Optional: Check for required Image components on rows
        // (Add similar checks if TextMeshPro components are consistently missing)
        foreach(var row in topScoreRowsUI) { if(row != null && row.GetComponent<Image>() == null) Debug.LogWarning($"Row '{row.name}' is missing an Image component required for highlighting."); }
        if(temporaryScoreRowUI != null && temporaryScoreRowUI.GetComponent<Image>() == null) Debug.LogWarning($"Temporary Row '{temporaryScoreRowUI.name}' is missing an Image component required for highlighting.");


        // Ensure all score rows start inactive (can also be set in the Editor)
        foreach (var row in topScoreRowsUI)
        {
            if (row != null) row.SetActive(false);
        }
        if (temporaryScoreRowUI != null) temporaryScoreRowUI.SetActive(false);

        // Load existing scores from file
        LoadScores();
    }

    void Start()
    {
        // Add listener to the test button's click event
        if (testButton != null)
        {
            // Use a lambda expression or create a separate method
            testButton.onClick.AddListener(OnTestButtonClicked);
        }
        else
        {
            Debug.LogError("ScoreboardManager: TestButton is not assigned in the Inspector!");
        }

        // Perform initial UI update after loading (no highlight on start)
        lastAddedRank = -1; // Ensure no highlight from previous session or testing
        UpdateScoreboardUI();
    }

    // --- Button Action Method ---

    void OnTestButtonClicked()
    {
        // Get player name from input field, use default if empty
        string playerName = nameInputField.text;
        if (string.IsNullOrWhiteSpace(playerName))
        {
            playerName = "Player"; // Default name
        }

        // Generate a random score for testing
        int randomScore = Random.Range(10, 1001); // Example range

        // Add the new entry (this will handle sorting, saving, and updating UI)
        AddScoreEntry(playerName, randomScore);

        // Clear the input field for the next entry
        nameInputField.text = "";
    }

    // --- Core Score Logic Method ---

    public void AddScoreEntry(string name, int score)
    {
        // 1. Create the new score entry object
        ScoreEntry newEntry = new ScoreEntry { playerName = name, score = score };

        // 2. Add it to the internal list
        scoreData.scores.Add(newEntry);

        // 3. Sort the entire list by score in descending order (highest first)
        scoreData.scores = scoreData.scores.OrderByDescending(entry => entry.score).ToList();

        // 4. Find the rank (1-based index) of the entry *we just added*
        //    We use ReferenceEquals to find the exact object instance we added.
        lastAddedRank = scoreData.scores.FindIndex(entry => System.Object.ReferenceEquals(entry, newEntry)) + 1;
        lastAddedEntry = newEntry; // Store the data reference as well

        // 5. Determine if the temporary row display is needed
        //    If the rank is within the top 10, we clear `lastAddedEntry` because
        //    the temporary row logic specifically checks for it being non-null.
        //    We *keep* `lastAddedRank` as it's needed for highlighting the top 10 row.
        if (lastAddedRank <= DisplayRowCount)
        {
           lastAddedEntry = null; // Not needed for *temporary row display*
        }
        // else: lastAddedEntry and lastAddedRank (> 10) remain set for the temporary row

        // 6. Limit the total number of *stored* scores (optional, but good practice)
        if (scoreData.scores.Count > maxScoreboardEntries)
        {
            scoreData.scores = scoreData.scores.GetRange(0, maxScoreboardEntries);
            // Note: If trimming could potentially remove the entry we just added
            // (i.e., if maxScoreboardEntries is very small), we might need to
            // re-calculate lastAddedRank here. Assumed unlikely for typical use.
        }

        // 7. Save the updated scores list to the JSON file
        SaveScores();

        // 8. Update the visual display (which will use lastAddedRank for highlighting)
        UpdateScoreboardUI();
    }

    // --- UI Update Method ---

    void UpdateScoreboardUI()
    {
        // 1. Clear any existing highlight from the previous update
        ClearHighlight();

        // 2. Show/hide the table header based on whether there are any scores
        if (tableHeader != null) tableHeader.SetActive(scoreData.scores.Count > 0);

        // 3. Update the Top 10 rows
        for (int i = 0; i < DisplayRowCount; i++)
        {
            // Ensure the row exists in our list (Inspector check)
            if (i >= topScoreRowsUI.Count || topScoreRowsUI[i] == null) continue;

            GameObject row = topScoreRowsUI[i];

            // Check if there's score data for this rank (index i)
            if (i < scoreData.scores.Count)
            {
                // Data exists: Activate row, get data, update text
                row.SetActive(true);
                ScoreEntry entry = scoreData.scores[i];
                UpdateRowText(row, i + 1, entry.playerName, entry.score);

                // Highlight this row if its rank matches the last added rank
                if (lastAddedRank == (i + 1))
                {
                    HighlightRow(row);
                }
            }
            else
            {
                // No data for this rank (e.g., only 5 scores exist), hide the row
                row.SetActive(false);
            }
        }

        // 4. Update the temporary score row (below the top 10)
        if (temporaryScoreRowUI != null)
        {
            // Show temporary row ONLY if last added entry exists AND its rank was > 10
            if (lastAddedEntry != null && lastAddedRank > DisplayRowCount)
            {
                temporaryScoreRowUI.SetActive(true);
                UpdateRowText(temporaryScoreRowUI, lastAddedRank, lastAddedEntry.playerName, lastAddedEntry.score);

                // If the temporary row is active, it means it's the one to highlight
                HighlightRow(temporaryScoreRowUI);
            }
            else
            {
                // Hide the temporary row if the last score was in top 10 or no score added yet
                temporaryScoreRowUI.SetActive(false);
            }
        }
    }

    // --- Helper Method to Update Text Elements of a Row ---

    void UpdateRowText(GameObject rowGO, int place, string name, int score)
    {
        if (rowGO == null) return;

        // Find TextMeshPro components within the row GameObject's children
        // Assumes order: Place, Name, Score. Adjust indices if structure differs.
        TextMeshProUGUI[] texts = rowGO.GetComponentsInChildren<TextMeshProUGUI>();

        if (texts.Length >= 3)
        {
            texts[0].text = place.ToString();             // Rank/Place
            texts[1].text = name;                         // Player Name
            texts[2].text = score.ToString("D3");         // Score (formatted to 3 digits, e.g., 042)
        }
        else
        {
            Debug.LogWarning($"Row '{rowGO.name}' does not have enough TextMeshProUGUI children (expected 3). Found {texts.Length}.");
        }
    }

    // --- Highlighting Helper Methods ---

    void HighlightRow(GameObject rowToHighlight)
    {
        if (rowToHighlight == null) return;

        // Attempt to get the Image component on the row GameObject itself
        Image img = rowToHighlight.GetComponent<Image>();
        if (img != null)
        {
            img.color = highlightColor; // Apply highlight color
            currentlyHighlightedRow = rowToHighlight; // Store reference to the highlighted row
            highlightedImageComponent = img;         // Store reference to its Image component
        }
        else
        {
            // Log a warning if highlighting is attempted on a row without an Image
            Debug.LogWarning($"Cannot highlight '{rowToHighlight.name}' - missing Image component.");
        }
    }

    void ClearHighlight()
    {
        // If we have a cached reference to a previously highlighted row's Image...
        if (currentlyHighlightedRow != null && highlightedImageComponent != null)
        {
            // ...reset its color back to the default.
            highlightedImageComponent.color = defaultRowColor;
        }

        // Clear the tracking variables regardless, ensuring no stale references
        currentlyHighlightedRow = null;
        highlightedImageComponent = null;
    }


    // --- Saving and Loading Methods ---

    void SaveScores()
    {
        // Serialize the entire ScoreData object (which contains the list) to JSON
        string json = JsonUtility.ToJson(scoreData, true); // 'true' for pretty print (readable JSON)

        try
        {
            // Write the JSON string to the specified file path
            File.WriteAllText(savePath, json);
            // Debug.Log("Scoreboard saved to: " + savePath); // Optional: Uncomment for save confirmation
        }
        catch (System.Exception e)
        {
            // Log error if saving fails (e.g., permissions issue)
            Debug.LogError($"Failed to save scoreboard to {savePath}: {e.Message}");
        }
    }

    void LoadScores()
    {
        // Reset temporary display/highlight state before loading
        lastAddedEntry = null;
        lastAddedRank = -1;
        ClearHighlight(); // Ensure no visual highlight remains from editor state

        // Check if the save file actually exists
        if (File.Exists(savePath))
        {
            try
            {
                // Read the entire JSON file content
                string json = File.ReadAllText(savePath);
                // Deserialize the JSON back into our ScoreData object
                scoreData = JsonUtility.FromJson<ScoreData>(json);

                // Basic validation: Ensure list isn't null after loading
                 if (scoreData == null) scoreData = new ScoreData(); // Create fresh if file was invalid
                 if (scoreData.scores == null) scoreData.scores = new List<ScoreEntry>(); // Ensure list exists

                 // Optional: Re-sort on load, in case data wasn't saved sorted or needs validation
                 scoreData.scores = scoreData.scores.OrderByDescending(entry => entry.score).ToList();

                Debug.Log("Scoreboard loaded successfully from: " + savePath);
            }
            catch (System.Exception e)
            {
                // Log error if loading/parsing fails, and start with fresh data
                Debug.LogError($"Failed to load scoreboard from {savePath}: {e.Message}. Starting fresh.");
                scoreData = new ScoreData(); // Initialize empty score data
            }
        }
        else
        {
            // If no save file found, start with fresh data
            Debug.Log("No scoreboard save file found at: " + savePath + ". Starting fresh.");
            scoreData = new ScoreData(); // Initialize empty score data
        }
    }

    // --- Optional Utility Method ---

    // Adds a command to the GameObject's context menu in the Inspector
    [ContextMenu("Clear All Scoreboard Data")]
    public void ClearScoreboardData()
    {
        Debug.LogWarning("Clearing all scoreboard data...");
        scoreData = new ScoreData(); // Create a new empty data object
        lastAddedEntry = null;       // Reset temporary state
        lastAddedRank = -1;
        SaveScores();                // Save the empty data to overwrite the file
        UpdateScoreboardUI();        // Update the display (will clear rows and highlight)
        Debug.Log("Scoreboard data cleared and file overwritten.");
    }
}