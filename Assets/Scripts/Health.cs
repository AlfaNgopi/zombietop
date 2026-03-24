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
        Debug.Log(gameObject.name + " took damage! Current HP: " + currentHealth);

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