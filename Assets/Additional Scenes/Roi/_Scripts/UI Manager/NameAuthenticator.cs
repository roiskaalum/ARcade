using UnityEngine;
using TMPro; // For TMP_InputField
using UnityEngine.UI;
using System.Collections;

public class NameAuthenticator : MonoBehaviour
{
    [Tooltip("Can be Empty")]
    [SerializeField] private TMP_InputField nameInputField; // Reference to the input field
    
    [Tooltip("Can be Empty")]
    [SerializeField] private ScoreboardManager scoreboardManager; // Reference to the ScoreboardManager

    [Tooltip("Cannot be Empty")]
    [SerializeField] private GameObject PopupConfirmUI;
    [Tooltip("Cannot be Empty")]
    [SerializeField] private GameObject PopupNameEmptyUI;

    private void Awake()
    {
        // Ensure the input field is assigned
        if (nameInputField == null)
        {
            nameInputField = GetComponent<TMP_InputField>();
            if (nameInputField == null)
            {
                Debug.LogError("NameAuthenticator: TMP_InputField is not assigned or found.");
            }
        }

        // Find sibling panels dynamically based on their names
        if (nameInputField != null)
        {
            Transform parentTransform = nameInputField.transform.parent; // Get the parent of the TMP_InputField
            if (parentTransform != null)
            {
                foreach (Transform child in parentTransform)
                {
                    // Check for the naming convention
                    if (child.name == "Panel - ConfirmPopupUI")
                    {
                        PopupConfirmUI = child.gameObject;
                    }
                    else if (child.name == "Panel - EmptyNamePopupUI")
                    {
                        PopupNameEmptyUI = child.gameObject;
                    }
                }

                // Log errors if panels are not found
                if (PopupConfirmUI == null)
                {
                    Debug.LogError("NameAuthenticator: PopupConfirmUI not found among siblings.");
                }
                if (PopupNameEmptyUI == null)
                {
                    Debug.LogError("NameAuthenticator: PopupNameEmptyUI not found among siblings.");
                }
            }
        }

        // Ensure the ScoreboardManager is assigned
        if (scoreboardManager == null)
        {
            scoreboardManager = FindFirstObjectByType<ScoreboardManager>();
            if (scoreboardManager == null)
            {
                Debug.LogError("NameAuthenticator: ScoreboardManager not found in the scene.");
            }
        }

        // Set initial states for the popups
        if (PopupConfirmUI != null) PopupConfirmUI.SetActive(false);
        if (PopupNameEmptyUI != null) PopupNameEmptyUI.SetActive(false);
    }

    private IEnumerator DisablePopupAfterClick()
    {
        // Wait for the user to click anywhere on the screen
        while(!Input.GetMouseButtonDown(0))
        {
            yield return null;
        }

        PopupNameEmptyUI.SetActive(false);
    }

    public void ValidateName()
    {
        string enteredName = nameInputField.text.Trim();
        Debug.Log($"NameAuthenticator: Validating name: {enteredName}");

        if (string.IsNullOrEmpty(enteredName))
        {
            PopupNameEmptyUI.SetActive(true);
            StartCoroutine(DisablePopupAfterClick());
            return;
        }

        // Check if the name exists in the scoreboard
        var existingEntry = scoreboardManager.scoreData.scores.Find(entry => entry.playerName == enteredName);

        if (existingEntry != null)
        {
            if (existingEntry.score == -1)
            {
                // Name exists but has no score, allow the player to proceed
                StartGameWithName(enteredName);
            }
            else
            {
                // Name exists with a score, ask for confirmation
                PopupConfirmUI.SetActive(true);
            }
        }
        else
        {
            // Name is new, add it to the scoreboard with a score of -1
            scoreboardManager.AddScoreEntry(enteredName, -1);
            StartGameWithName(enteredName);
        }
    }

    public void OnConfirmButtonClicked()
    {
        // Hide the confirmation UI
        PopupConfirmUI.SetActive(false);

        // Proceed with the game
        string enteredName = nameInputField.text.Trim();
        StartGameWithName(enteredName);
    }

    public void OnCancelButtonClicked()
    {
        // Hide the confirmation UI
        PopupConfirmUI.SetActive(false);
    }

    private void StartGameWithName(string playerName)
    {
        Debug.Log($"Starting game with player name: {playerName}");
        // Temporary database entry
        scoreboardManager.AddScoreEntry(playerName, -1);

        // Add logic to start the game here
        // SomeStartGameMethod(playerName);
    }
    public void GuestStart()
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
}
