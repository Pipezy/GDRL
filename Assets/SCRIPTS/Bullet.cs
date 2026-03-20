using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private int baseDamage = 1;
    
    void OnCollisionEnter(Collision collision)
    {
        EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            // Calculate damage with player's multiplier
            int totalDamage = Mathf.RoundToInt(baseDamage * PlayerStats.Instance.damageMultiplier);
            enemy.TakeDamage(totalDamage);
        }
        
        Destroy(gameObject);
    }
}