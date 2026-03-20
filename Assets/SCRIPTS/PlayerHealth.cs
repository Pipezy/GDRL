using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] public int maxHealth = 10;
    [SerializeField] private float damageCooldown = 1f; // Time between damage instances
    
    public int currentHealth;
    private float nextDamageTime = 0f;
    
    void Start()
    {
        currentHealth = maxHealth;
        UIManager.Instance.UpdateHealthBar(currentHealth, maxHealth);
    }
    
    public void TakeDamage(int damage)
    {
        if (Time.time >= nextDamageTime)
        {
            currentHealth -= damage;
            nextDamageTime = Time.time + damageCooldown;
            
            // Update UI
            UIManager.Instance.UpdateHealthBar(currentHealth, maxHealth);
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }
    
    void Die()
    {
        Debug.Log("Player Died!");
        Destroy(gameObject);
    }
}