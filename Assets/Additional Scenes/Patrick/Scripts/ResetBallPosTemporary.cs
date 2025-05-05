using UnityEngine;

public class ResetBallPosTemporary : MonoBehaviour
{
    [SerializeField] private Transform ball;
    [SerializeField] private Rigidbody rb;
    private Vector3 startPosition;

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
        if (ball !=null && rb != null)
        {
            rb.angularVelocity = Vector3.zero;
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
            ball.position = startPosition;
        }
    }

}
