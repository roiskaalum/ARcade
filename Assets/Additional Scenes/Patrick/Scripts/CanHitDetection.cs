using TMPro;
using UnityEngine;

public class CanHitDetection : MonoBehaviour
{
    private Transform can;
    private PointCounter pointCounter;
    public TextMeshProUGUI pointsText;

    private bool hasFallen = false;

    void Awake()
    {
        can = transform;
        pointCounter = FindFirstObjectByType<PointCounter>();
        GameObject pointsObj = GameObject.Find("Points");
        if (pointsObj != null)
            pointsText = pointsObj.GetComponent<TextMeshProUGUI>();
        else
            Debug.LogWarning("Points TextMeshProUGUI not found in scene!");
    }

    // Update tjekker om en d�se er faldet ved at tjekke efter y-positionen
    void Update()
    {
        if (!hasFallen && can.position.y < -10f)
        {
            hasFallen = true;
            gameObject.SetActive(false);
            if (pointCounter != null)
                pointCounter.AddCanHit();
        }
    }
}
