using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CameraManager : MonoBehaviour
{
    // References to each camera
    public Camera mainCamera;
    public Camera wideAngleCamera;
    public Camera mediumShotCamera;
    public Camera closeUpCamera;

    // Zoom and transition settings
    public float zoomFieldOfView = 20f;
    public float normalFieldOfView = 60f;
    public float zoomSpeed = 5f;
    public float transitionSpeed = 2f;

    // Dynamic movement settings
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;

    // Slow-motion settings
    public float slowMotionFactor = 0.5f;

    private Camera activeCamera;
    private bool isZooming = false;
    private bool isTransitioning = false;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Camera targetCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Camera Manager Started");
        SetActiveCamera(mainCamera);  // Start with the main camera active
        Debug.Log("Active Camera: " + activeCamera.name);
    }

    // Update is called once per frame
    void Update()
    {
        HandleCameraSwitching();
        HandleZoom(); // Zoom handling for all cameras
        HandleTransitions();
        HandleDynamicMovement();
        HandleSlowMotion();
    }

    private void HandleCameraSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            StartCameraTransition(mainCamera);
            Debug.Log("Switched to Main Camera");
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCameraTransition(wideAngleCamera);
            Debug.Log("Switched to Wide Angle Camera");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCameraTransition(mediumShotCamera);
            Debug.Log("Switched to Medium Shot Camera");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCameraTransition(closeUpCamera);
            Debug.Log("Switched to Close Up Camera");
        }
    }

    private void SetActiveCamera(Camera cameraToActivate)
    {
        // Disable all cameras first
        mainCamera.enabled = false;
        wideAngleCamera.enabled = false;
        mediumShotCamera.enabled = false;
        closeUpCamera.enabled = false;

        // Enable the target camera
        cameraToActivate.enabled = true;
        activeCamera = cameraToActivate;
        Debug.Log("Camera Set to: " + activeCamera.name);
    }

    private void StartCameraTransition(Camera cameraToActivate)
    {
        if (activeCamera != cameraToActivate)
        {
            startPosition = activeCamera.transform.position;
            startRotation = activeCamera.transform.rotation;
            targetCamera = cameraToActivate;
            isTransitioning = true;
            Debug.Log("Transitioning to camera: " + targetCamera.name);
        }
        else
        {
            Debug.Log("Already at target camera: " + activeCamera.name);
        }
    }

    private void HandleZoom()
    {
        // Zoom functionality works on all cameras after transition is complete
        if (isTransitioning) return; // Don't allow zooming while transitioning

        if (Input.GetKey(KeyCode.Z))
        {
            isZooming = true;
            Debug.Log("Zooming In on Camera: " + activeCamera.name);
        }
        if (Input.GetKeyUp(KeyCode.Z))
        {
            isZooming = false;
            Debug.Log("Zooming Out on Camera: " + activeCamera.name);
        }

        // Apply zoom effect to active camera
        if (isZooming)
        {
            activeCamera.fieldOfView = Mathf.Lerp(activeCamera.fieldOfView, zoomFieldOfView, zoomSpeed * Time.deltaTime);
        }
        else
        {
            activeCamera.fieldOfView = Mathf.Lerp(activeCamera.fieldOfView, normalFieldOfView, zoomSpeed * Time.deltaTime);
        }

        Debug.Log("Current FOV: " + activeCamera.fieldOfView); // Log to check if zoom is being applied
    }

    private void HandleTransitions()
    {
        if (isTransitioning)
        {
            // Move and rotate the camera gradually
            activeCamera.transform.position = Vector3.Lerp(activeCamera.transform.position, targetCamera.transform.position, Time.deltaTime * transitionSpeed);
            activeCamera.transform.rotation = Quaternion.Lerp(activeCamera.transform.rotation, targetCamera.transform.rotation, Time.deltaTime * transitionSpeed);

            // Debug information about current transition state
            Debug.Log("Transitioning to Camera: " + targetCamera.name);
            Debug.Log("Position Difference: " + Vector3.Distance(activeCamera.transform.position, targetCamera.transform.position));
            Debug.Log("Rotation Difference: " + Quaternion.Angle(activeCamera.transform.rotation, targetCamera.transform.rotation));

            // Check if transition is complete
            if (Vector3.Distance(activeCamera.transform.position, targetCamera.transform.position) < 0.1f &&
                Quaternion.Angle(activeCamera.transform.rotation, targetCamera.transform.rotation) < 1f)
            {
                SetActiveCamera(targetCamera);  // Set the target camera as active
                isTransitioning = false;  // Stop the transition
                Debug.Log("Transition Complete to: " + targetCamera.name);
            }
        }
    }

    private void HandleDynamicMovement()
    {
        if (activeCamera == mainCamera && waypoints.Length > 0)
        {
            // Move the main camera between waypoints
            activeCamera.transform.position = Vector3.MoveTowards(activeCamera.transform.position, waypoints[currentWaypointIndex].position, Time.deltaTime);

            if (Vector3.Distance(activeCamera.transform.position, waypoints[currentWaypointIndex].position) < 0.1f)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                Debug.Log("Waypoint Reached: " + currentWaypointIndex);
            }
        }
    }

    // Handle Slow-Motion
    private void HandleSlowMotion()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Time.timeScale = slowMotionFactor; // Slow down the game time
            Debug.Log("Slow Motion Activated");
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Time.timeScale = 1f; // Reset to normal speed
            Debug.Log("Slow Motion Deactivated");
        }
    }
}
    
