using System.Collections.Generic;
using UnityEngine;

public class CanReset : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cansParent;  // Parent of all cans
    [SerializeField] private PointCounter pointCounter;  // Optional scoring system

    [SerializeField] private GameObject ball;

    private List<Rigidbody> _canRigidbodies = new List<Rigidbody>();
    private List<Vector3> _originalPositions = new List<Vector3>();
    private List<Quaternion> _originalRotations = new List<Quaternion>();

    // private ARPrefabBridge arPrefabBridge;

    private void Awake() => RememberCans();



    public void RememberCans()
    {

        if (!cansParent) return;

        _canRigidbodies.Clear();
        _originalPositions.Clear();
        _originalRotations.Clear();

        foreach (Transform child in cansParent)
        {
            if (child.TryGetComponent<Rigidbody>(out var rb))
            {
                _canRigidbodies.Add(rb);
                _originalPositions.Add(child.position);
                _originalRotations.Add(child.rotation);
            }
        }
        // ARPrefabBridge.Instance.Register(this);
    }

    public void ResetCans()
    {
        
        pointCounter.gameObject.SetActive(false);  // Reset score if available
        pointCounter.gameObject.SetActive(true);  // Reset score if available

        ball.gameObject.SetActive(false);  // Deactivate ball

        for (int i = 0; i < _canRigidbodies.Count; i++)
        {
            if (!_canRigidbodies[i]) continue;

            Rigidbody rb = _canRigidbodies[i];

            // Force kinematic ON for reset
            rb.isKinematic = true;

            // Apply position/rotation reset
            rb.transform.SetPositionAndRotation(_originalPositions[i], _originalRotations[i]);
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Always disable kinematic after
            rb.isKinematic = false;
        }
    }
}