using UnityEngine;
using TMPro; // Required for TextMeshPro elements
using UnityEngine.UI; // Required for Button, InputField, and Image components
using System.Collections.Generic; // Required for Lists
using System.Linq; // Required for sorting (OrderByDescending) and FindIndex, FirstOrDefault
using System.IO; // Required for reading/writing files (JSON)

// Data Structures (ScoreEntry, ScoreData) remain the same
[System.Serializable]
public class ScoreEntry
{
    public string playerName;
    public int score;
}

[System.Serializable]
public class ScoreData
{
    public List<ScoreEntry> scores = new List<ScoreEntry>();
}


public class ScoreboardManager : MonoBehaviour
{
    // --- UI References (Assign in Inspector) ---
    [Header("UI References")]
    public TMP_InputField nameInputField;
    public Button testButton;
    public GameObject tableHeader;
    public List<GameObject> topScoreRowsUI = new List<GameObject>(10);
    public GameObject temporaryScoreRowUI;

    // --- Score Data Settings ---
    [Header("Score Settings")]
    [SerializeField] private int maxScoreboardEntries = 100;
    private const int DisplayRowCount = 10;

    // --- Highlighting Settings (Assign in Inspector) ---
    [Header("Highlighting")]
    public Color defaultRowColor = Color.white;
    public Color highlightColor = Color.yellow;

    // --- Private Internal State ---
    // *** NOTE: Made scoreData public in the provided script, changed back to private for encapsulation ***
    // If it needs to be public for other reasons, be careful about external modifications.
    public ScoreData scoreData = new ScoreData();
    private string savePath;

    // State for tracking the last added/updated score for highlighting and temporary row display
    private ScoreEntry lastAddedOrUpdatedEntry = null; // Renamed for clarity
    private int lastAddedOrUpdatedRank = -1; // Renamed for clarity

    // State for managing the currently highlighted row's appearance
    private GameObject currentlyHighlightedRow = null;
    private Image highlightedImageComponent = null;

