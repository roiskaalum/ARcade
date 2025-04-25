using System.Collections.Generic;
using UnityEngine;

public class Raycasting : MonoBehaviour
{
    private Transform selectedBall = null;
    private float hitDistance = 0f;
    [SerializeField] private float moveSmoothness = 10f; //Højere = mere hastighed
    private Vector2 lastScreenPos;
    private Vector3 velocity;
    private float velocityFactor = 0.01f;
    private Rigidbody rb;
    private Queue<Vector2> positionHistory = new Queue<Vector2>();
    [SerializeField] private int maxHistory = 50;
    [SerializeField] private float zBoost = 1.5f;



    void Update()
    {
        // Input til touch
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPos = touch.position;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    TrySelectBall(touchPos);
                    break;

                case TouchPhase.Moved:
                    if (selectedBall != null)
                        MoveBall(touchPos);
                    break;

                case TouchPhase.Ended:

                case TouchPhase.Canceled:
                    if (selectedBall != null)
                        ReleaseBall();
                    break;

            }
        }

        // Input til mus
        else
        {
            
            if (Input.GetMouseButtonDown(0))
            {
                TrySelectBall(Input.mousePosition);
            }

            else if (Input.GetMouseButton(0) && selectedBall != null)
            {
                MoveBall(Input.mousePosition);
            }

            else if (Input.GetMouseButtonUp(0) && selectedBall != null)
            {
                ReleaseBall();
            }
        }
    }

    private void TrySelectBall(Vector2 screenPosition)
    {
        
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.CompareTag("Ball"))
            {
                
                selectedBall = hit.transform;
                hitDistance = Vector3.Distance(Camera.main.transform.position, hit.point);
                
                rb = selectedBall.GetComponent<Rigidbody>();
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
                
                lastScreenPos = screenPosition;            
            }
        }
    }

    private void MoveBall(Vector2 screenPosition)
    {
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(new Vector3 (screenPosition.x, screenPosition.y, hitDistance));
        targetPos.z = selectedBall.position.z;

        Vector3 newPos = Vector3.Lerp(selectedBall.position, targetPos, Time.deltaTime * moveSmoothness);
        rb.MovePosition(newPos);

        // Gemmer bevægelse som velocity
        velocity = (screenPosition - lastScreenPos) / Time.deltaTime;
        lastScreenPos = screenPosition;

        // Tjekekr for mængden af queues i positionHistory til udregning af swipe kraft/retning
        positionHistory.Enqueue(screenPosition);
        if (positionHistory.Count > maxHistory)
        {
            positionHistory.Dequeue();
        }

    }

    private void ReleaseBall()
    {
        if (positionHistory.Count >= 2)
        {
            Vector2 releaseScreenPos = lastScreenPos; //Hvor bolden bliver sluppet
            float swipeSpeed = velocity.magnitude; //Hvor hurtigt der blev swipet
            Vector2 start = positionHistory.Peek();
            Vector2 end = lastScreenPos;
            Vector2 avgVelocity = (end - start) / (Time.deltaTime * positionHistory.Count);

            rb.isKinematic = false;

            // Konverter velocity til world space
            Vector3 from = Camera.main.ScreenToWorldPoint(new Vector3(lastScreenPos.x, lastScreenPos.y, hitDistance));
            Vector3 to = Camera.main.ScreenToWorldPoint(new Vector3(lastScreenPos.x + velocity.x, lastScreenPos.y + velocity.y, hitDistance));
            Vector3 worldVelocity = (to - from).normalized;

            float screenHeight = Screen.height;
            float releaseHeightFactor = releaseScreenPos.y / screenHeight; // 0 some nederste værdi, 1 som øverst værdi
            float yBoost = Mathf.Lerp(0.5f, 2f, releaseHeightFactor*2); //Tilpas værdier
            float zBoost = Mathf.Clamp(swipeSpeed * 0.01f, 0f, 3f); //Justér faktor og max

            worldVelocity.y *= yBoost;
            worldVelocity.z += zBoost;

            rb.AddForce(worldVelocity * swipeSpeed * velocityFactor, ForceMode.Impulse);

            Vector3 angularDirection = new Vector3(velocity.y, -velocity.x, 0).normalized;
            float spinAmount = velocity.magnitude * 0.5f;
            rb.angularVelocity = angularDirection * spinAmount;

            //selectedBall = null;
            positionHistory.Clear();
        }
    }

}
