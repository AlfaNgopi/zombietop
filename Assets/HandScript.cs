using UnityEngine;
using UnityEngine.InputSystem;

public partial class HandScript : MonoBehaviour
{
    [SerializeField] private float orbitDistance = 1.2f;
    [SerializeField] private Transform playerTransform;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        
        // If you didn't assign the player in the inspector, find the parent
        if (playerTransform == null)
            playerTransform = transform.parent;
    }

    void Update()
    {
        // 1. Get Mouse Position in World Space
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, mainCamera.nearClipPlane));

        // 2. Calculate Direction from Player to Mouse
        Vector2 direction = (mouseWorldPos - playerTransform.position).normalized;

        // 3. Position the Hand at a fixed distance (Orbit)
        transform.position = (Vector2)playerTransform.position + (direction.normalized * orbitDistance);

        // 4. (Optional) Rotate the hand to face the mouse
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}