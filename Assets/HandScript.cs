using UnityEngine;
using UnityEngine.InputSystem;

public partial class HandScript : MonoBehaviour
{
    [SerializeField] private float orbitDistance = 1.2f;
    [SerializeField] private Transform playerTransform;


    [Header("References")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint; // Create an empty child on Hand at the tip

    [Header("Stats")]
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float fireRate = 0.2f; // Time between shots
    [SerializeField] private float maxRange = 10f;
    [SerializeField] [Range(0, 20)] private float accuracyWeight = 5f; // Higher = more spread

    private float nextFireTime;


    private Camera mainCamera;

    // This matches the "Attack" action in Input System
    void OnAttack(InputValue value)
    {
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        // 1. Calculate Spread (Accuracy)
        float spread = Random.Range(-accuracyWeight, accuracyWeight);
        Quaternion bulletRotation = transform.rotation * Quaternion.Euler(0, 0, spread);

        // 2. Spawn Bullet
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, bulletRotation);
        
        // 3. Initialize Bullet
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        bulletScript.Setup(bulletSpeed, maxRange);
    }

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