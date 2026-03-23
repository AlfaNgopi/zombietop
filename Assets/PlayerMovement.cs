using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    
    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // This method is called automatically by the Player Input component
    // (Behavior: Send Messages) because the Action is named "Move"
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        // Apply velocity to the Rigidbody
        rb.linearVelocity = moveInput * moveSpeed;
    }
}