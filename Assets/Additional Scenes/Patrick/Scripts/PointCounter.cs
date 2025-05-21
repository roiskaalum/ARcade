using UnityEngine;
using TMPro;

public class PointCounter : MonoBehaviour
{
    public int cansHit = 0;
    public int ballsLeft = 2;
    public int totalPoints = 0;
    public int points = 0;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI ballsText;

    // Metode som kaldes ved genstart af spillet
    public void OnEnable()
    {
        cansHit = 0;
        ballsLeft = 3;
        //totalPoints = 0;
        points = 0;
    }

    // Update opdaterer point i scenen
    public void Update()
    {
        switch (cansHit)
            {
                case 0:
                    points = 0;
                    break;
                case 1:
                    points = 50;
                    break;
                case 2:
                    points = 100;
                    break;
                case 3:
                    points = 150;
                    break;
                case 4:
                    points = 200;
                    break;
                case 5:
                    points = 250;
                    break;
                case 6:
                // Evt. udskift til switch indeni anden switch istedet for if, else if, else if opsætning
                if (ballsLeft == 0)
                    {
                        points = 500;
                    }

                else if (ballsLeft == 1)
                    {
                        points = 750;
                    }

                else if (ballsLeft == 2)
                    {
                        points = 1000;
                    }
                break;
            }
        


        pointsText.text = "Points: " + points.ToString();
        ballsText.text = ballsLeft.ToString();
        
    }

    // Metode til at sikre os at ballsLeft ikke kan ryge under 0
    public void BallsSpent()
    {
        ballsLeft--;
        ballsLeft = Mathf.Max(ballsLeft, 0);
    }

    // Metode som tilføjer cans til cans hit
    public void AddCanHit()
    {
        cansHit++;
    }

    
}
