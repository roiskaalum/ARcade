using UnityEngine;
using TMPro; // For TMP_InputField
using UnityEngine.UI; // For Button

public class NameAuthenticator : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField; // Reference to the input field
    
    [SerializeField] private ScoreboardManager scoreboardManager; // Reference to the ScoreboardManager

    private void Awake()
    {
        // Ensure the input field and confirm button are assigned
        if (nameInputField == null)
        {
            nameInputField = GetComponent<TMP_InputField>();
            if (nameInputField == null)
            {
                Debug.LogError("NameAuthenticator: TMP_InputField is not assigned or found.");
            }
        }

        if (scoreboardManager == null)
        {
            scoreboardManager = FindFirstObjectByType<ScoreboardManager>();
            if (scoreboardManager == null)
            {
                Debug.LogError("NameAuthenticator: ScoreboardManager not found in the scene.");
            }
        }

    }

    public void ValidateName()
    {
        string enteredName = nameInputField.text.Trim();

        if (string.IsNullOrEmpty(enteredName))
        {
            Debug.LogWarning("NameAuthenticator: Name field is empty.");
            return;
        }

        // Check if the name exists in the scoreboard
        var existingEntry = scoreboardManager.scoreData.scores.Find(entry => entry.playerName == enteredName);

        if (existingEntry != null)
        {
            if (existingEntry.score == -1)
            {
                // Name exists but has no score, allow the player to proceed
                Debug.Log($"Name '{enteredName}' exists but has no score. Proceeding...");
                StartGameWithName(enteredName);
            }
            else
            {
                // Name exists with a score, ask for confirmation
                Debug.Log($"Name '{enteredName}' already exists with a score of {existingEntry.score}. Asking for confirmation...");
                AskForConfirmation(enteredName);
            }
        }
        else
        {
            // Name is new, add it to the scoreboard with a score of -1
            Debug.Log($"Name '{enteredName}' is new. Adding to the scoreboard...");
            scoreboardManager.AddScoreEntry(enteredName, -1);
            StartGameWithName(enteredName);
        }
    }

    private void AskForConfirmation(string enteredName)
    {
        // Implement a UI popup or confirmation dialog here
        // For simplicity, we'll just log the confirmation request
        Debug.Log($"Confirmation required: Do you want to proceed with the name '{enteredName}'?");

        // If confirmed, proceed with the game
        StartGameWithName(enteredName);
    }

    private void StartGameWithName(string playerName)
    {
        Debug.Log($"Starting game with player name: {playerName}");
        // Add logic to start the game here
    }
}
