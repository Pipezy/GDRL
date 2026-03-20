using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Upgrade
{
    public string name;
    public UpgradeType type;
}

public enum UpgradeType
{
    Damage,
    MoveSpeed,
    FireRate,
    Health
}

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;
    
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private Button[] upgradeButtons; // Array of 3 buttons
    
    private Upgrade[] availableUpgrades = new Upgrade[]
    {
        new Upgrade { name = "Damage +20%", type = UpgradeType.Damage },
        new Upgrade { name = "Move Speed +15%", type = UpgradeType.MoveSpeed },
        new Upgrade { name = "Fire Rate +20%", type = UpgradeType.FireRate },
        new Upgrade { name = "Max Health +2", type = UpgradeType.Health }
    };
    
    void Awake()
    {
        Instance = this;
    }
    
    public void ShowUpgradeOptions()
    {
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(true);
            
            // Pick 3 random upgrades
            for (int i = 0; i < upgradeButtons.Length; i++)
            {
                Upgrade randomUpgrade = availableUpgrades[Random.Range(0, availableUpgrades.Length)];
                
                // Set button text
                upgradeButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = randomUpgrade.name;
                
                // Set up button click - we need to capture the upgrade type
                UpgradeType upgradeType = randomUpgrade.type;
                upgradeButtons[i].onClick.RemoveAllListeners();
                upgradeButtons[i].onClick.AddListener(() => ApplyUpgrade(upgradeType));
            }
        }
    }
    
    public void ApplyUpgrade(UpgradeType type)
{
    GameObject player = GameObject.FindGameObjectWithTag("Player");
    
    switch (type)
    {
        case UpgradeType.Damage:
            PlayerStats.Instance.damageMultiplier *= 1.2f;
            Debug.Log("Damage increased!");
            break;
            
        case UpgradeType.MoveSpeed:
            PlayerController controller = player.GetComponent<PlayerController>();
            controller.moveSpeed *= 1.15f;
            Debug.Log("Move speed increased!");
            break;
            
        case UpgradeType.FireRate:
            // For now, just log it - we'll handle element upgrades differently later
            Debug.Log("Fire rate upgrade - not implemented yet for elements");
            break;
            
        case UpgradeType.Health:
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            health.maxHealth += 2;
            health.currentHealth += 2;
            UIManager.Instance.UpdateHealthBar(health.currentHealth, health.maxHealth);
            Debug.Log("Max health increased!");
            break;
    }
    
    HideUpgradePanel();
}
    
    public void HideUpgradePanel()
    {
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
        }
        
        Time.timeScale = 1f;
    }
}