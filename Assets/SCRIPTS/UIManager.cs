using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI waveText;
    
    void Awake()
    {
        Instance = this;
    }
    
    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }
    
    public void UpdateWaveText(int wave)
    {
        if (waveText != null)
        {
            waveText.text = "Wave " + wave;
        }
    }
}