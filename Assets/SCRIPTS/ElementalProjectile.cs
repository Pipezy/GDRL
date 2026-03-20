using UnityEngine;

public class ElementalProjectile : MonoBehaviour
{
    private ElementData elementData;
    
    public void Initialize(ElementData data)
    {
        elementData = data;
    }
    
    void OnCollisionEnter(Collision collision)
    {
        EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            // Deal instant damage
            enemy.TakeDamage(elementData.baseDamage);
            
            // Apply DOT if element has it
            if (elementData.hasDOT)
            {
                enemy.ApplyDOT(elementData.dotDamage, elementData.dotDuration);
            }
            
            // Apply knockback if element has it
            if (elementData.knockbackForce > 0)
            {
                Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
                if (enemyRb != null)
                {
                    Vector3 knockbackDir = (enemy.transform.position - transform.position).normalized;
                    enemyRb.AddForce(knockbackDir * elementData.knockbackForce, ForceMode.Impulse);
                }
            }
        }
        
        Destroy(gameObject);
    }
}