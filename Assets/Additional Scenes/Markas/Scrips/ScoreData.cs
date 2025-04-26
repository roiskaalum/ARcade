using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScoreEntry
{
    public string playerName;
    public int score;
    public int rank;

}

[System.Serializable]
public class ScoreboardData
{
    public List<ScoreEntry> highScores = new List<ScoreEntry>();
}