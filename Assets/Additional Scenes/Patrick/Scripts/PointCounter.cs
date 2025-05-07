using UnityEngine;
using TMPro;

public class PointCounter : MonoBehaviour
{

    

    public int cansHit = 0;
    public int cansLeft = 6;
    public int ballsLeft = 2;
    public int totalPoints = 0;
    public int points = 0;
    public TextMeshProUGUI pointsText;
    // totalPoints uden ballsLeft multiplier

    public void OnEnable()
    {
        cansHit = 0;
        cansLeft = 6;
        ballsLeft = 2;
        totalPoints = 0;
        points = 0;
    }

    public void Update()
    {
        points = cansHit * cansHit;
        totalPoints = points * (ballsLeft + 1);
        if (cansLeft == 0 || ballsLeft == 0)
        {
            pointsText.text = "Total points: " + totalPoints;
        }

        if (ballsLeft > 0 && cansHit != 6)
        {
            pointsText.text = "Points: " + points;
        }
    }


    public void BallsSpent()
    {
        ballsLeft--;
        ballsLeft = Mathf.Max(ballsLeft, 0);
    }

    public void AddCanHit()
    {
        cansHit++;
        cansLeft--;
    }

    
}
