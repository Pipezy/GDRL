using UnityEngine;

public class Powerup : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Pause the game
            Time.timeScale = 0f;
            
            // Show upgrade UI (we'll make this next)
            UpgradeManager.Instance.ShowUpgradeOptions();
            
            // Destroy the powerup
            Destroy(gameObject);
        }
    }
}