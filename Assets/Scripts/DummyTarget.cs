using UnityEngine;

public class DummyTarget : MonoBehaviour, IDamageable
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
        Debug.Log($"{name} took {amount} damage. HP: {currentHealth:F0}", this);

        if (currentHealth <= 0f)
        {
            Debug.Log($"{name} destroyed!");
            Destroy(gameObject);
        }
    }
}