using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    
    void OnCollisionEnter(Collision collision)
    {
        // Check if we hit the player
        PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(damage);
        }
        
        // Destroy the projectile on any collision
        Destroy(gameObject);
    }
}