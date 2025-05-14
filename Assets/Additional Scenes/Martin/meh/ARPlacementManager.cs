using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ARPlacementManager : MonoBehaviour
{
    [SerializeField] private GameObject placementIndicatorPrefab; // En visuel indikator (valgfri)
    [SerializeField] private GameObject objectToPlace;            // Det du vil placere (fx gameObjects)
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARPlaneManager planeManager;

    private GameObject placementIndicator;
    private Pose placementPose;
    private bool placementPoseIsValid = false;
    private bool hasPlaced = false;

    void Start()
    {
        if (placementIndicatorPrefab != null)
        {
            placementIndicator = Instantiate(placementIndicatorPrefab);
            placementIndicator.SetActive(false);
        }
    }

    void Update()
    {
        if (hasPlaced) return;

        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceObject();
        }
    }

    private void UpdatePlacementPose()
    {
        if (raycastManager == null)
        {
            Debug.LogError("ARPlacementManager: raycastManager is NULL!");
            return;
        }

        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;
        }
    }




    private void UpdatePlacementIndicator()
    {
        if (placementIndicator == null) return;

        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }




    private void PlaceObject()
    {
        if (objectToPlace != null)
        {
            objectToPlace.transform.position = placementPose.position;
            objectToPlace.transform.rotation = placementPose.rotation;
            objectToPlace.SetActive(true);
        }

        hasPlaced = true;

        if (placementIndicator != null)
        {
            placementIndicator.SetActive(false);
        }

        DisableARPlanes();

        GameManager.Instance.StartGameplay("Guest"); 
    }




    private void DisableARPlanes()
    {
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }

        planeManager.enabled = false;
    }
}
