using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine; // Use 'Cinemachine' if on an older version than Unity 6

public class CameraScript : MonoBehaviour
{
    private CinemachineCamera vCam; // Or CinemachineVirtualCamera for older versions
    
    [Header("Zoom Settings")]
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 10f;
    [SerializeField] private float zoomSensitivity = 0.5f;
    [SerializeField] private float smoothTime = 0.1f;

    private float targetZoom;
    private float zoomVelocity = 0f;

    void Start()
    {
        vCam = GetComponent<CinemachineCamera>();
        targetZoom = vCam.Lens.OrthographicSize;
    }

    // This matches the "Zoom" action name in the Input System
    void OnZoom(InputValue value)
    {
        float scrollValue = value.Get<float>();
        
        // Scroll up is usually positive, scroll down is negative.
        // We subtract to "zoom in" (smaller ortho size) when scrolling up.
        targetZoom -= scrollValue * zoomSensitivity;
        
        // Clamping prevents zooming too far in or out
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
    }

    void Update()
    {
        // Smoothly move the lens size toward our target
        vCam.Lens.OrthographicSize = Mathf.SmoothDamp(
            vCam.Lens.OrthographicSize, 
            targetZoom, 
            ref zoomVelocity, 
            smoothTime
        );
    }
}