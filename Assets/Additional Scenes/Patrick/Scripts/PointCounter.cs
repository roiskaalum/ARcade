using UnityEngine;
using TMPro;

public class PointCounter : MonoBehaviour
{

    

    public int cansHit = 0;
    public int ballsLeft = 2;
    public int totalPoints = 0;
    public int points = 0;
    public TextMeshProUGUI pointsText;

    public void OnEnable()
    {
        cansHit = 0;
        ballsLeft = 3;
        totalPoints = 0;
        points = 0;
    }

    public void Update()
    {
        points = cansHit * cansHit;
        totalPoints = points * (ballsLeft + 1);
        if (cansHit == 6 || ballsLeft == 0)
        {
            pointsText.text = "Points: " + totalPoints;
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
    }

    
}
