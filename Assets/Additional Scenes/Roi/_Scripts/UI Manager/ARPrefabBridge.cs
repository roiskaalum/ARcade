using UnityEngine;

public class ARPrefabBridge : MonoBehaviour
{
    public static ARPrefabBridge Instance { get; private set; }
    public CanReset canResetReference { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Call this from the prefab's Awake() to register itself
    public void Register(CanReset script)
    {
        canResetReference = script;
    }

    // // Call this when the prefab is destroyed. But in our case, we don't destroy the prefab, so this is not needed.
    // public void Unregister()
    // {
    //     canResetReference = null;
    // }
}