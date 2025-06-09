using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARGameSpawner : MonoBehaviour
{
    [Header("Existing Game Prefab (Disabled in Scene)")]
    public ARPrefabBridge arPrefabBridge; // Reference to the ARPrefabBridge
    public GameObject gamePrefab; // The game prefab to spawn

    [Header("Spawn Settings")]
    public float forwardOffset = 1.5f;  // Distance in front of camera
    public float verticalOffset = 0.0f; // Optional floor adjustment
    public float yRotationOffset = 0f;  // Flip to face user (e.g., 180)

    private bool hasSpawned = false;

    private float debugLogTimer = 0f;

    public float debugLogInterval = 10f; // Interval for debug logs

    void Start()
    {
        if (arPrefabBridge == null)
        {
            arPrefabBridge = FindFirstObjectByType<ARPrefabBridge>();
            if (arPrefabBridge == null)
            {
                Debug.LogError("ARPrefabBridge not found in the scene. Critical error.");
                return;
            }
        }
        SpawnGame();
    }

    private void SpawnGame()
    {
        Debug.Log("Spawning game prefab...");
        Debug.LogWarning(gamePrefab == null ? "gamePrefab is null!" : "gamePrefab is NOT null!");
        if (hasSpawned)
        {
            return;
        }

        if (gamePrefab == null)
        {
            gamePrefab = arPrefabBridge.gamePrefab;
            Debug.LogWarning(gamePrefab + " || " + arPrefabBridge.gamePrefab);
        }
        if (gamePrefab == null)
        {
            Debug.LogWarning("No game prefab found to spawn!");
            return;
        }

        Transform cam = Camera.main.transform;

        // Flatten forward to ground plane
        Vector3 forward = new Vector3(cam.forward.x, 0f, cam.forward.z).normalized;
        Vector3 spawnPosition = cam.position + forward * forwardOffset + Vector3.up * verticalOffset;

        // Only Y rotation allowed
        float yRotation = cam.eulerAngles.y + yRotationOffset;
        Quaternion spawnRotation = Quaternion.Euler(0f, yRotation, 0f);

        // Move and rotate before enabling
        gamePrefab.transform.SetPositionAndRotation(spawnPosition, spawnRotation);
        gamePrefab.SetActive(true);

        hasSpawned = true;
        ARTrackedImageManager trackedImageManager = FindFirstObjectByType<ARTrackedImageManager>();
        trackedImageManager.enabled = false; // Disable image tracking after spawning
        foreach(var trackedImage in trackedImageManager.trackables)
        {
            Destroy(trackedImage.gameObject); // Destroy all tracked images
        }
    }

    public void ResetSpawnedFlag()
    {
        hasSpawned = false;
    }

    void Update()
    {
        // Log camera and prefab positions every debugLogInterval seconds
        debugLogTimer += Time.unscaledDeltaTime;
        if (debugLogTimer >= debugLogInterval)
        {
            debugLogTimer = 0f;
            LogCameraAndPrefab();
        }
    }

    private void LogCameraAndPrefab()
    {
        var cam = Camera.main;
        if (cam != null)
        {
            Debug.Log($"[DEBUG] Camera Position: {cam.transform.position}, Rotation: {cam.transform.rotation.eulerAngles}");
        }
        else
        {
            Debug.LogWarning("[DEBUG] Camera.main is null!");
        }

        if (gamePrefab != null)
        {
            Debug.Log($"[DEBUG] gamePrefab Position: {gamePrefab.transform.position}, Rotation: {gamePrefab.transform.rotation.eulerAngles}");
        }
        else
        {
            Debug.LogWarning("[DEBUG] gamePrefab is null!");
        }
    }
}
