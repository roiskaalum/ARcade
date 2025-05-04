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
    [SerializeField] private int maxHistory = 10;
    [SerializeField] private float sideAngleFactor = 0.5f; // 0 = ingen vinkel, 1 = fuld vinkel



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
                        selectedBall = null;
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
                selectedBall = null;
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
                rb = selectedBall.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    hitDistance = Vector3.Distance(Camera.main.transform.position, hit.point);
                    rb.isKinematic = true;
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    lastScreenPos = screenPosition;            
                }
            }
        }
    }

    private void MoveBall(Vector2 screenPosition)
    {
        if (selectedBall == null || rb == null)
            return;

        else
        {
            Vector3 targetPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, hitDistance));
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

    }

    private void ReleaseBall()
    {
        if (positionHistory.Count >= 2)
        {
            Vector2 releaseScreenPos = lastScreenPos;

            float screenHeight = Screen.height;
            float screenWidth = Screen.width;
            float releaseHeightFactor = releaseScreenPos.y / screenHeight;

            float swipeSpeed = velocity.magnitude;
            Vector2 start = positionHistory.Peek();
            Vector2 end = lastScreenPos;

            rb.isKinematic = false;

            // Beregn basis fremad-retning
            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0f; // Fjern hældning i y-retningen
            forward.Normalize();

            // Beregn y-boost baseret på hvor højt bolden blev sluppet
            float yBoost = Mathf.Lerp(0.0f, 0.8f, releaseHeightFactor * 0.5f);

            // Begræns sidepåvirkning
            float sideInfluence = Mathf.Clamp((end.x - start.x) / screenWidth, -0.5f, 0.5f);
            Vector3 sideDir = Camera.main.transform.right * sideInfluence * sideAngleFactor;

            // Saml hele kast-retningen
            Vector3 worldVelocity = forward + Vector3.up * yBoost + sideDir;

            // Boost fremad i z-retning afhængig af swipe speed
            float zBoost = Mathf.Clamp(swipeSpeed * 0.01f, 0f, 0.01f);
            worldVelocity += forward * zBoost;

            // Kast bolden
            rb.AddForce(worldVelocity.normalized * swipeSpeed * velocityFactor, ForceMode.Impulse);

            // Giv lidt spin
            Vector3 angularDirection = new Vector3(velocity.y, -velocity.x, 0).normalized;
            float spinAmount = velocity.magnitude * 2f;
            rb.angularVelocity = angularDirection * spinAmount;

            // Ryd positionshistorik
            positionHistory.Clear();
        }
    }




}
