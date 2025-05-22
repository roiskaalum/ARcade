using TMPro;
using UnityEngine;

public class CanHitDetection : MonoBehaviour
{
    [SerializeField] Transform can;
    [SerializeField] PointCounter pointCounter;
    public TextMeshProUGUI pointsText;

    private bool hasFallen = false;

    
    // Update tjekker om en d�se er faldet ved at tjekke efter y-positionen
    void Update()
    {
        if (!hasFallen && can.position.y < -10f)
        {
            hasFallen = true;
            gameObject.SetActive(false);
            pointCounter.AddCanHit();
        }
    }
}
