using UnityEngine;
using System.Collections;
using UnityEngine.Events; // Useful for triggering death effects

public class ZedScript : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private float moveSpeed = 2f;


    [Header("Vision")]
    [SerializeField] private LayerMask obstructionMask;
    public float visionRadius = 5f;
    [Range(0, 360)] public float visionAngle = 90f;

    [Header("Logic")]
    [SerializeField] private Vector2 lastKnownPosition;
    [SerializeField] private bool hasLastKnownPosition = false;


    private float currentSpeed;
    private UnityEngine.AI.NavMeshAgent agent;
    private Transform player;
    private float nextAttackTime;
    private bool canSeePlayer;


    private SpriteRenderer spriteRenderer; // For visual feedback when hit

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Required for 2D: Prevents the agent from trying to rotate in 3D
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // set inital render to false
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
        
    }

    void Update()
    {
        if (player != null && canSeePlayer)
        {
            lastKnownPosition = player.position;
            hasLastKnownPosition = true;
            agent.SetDestination(lastKnownPosition);
        }
        else if (hasLastKnownPosition)
        {
            // We lost the player, but we know where they were!
            agent.SetDestination(lastKnownPosition);

            // Check if we arrived at the last known position
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                hasLastKnownPosition = false; // Stop searching once reached
                // You could trigger a 'Patrol' state here!
            }
        }

        // Facing where it moves
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            float angle = Mathf.Atan2(agent.velocity.y, agent.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void CheckForPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < visionRadius)
        {
            Vector2 dirToPlayer = (player.position - transform.position).normalized;
            
            // Check if player is within the enemy's forward-facing cone
            // In 2D top-down, 'up' or 'right' is often the forward face
            if (Vector2.Angle(transform.up, dirToPlayer) < visionAngle / 2)
            {
                if (!Physics2D.Raycast(transform.position, dirToPlayer, distanceToPlayer, obstructionMask))
                {
                    canSeePlayer = true;
                    return;
                }
            }
        }
        canSeePlayer = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Check if we hit the player and cooldown is over
        if (collision.gameObject.CompareTag("Player") && Time.time >= nextAttackTime)
        {
            AttackPlayer(collision.gameObject);
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    // Inside your EnemyAI script, update the AttackPlayer method:
    void AttackPlayer(GameObject playerObj)
    {
        Health playerHealth = playerObj.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
        }
        
    }

    public void ApplySlow(float multiplier, float duration)
    {
        Debug.Log(gameObject.name + " is slowed! Multiplier: " + multiplier);
        StopAllCoroutines();
        StartCoroutine(SlowRoutine(multiplier, duration));
    }

    IEnumerator SlowRoutine(float multiplier, float duration)
    {
        agent.speed = moveSpeed * multiplier;
        yield return new WaitForSeconds(duration);
        agent.speed = moveSpeed;
    }
}
