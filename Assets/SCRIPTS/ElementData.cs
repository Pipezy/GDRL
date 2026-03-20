using UnityEngine;
 
[CreateAssetMenu(fileName = "NewElement", menuName = "Elements/Element Data")]
public class ElementData : ScriptableObject
{
    public string elementName;
    public ElementType type;
    
    // Shooting stats
    public float fireRate;
    public float projectileSpeed;
    public int baseDamage;
    public float projectileSize = 1f;
    
    // Special properties
    public bool hasDOT = false;
    public float dotDamage = 0f;
    public float dotDuration = 0f;
    
    public float knockbackForce = 0f;
    
    // ── Stream / Beam attack ──────────────────────────────────────────
    [Header("Stream Attack (Fire)")]
    public bool isStream = false;           // tick this on the Fire ScriptableObject
    public float streamDamagePerSecond = 15f;
    public float streamRange = 8f;
    public ParticleSystem streamParticlePrefab; // assign your particle prefab here
    
    // Visual
    public GameObject projectilePrefab;
    public Color elementColor;
}
 
public enum ElementType
{
    Fire,
    Earth,
    Air,
    Water
}