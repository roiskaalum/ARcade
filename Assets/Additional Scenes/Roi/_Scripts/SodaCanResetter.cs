using UnityEngine;
using System.Collections.Generic;

public class SodaCanResetter : MonoBehaviour
{
    [SerializeField] private GameObject referenceObject; // Reference to the empty game object
    private List<Transform> childTransforms = new List<Transform>();
    private Dictionary<Transform, Vector3> initialPositions = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, Quaternion> initialRotations = new Dictionary<Transform, Quaternion>();

    void Start()
    {
        // Store the initial transforms of all children
        foreach (Transform child in referenceObject.transform)
        {
            childTransforms.Add(child);
            initialPositions[child] = child.position;
            initialRotations[child] = child.rotation;
        }
    }

    public void Reset()
    {
        foreach (Transform child in childTransforms)
        {
            Rigidbody rb = child.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; // Disable physics while resetting
            }

            // Reset position and rotation
            child.position = initialPositions[child];
            child.rotation = initialRotations[child];

            if (rb != null)
            {
                rb.isKinematic = false; // Re-enable physics
            }
        }
    }
}
