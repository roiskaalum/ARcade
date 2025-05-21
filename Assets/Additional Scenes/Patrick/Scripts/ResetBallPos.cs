using UnityEngine;

public class ResetBallPosTemporary : MonoBehaviour
{
    [SerializeField] private Transform ball;
    [SerializeField] private Rigidbody rb;
    private Vector3 startPosition;
    public PointCounter pointCounter;
    public bool isThrown = false;

    // Start sætter startpositionen til boldens position i scenen så den senere kan sættes til startpositionen
    void Start()
    {
        if (ball != null)
        {
            startPosition = ball.position;
        }
    }

    // Update står for logikken der resetter boldens position
    void Update()
    {
        if (ball.transform.position.z > 2 && isThrown == false)
        {
            pointCounter.BallsSpent();
            isThrown = true;
        }

        if (ball !=null && rb != null && pointCounter.ballsLeft > 0 && pointCounter.cansHit != 6 && (ball.transform.position.y < -25 || ball.transform.position.z > 30))
        {
            rb.angularVelocity = Vector3.zero;
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
            ball.position = startPosition;
            isThrown = false;
        }

        if (ball != null && rb != null && (pointCounter.cansHit == 6 || pointCounter.ballsLeft == 0) && (ball.transform.position.y < -25 || ball.transform.position.z > 30))
        {
            ball.gameObject.SetActive(false);
        }
    }
}
