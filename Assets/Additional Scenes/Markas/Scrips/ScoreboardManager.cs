using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class ScoreboardManager : MonoBehaviour
{
    public GameObject scoreboardPanel;       // Drag ScoreboardPanel from Hierarchy here
    public GameObject scoreEntryPrefab;      // Drag ScoreEntryRow prefab from Project folder here
    public Transform scoreEntryContainer;    // Drag ScoreEntriesContainer from Hierarchy here
    public int maxScoresToShow = 10;        // How many scores to keep/display

    private ScoreboardData scoreboardData;
    private const string SaveKey = "HighScores"; // Key for PlayerPrefs or file name

    void Start()
    {
        // Activate panel on start, ensure it's ready
        if (scoreboardPanel != null)
            scoreboardPanel.SetActive(true);

        LoadScores();
    }

    // Call this when the game ends or player achieves a score
    public void AddScore(string playerName, int score)
    {
        ScoreEntry newEntry = new ScoreEntry { playerName = playerName, score = score };
        scoreboardData.highScores.Add(newEntry);

        // Sort scores descending (highest first)
        scoreboardData.highScores = scoreboardData.highScores.OrderByDescending(s => s.score).ToList();

        // Keep only the top scores
        if (scoreboardData.highScores.Count > maxScoresToShow)
        {
            scoreboardData.highScores = scoreboardData.highScores.GetRange(0, maxScoresToShow);
        }

        SaveScores();
    }

    void SaveScores()
    {
        string json = JsonUtility.ToJson(scoreboardData);
        PlayerPrefs.SetString(SaveKey, json); // Simple saving using PlayerPrefs
        PlayerPrefs.Save();
        Debug.Log("Scores Saved: " + json);
    }

    void LoadScores()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string json = PlayerPrefs.GetString(SaveKey);
            scoreboardData = JsonUtility.FromJson<ScoreboardData>(json);
            Debug.Log("Scores Loaded: " + json);
        }
        else
        {
            scoreboardData = new ScoreboardData(); // Create new data if none exists
            Debug.Log("No scores found. Creating new scoreboard data.");
        }
        // Ensure list is sorted even after loading (in case save format changes etc.)
        scoreboardData.highScores = scoreboardData.highScores.OrderByDescending(s => s.score).ToList();
    }

    // Call this to display the scoreboard UI
    public void ShowScoreboard()
    {
        UpdateScoreboardUI();
        if (scoreboardPanel != null)
            scoreboardPanel.SetActive(true);
    }

    // Call this to hide the scoreboard UI (e.g., via a close button)
    public void HideScoreboard()
    {
        if (scoreboardPanel != null)
            scoreboardPanel.SetActive(false);
    }

    void UpdateScoreboardUI()
    {
        // Clear previous entries
        foreach (Transform child in scoreEntryContainer)
        {
            Destroy(child.gameObject);
        }

        // Populate with current high scores
        for (int i = 0; i < scoreboardData.highScores.Count; i++)
        {
            GameObject entryGO = Instantiate(scoreEntryPrefab, scoreEntryContainer);
            ScoreEntryUI entryUI = entryGO.GetComponent<ScoreEntryUI>(); // We'll create this script next

            if (entryUI != null)
            {
                ScoreEntry currentEntry = scoreboardData.highScores[i];
                entryUI.SetData(i + 1, currentEntry.playerName, currentEntry.score);
            }
            else
            {
                Debug.LogError("ScoreEntryPrefab is missing the ScoreEntryUI script!");
            }
        }
    }

    // --- Example Test Function (Optional) ---
    // You can call this from a test button temporarily
    public void AddTestScore()
    {
        string[] names = { "Alice", "Bob", "Charlie", "David", "Eve" };
        string randomName = names[Random.Range(0, names.Length)];
        int randomScore = Random.Range(10, 1000);
        AddScore(randomName, randomScore);
        Debug.Log($"Added test score: {randomName} - {randomScore}");
        // Optional: Automatically show after adding for testing
        // ShowScoreboard();
    }
}