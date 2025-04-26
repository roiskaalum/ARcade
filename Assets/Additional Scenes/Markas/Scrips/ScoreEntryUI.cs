using UnityEngine;
using TMPro; // Use TextMeshPro namespace

public class ScoreEntryUI : MonoBehaviour
{
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;

    // Call this from ScoreboardManager to populate the text fields
    public void SetData(int rank, string playerName, int score)
    {
        if (rankText != null) rankText.text = rank.ToString();
        if (nameText != null) nameText.text = playerName;
        if (scoreText != null) scoreText.text = score.ToString();
    }
}