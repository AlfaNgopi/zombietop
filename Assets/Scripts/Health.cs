using UnityEngine;
using UnityEngine.Events; // Useful for triggering death effects

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    public bool isPlayer = false; // Check this box on the Player object only

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        float slowAmount = maxHealth > 0 ? amount / maxHealth : 0; // Avoid division by zero

        Debug.Log(gameObject.name + " took damage! Current HP: " + currentHealth);

        // Check for Player Slow
        if (TryGetComponent<PlayerScript>(out PlayerScript pm))
        {
            pm.ApplySlow(slowAmount, 0.2f); 
        }

        // Check for Enemy Slow
        if (TryGetComponent<ZedScript>(out ZedScript ai))
        {
            ai.ApplySlow(slowAmount, 0.3f); 
        }


        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isPlayer)
        {
            Debug.Log("GAME OVER");
            // You could reload the scene here: 
            // UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
        else
        {
            Debug.Log(gameObject.name + " destroyed!");
            Destroy(gameObject);
        }
    }
}