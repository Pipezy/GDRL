using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private int health = 3;
    [SerializeField] private int contactDamage = 1;
    [SerializeField] private GameObject powerupPrefab;
    
    // For shooter type
    [SerializeField] private bool isShooter = false;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float shootRange = 8f;
    [SerializeField] private float fireRate = 2f;
    
    private Transform player;
    private Rigidbody rb;
    private float nextFireTime = 0f;
    
    // DOT (Damage Over Time)
    private bool isBurning = false;
    private float dotDamage = 0f;
    private float dotEndTime = 0f;
    private float lastDotTick = 0f;
    
    public System.Action OnDeath;
    
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            
            if (isShooter)
            {
                // Shooter behavior - keep distance and shoot
                if (distanceToPlayer > shootRange)
                {
                    MoveTowardsPlayer();
                }
                else if (distanceToPlayer < shootRange - 2f)
                {
                    MoveAwayFromPlayer();
                }
                else
                {
                    rb.linearVelocity = Vector3.zero;
                }
                
                // Shoot at player
                if (Time.time >= nextFireTime)
                {
                    Shoot();
                    nextFireTime = Time.time + fireRate;
                }
                
                FacePlayer();
            }
            else
            {
                // Basic chaser behavior
                MoveTowardsPlayer();
                FacePlayer();
            }
        }
        
        // Handle DOT
        if (isBurning && Time.time < dotEndTime)
        {
            // Apply DOT damage every second
            if (Time.time >= lastDotTick + 1f)
            {
                TakeDamage(Mathf.RoundToInt(dotDamage));
                lastDotTick = Time.time;
            }
        }
        else if (isBurning && Time.time >= dotEndTime)
        {
            isBurning = false;
        }
    }
    
    void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        rb.linearVelocity = new Vector3(direction.x * moveSpeed, 0, direction.z * moveSpeed);
    }
    
    void MoveAwayFromPlayer()
    {
        Vector3 direction = (transform.position - player.position).normalized;
        direction.y = 0;
        rb.linearVelocity = new Vector3(direction.x * moveSpeed, 0, direction.z * moveSpeed);
    }
    
    void FacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
    
    void Shoot()
    {
        if (projectilePrefab != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position + transform.forward * 0.5f, transform.rotation);
            Rigidbody projRb = projectile.GetComponent<Rigidbody>();
            projRb.linearVelocity = transform.forward * 10f;
            Destroy(projectile, 5f);
        }
    }
    
    void OnCollisionStay(Collision collision)
    {
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(contactDamage);
        }
    }
    
    public void TakeDamage(int damage)
    {
        health -= damage;
        
        if (health <= 0)
        {
            // 40% chance to drop powerup
            if (powerupPrefab != null && Random.value < 0.4f)
            {
                Instantiate(powerupPrefab, transform.position, Quaternion.identity);
            }
            
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }
    
    public void ApplyDOT(float damage, float duration)
    {
        isBurning = true;
        dotDamage = damage;
        dotEndTime = Time.time + duration;
        lastDotTick = Time.time;
    }
    
    public void ScaleStats(int wave)
    {
        // Increase health by 10% per wave
        health += Mathf.RoundToInt(health * 0.1f * (wave - 1));
        
        // Increase speed by 5% per wave (capped)
        moveSpeed += moveSpeed * 0.05f * (wave - 1);
        moveSpeed = Mathf.Min(moveSpeed, 8f);
    }
}