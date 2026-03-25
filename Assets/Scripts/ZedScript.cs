using UnityEngine;
using System.Collections;
using UnityEngine.Events; // Useful for triggering death effects

public class ZedScript : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private float moveSpeed = 2f;


    private float currentSpeed;
    private UnityEngine.AI.NavMeshAgent agent;
    private Transform player;
    private float nextAttackTime;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Required for 2D: Prevents the agent from trying to rotate in 3D
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        if (player != null)
        {
            // Pathfind to player position
            agent.SetDestination(player.position);
        }

        // Facing where it moves
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            float angle = Mathf.Atan2(agent.velocity.y, agent.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
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
