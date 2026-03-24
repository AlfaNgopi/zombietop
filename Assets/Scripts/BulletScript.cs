using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float maxRange;
    private Vector2 startPos;

    public void Setup(float speed, float range)
    {
        maxRange = range;
        startPos = transform.position;
        GetComponent<Rigidbody2D>().linearVelocity = transform.right * speed;
    }

    void Update()
    {
        // Destroy bullet if it exceeds max range
        if (Vector2.Distance(startPos, transform.position) > maxRange)
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private float damage = 25f;

private void OnTriggerEnter2D(Collider2D collision)
{
    // Try to find the Health component on whatever we hit
    Health targetHealth = collision.GetComponent<Health>();

    if (targetHealth != null)
    {
        targetHealth.TakeDamage(damage);
        Destroy(gameObject); // Destroy bullet on hit
    }
    
    if (collision.CompareTag("Wall"))
    {
        Destroy(gameObject);
    }
}
}