    // --- Initialization Methods --- (Awake, Start are mostly the same)
    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "scoreboardData.json");
        // Validations...
        if (topScoreRowsUI == null || topScoreRowsUI.Count != DisplayRowCount) Debug.LogError($"Assign {DisplayRowCount} rows to Top Score Rows UI.");
        topScoreRowsUI.RemoveAll(item => item == null);
        if (topScoreRowsUI.Count != DisplayRowCount) Debug.LogError($"One or more Top Score Rows UI elements unassigned.");
        if (temporaryScoreRowUI == null) Debug.LogWarning("Temporary Score Row UI not assigned.");
        // Image component checks...
        foreach (var row in topScoreRowsUI) { if (row != null && row.GetComponent<Image>() == null) Debug.LogWarning($"Row '{row.name}' missing Image."); }
        if (temporaryScoreRowUI != null && temporaryScoreRowUI.GetComponent<Image>() == null) Debug.LogWarning($"Temporary Row '{temporaryScoreRowUI.name}' missing Image.");
        // Initial activation state...
        foreach (var row in topScoreRowsUI) { if (row != null) row.SetActive(false); }
        if (temporaryScoreRowUI != null) temporaryScoreRowUI.SetActive(false);
        LoadScores();
    }

    void Start()
    {
        if (testButton != null) testButton.onClick.AddListener(OnTestButtonClicked);
        else Debug.LogError("TestButton not assigned!");
        lastAddedOrUpdatedRank = -1; // Use renamed variable
        UpdateScoreboardUI();
    }

    // --- Button Action Method --- (OnTestButtonClicked is the same)
    void OnTestButtonClicked()
    {
        string playerName = nameInputField.text;
        if (string.IsNullOrWhiteSpace(playerName)) playerName = "Player";
        int randomScore = Random.Range(10, 1001);
        AddScoreEntry(playerName, randomScore);
        nameInputField.text = "";
    }

    // --- Core Score Logic Method (MODIFIED to include high score check) ---

    public void AddScoreEntry(string name, int score)
    {
        bool scoreChanged = false; // Flag to track if data was modified
        ScoreEntry entryToProcess = null; // Will hold the entry that was added or updated

        // --- Check for Existing Player ---
        // Find the first entry matching the name (case-sensitive).
        // Use .Equals(name, System.StringComparison.OrdinalIgnoreCase) for case-insensitive.
        ScoreEntry existingEntry = scoreData.scores.FirstOrDefault(entry => entry.playerName == name);

        if (existingEntry != null)
        {
            // --- Player Found: Check Score ---
            if (score > existingEntry.score)
            {
                // New score is higher, update it
                Debug.Log($"Updating score for player '{name}' from {existingEntry.score} to {score}");
                existingEntry.score = score;
                entryToProcess = existingEntry; // This is the entry we care about now
                scoreChanged = true;
            }
            else
            {
                // New score is not higher, do nothing to the data
                Debug.Log($"New score {score} for player '{name}' is not higher than existing {existingEntry.score}. Scoreboard unchanged.");
                lastAddedOrUpdatedRank = -1; // Ensure no highlight from this attempt
                lastAddedOrUpdatedEntry = null;
                UpdateScoreboardUI(); // Update UI mainly to clear any previous highlight
                return; // Exit, no sorting/saving needed
            }
        }
        else
        {
            // --- Player Not Found: Add New Entry ---
            Debug.Log($"Adding new player '{name}' with score {score}");
            ScoreEntry newEntry = new ScoreEntry { playerName = name, score = score };
            scoreData.scores.Add(newEntry);
            entryToProcess = newEntry; // This new entry is the one we care about
            scoreChanged = true;
        }

        // --- Post-Processing (Only if score was added or updated) ---
        if (scoreChanged)
        {
            // 1. Re-sort the scores list
            scoreData.scores = scoreData.scores.OrderByDescending(e => e.score).ToList();

            // 2. Find the rank of the entry we processed (added or updated)
            //    Use ReferenceEquals to be sure we find the exact object instance
            lastAddedOrUpdatedRank = scoreData.scores.FindIndex(e => System.Object.ReferenceEquals(e, entryToProcess)) + 1;
            lastAddedOrUpdatedEntry = entryToProcess; // Store reference for temp row/highlight logic

            // 3. Handle temporary row logic (clear entry reference if in top 10)
            if (lastAddedOrUpdatedRank <= DisplayRowCount)
            {
                lastAddedOrUpdatedEntry = null; // Not needed for *temp row display* if in top 10
            }
            // else: Entry reference and rank remain set for the temporary row

            // 4. Limit total stored entries
            if (scoreData.scores.Count > maxScoreboardEntries)
            {
                scoreData.scores = scoreData.scores.GetRange(0, maxScoreboardEntries);
                // Consider re-checking rank here if maxScoreboardEntries is very small
            }

            // 5. Save changes
            SaveScores();

            // 6. Update UI display
            UpdateScoreboardUI();
        }
    }

    // --- UI Update Method --- (Uses renamed variables lastAddedOrUpdated...)

    void UpdateScoreboardUI()
    {
        ClearHighlight();
        if (tableHeader != null) tableHeader.SetActive(scoreData.scores.Count > 0);

        // Update Top 10
        for (int i = 0; i < DisplayRowCount; i++)
        {
            if (i >= topScoreRowsUI.Count || topScoreRowsUI[i] == null) continue;
            GameObject row = topScoreRowsUI[i];
            if (i < scoreData.scores.Count)
            {
                row.SetActive(true);
                ScoreEntry entry = scoreData.scores[i];
                UpdateRowText(row, i + 1, entry.playerName, entry.score);
                // Highlight if rank matches last added/updated
                if (lastAddedOrUpdatedRank == (i + 1)) // Check renamed variable
                {
                    HighlightRow(row);
                }
            }
            else
            {
                row.SetActive(false);
            }
        }

        // Update Temporary Row
        if (temporaryScoreRowUI != null)
        {
            // Check renamed variables for temp row logic
            if (lastAddedOrUpdatedEntry != null && lastAddedOrUpdatedRank > DisplayRowCount)
            {
                temporaryScoreRowUI.SetActive(true);
                UpdateRowText(temporaryScoreRowUI, lastAddedOrUpdatedRank, lastAddedOrUpdatedEntry.playerName, lastAddedOrUpdatedEntry.score);
                HighlightRow(temporaryScoreRowUI);
            }
            else
            {
                temporaryScoreRowUI.SetActive(false);
            }
        }
    }

    // --- Helper Method to Update Text --- (UpdateRowText is the same)
    void UpdateRowText(GameObject rowGO, int place, string name, int score)
    {
        if (rowGO == null) return;
        TextMeshProUGUI[] texts = rowGO.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length >= 3)
        {
            texts[0].text = place.ToString();
            texts[1].text = name;
            texts[2].text = score.ToString("D3");
        }
        else { Debug.LogWarning($"Row '{rowGO.name}' missing Text components ({texts.Length} found)."); }
    }


    // --- Highlighting Helpers --- (HighlightRow, ClearHighlight are the same)
    void HighlightRow(GameObject rowToHighlight)
    {
        if (rowToHighlight == null) return;
        Image img = rowToHighlight.GetComponent<Image>();
        if (img != null)
        {
            img.color = highlightColor;
            currentlyHighlightedRow = rowToHighlight;
            highlightedImageComponent = img;
        }
        else { Debug.LogWarning($"Cannot highlight '{rowToHighlight.name}', missing Image."); }
    }

    void ClearHighlight()
    {
        if (currentlyHighlightedRow != null && highlightedImageComponent != null)
        {
            highlightedImageComponent.color = defaultRowColor;
        }
        currentlyHighlightedRow = null;
        highlightedImageComponent = null;
    }


    // --- Saving and Loading --- (SaveScores, LoadScores are the same, use renamed variables)
    void SaveScores()
    {
        string json = JsonUtility.ToJson(scoreData, true);
        try { File.WriteAllText(savePath, json); }
        catch (System.Exception e) { Debug.LogError($"Failed to save scoreboard: {e.Message}"); }
    }

    void LoadScores()
    {
        // Use renamed variables
        lastAddedOrUpdatedEntry = null;
        lastAddedOrUpdatedRank = -1;
        ClearHighlight();

        if (File.Exists(savePath))
        {
            try
            { /* ... load logic ... */
                string json = File.ReadAllText(savePath);
                scoreData = JsonUtility.FromJson<ScoreData>(json);
                if (scoreData == null) scoreData = new ScoreData();
                if (scoreData.scores == null) scoreData.scores = new List<ScoreEntry>();
                scoreData.scores = scoreData.scores.OrderByDescending(entry => entry.score).ToList();
                Debug.Log("Scoreboard loaded from: " + savePath);
            }
            catch (System.Exception e)
            { /* ... error handling ... */
                Debug.LogError($"Failed to load scoreboard: {e.Message}. Starting fresh.");
                scoreData = new ScoreData();
            }
        }
        else
        { /* ... file not found ... */
            Debug.Log("No scoreboard save file found. Starting fresh.");
            scoreData = new ScoreData();
        }
    }

    // --- Optional Utility --- (ClearScoreboardData uses renamed variables)
    [ContextMenu("Clear All Scoreboard Data")]
    public void ClearScoreboardData()
    {
        Debug.LogWarning("Clearing all scoreboard data...");
        scoreData = new ScoreData();
        // Use renamed variables
        lastAddedOrUpdatedEntry = null;
        lastAddedOrUpdatedRank = -1;
        SaveScores();
        UpdateScoreboardUI();
        Debug.Log("Scoreboard data cleared.");
    }
}