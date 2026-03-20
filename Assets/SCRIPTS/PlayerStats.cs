using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;
    
    public float damageMultiplier = 1f;
    
    void Awake()
    {
        Instance = this;
    }
}