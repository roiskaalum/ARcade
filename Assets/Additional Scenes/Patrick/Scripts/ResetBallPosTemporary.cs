using UnityEngine;

public class ResetBallPosTemporary : MonoBehaviour
{
    [SerializeField] private Transform ball;
    [SerializeField] private Rigidbody rb;
    private Vector3 startPosition;
    public PointCounter pointCounter;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (ball != null)
        {
            startPosition = ball.position;
        }
    }

    public void ResetBall()
    {
        if (ball !=null && rb != null && pointCounter.ballsLeft > 0 && pointCounter.cansLeft != 0)
        {            
            rb.angularVelocity = Vector3.zero;
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
            ball.position = startPosition;
            pointCounter.BallsSpent();
        }
    }

}
