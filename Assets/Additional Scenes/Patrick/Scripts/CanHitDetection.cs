using TMPro;
using UnityEngine;

public class CanHitDetection : MonoBehaviour
{
    [SerializeField] Transform can;
    [SerializeField] PointCounter pointCounter;
    public TextMeshProUGUI pointsText;

    private bool hasFallen = false;

    
    // Update is called once per frame
    void Update()
    {
        if (!hasFallen && can.position.y < -21f)
        {
            hasFallen = true;
            pointCounter.AddCanHit();
        }

    }
}
