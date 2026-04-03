using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System; // For Action delegate
// Debug = UnityEngine.Debug to avoid confusion with System.Diagnostics.Debug



public class WeaponScript : MonoBehaviour
{
    public static event Action OnAmmoChanged;



    [SerializeField] private float orbitDistance = 1.2f;
    [SerializeField] private Transform playerTransform;



    [Header("References")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint; // Create an empty child on Hand at the tip

    [Header("Stats")]
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float fireRate = 0.2f; // Time between shots
    [SerializeField] private float maxRange = 10f;
    [SerializeField][Range(0, 20)] private float accuracyWeight = 5f; // Higher = more spread

    [Header("Magazine Settings")]
    [SerializeField] private int magSize = 12;
    [SerializeField] private float reloadTime = 1.5f;

    private float nextFireTime;
    private bool isReloading = false;
    private int currentAmmo = 12; // Start with a full mag




    private Camera mainCamera;

    public int getAmmo()
    {
        return currentAmmo;
    }

    public bool isCurrentlyReloading()
    {
        return isReloading;
    }

    public float getReloadTime()
    {
        return reloadTime;
    }

    public void Reload()
    {
        if (!isReloading && currentAmmo < magSize)
        {
            StartCoroutine(ReloadRoutine());
        }
    }


    public void Shoot()
    {
        if (currentAmmo <= 0 || isReloading || Time.time < nextFireTime)
            return;

        nextFireTime = Time.time + fireRate;
        currentAmmo--;
        OnAmmoChanged?.Invoke(); // Notify UI of ammo change

        // 1. Calculate Spread (Accuracy)
        float spread = UnityEngine.Random.Range(-accuracyWeight, accuracyWeight);
        Quaternion bulletRotation = transform.rotation * Quaternion.Euler(0, 0, spread);

        // 2. Spawn Bullet
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, bulletRotation);

        // 3. Initialize Bullet
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        bulletScript.Setup(bulletSpeed, maxRange);
    }

    IEnumerator ReloadRoutine()
    {
        
        isReloading = true;
        Debug.Log("Reloading...");
        OnAmmoChanged?.Invoke(); // Notify UI of ammo change

        // Optional: Change hand color or play animation here
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magSize;
        isReloading = false;
        GetComponent<SpriteRenderer>().color = Color.white;
        OnAmmoChanged?.Invoke(); // Notify UI of ammo change
        Debug.Log("Reload Complete!");
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