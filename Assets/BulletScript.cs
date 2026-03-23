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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Add logic here to damage enemies
        if (collision.CompareTag("Wall")) // Make sure your walls have the "Wall" tag
        {
            Destroy(gameObject);
        }
    }
}