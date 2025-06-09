using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanReset : MonoBehaviour
{
    [SerializeField] private GameObject containerObject; // The Container/Anchor in the scene
    [SerializeField] private GameObject referenceObject; // The current cans parent in the scene
    [SerializeField] private GameObject referencePrefab; // The prefab to instantiate (Cans Parent)
    [SerializeField] private GameObject ball;
    [SerializeField] private PointCounter pointCounter;

    private void Awake()
    {
        ARPrefabBridge.Instance.Register(this);
        ARPrefabBridge.Instance.RegisterGamePrefab(transform.root.gameObject);
        Debug.Log("CanReset script registered with ARPrefabBridge.: " + gameObject.name);
        transform.root.gameObject.SetActive(false); // Disable the root game object initially
    }

    public void Reset()
    {
        Debug.Log($"[CanReset] Reset called on {gameObject.name}");
        Debug.Log($"[CanReset] referenceObject: {referenceObject}, referencePrefab: {referencePrefab}, containerObject: {containerObject}");

        // Destroy the old referenceObject (Cans Parent)
        if (referenceObject != null)
        {
            Destroy(referenceObject);
            Debug.Log("[CanReset] Destroyed old referenceObject.");
        }

        // Instantiate new Cans Parent as a child of the container, at local position (0,0,0)
        referenceObject = Instantiate(referencePrefab, Vector3.zero, Quaternion.identity, containerObject.transform);
        Debug.Log("[CanReset] Instantiated new referenceObject as child of containerObject.");

        Debug.Log($"PointCounter ballsLeft: {pointCounter.ballsLeft}, cansHit: {pointCounter.cansHit}");

        ball.gameObject.SetActive(true);
        pointCounter.gameObject.SetActive(false);
        pointCounter.gameObject.SetActive(true);
        Debug.Log($"PointCounter ballsLeft: {pointCounter.ballsLeft}, cansHit: {pointCounter.cansHit}");
    }

    public IEnumerator Disable()
    {
        yield return new WaitForSeconds(1f);
        transform.root.gameObject.SetActive(false);
        Debug.Log("CanReset script disabled: " + gameObject.name);
        yield return null;
    }
}