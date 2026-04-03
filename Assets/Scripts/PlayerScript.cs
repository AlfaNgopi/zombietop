using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Events;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;


    private float currentSpeed;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private Camera mainCamera;

    private WeaponScript weapon; // Drag your Hand/Gun here

    void Start()
    {
        weapon = GetComponentInChildren<WeaponScript>(); // Automatically find the weapon script in children

        mainCamera = Camera.main;

        rb = GetComponent<Rigidbody2D>();
        currentSpeed = moveSpeed;
    }

    // This method is called automatically by the Player Input component
    // (Behavior: Send Messages) because the Action is named "Move"
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnAttack(InputValue value)
    {
        
        if (weapon != null)
        {
            weapon.Shoot();
        }
    }

    void OnReload(InputValue value)
    {
        Debug.Log("Reload button pressed");
        if (weapon != null)
        {
            weapon.Reload();
        }
    }

    // This is the function the Health script will call
    public void ApplySlow(float multiplier, float duration)
    {
        Debug.Log(gameObject.name + " is slowed! Multiplier: " + multiplier);
        StopAllCoroutines(); // Reset if hit again quickly
        StartCoroutine(SlowRoutine(multiplier, duration));
    }

    IEnumerator SlowRoutine(float multiplier, float duration)
    {
        currentSpeed = moveSpeed * multiplier; // e.g., 5 * 0.5 = 2.5
        yield return new WaitForSeconds(duration);
        currentSpeed = moveSpeed;
    }


    void Update()
    {
        // Optional 1: Rotate player to face movement direction
        // if (moveInput.sqrMagnitude > 0.1f)
        // {
        //     float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
        //     transform.rotation = Quaternion.Euler(0, 0, angle);
        // }

        // Optional 2: Rotate player to mouse direction
        if (Mouse.current != null)
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, mainCamera.nearClipPlane));
            Vector2 direction = (mouseWorldPos - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

    }

    void FixedUpdate()
    {
        // Apply velocity to the Rigidbody
        rb.linearVelocity = moveInput * currentSpeed;
    }
}