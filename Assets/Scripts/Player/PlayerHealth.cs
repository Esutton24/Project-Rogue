using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log($"Player took {amount} damage. HP: {currentHealth}");

        if (currentHealth <= 0f)
            Die();
    }

    void Die()
    {
        Debug.Log("Player died!");
        // TODO: reload scene, respawn, game-over UI, etc.
    }
}