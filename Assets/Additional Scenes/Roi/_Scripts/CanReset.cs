using System.Collections.Generic;
using UnityEngine;

public class CanReset : MonoBehaviour
{
    [SerializeField] private GameObject referenceObject; // Reference to the empty game object
    private List<Transform> childTransforms = new List<Transform>();
    private Dictionary<Transform, Vector3> initialPositions = new Dictionary<Transform, Vector3>();
    private Dictionary<Transform, Quaternion> initialRotations = new Dictionary<Transform, Quaternion>();

    [SerializeField] private GameObject ball;

    private void Awake()
    {
        // Store the initial transforms of all children
        foreach (Transform child in referenceObject.transform)
        {
            childTransforms.Add(child);
            initialPositions[child] = child.position;
            initialRotations[child] = child.rotation;
            Debug.Log($"Child: {child.name}, Initial Position: {initialPositions[child]}, Initial Rotation: {initialRotations[child]}");
        }
        ARPrefabBridge.Instance.Register(this);
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
            Debug.Log(child.position + " POSITION" + " | " + child.rotation + " ROTATION");
            // Reset position and rotation
            child.position = initialPositions[child];
            child.rotation = initialRotations[child];

            if (rb != null)
            {
                rb.isKinematic = false; // Re-enable physics
            }
        }
        ball.gameObject.SetActive(true);
    }

    private void Update()
    {
        Debug.Log(referenceObject.transform.position + " POS | ROT " + referenceObject.transform.rotation);
    }
}