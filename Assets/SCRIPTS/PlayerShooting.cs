using UnityEngine;
using UnityEngine.InputSystem;
 
public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private ElementData currentElement;
    [SerializeField] private Transform   firePoint;
 
    // Stream references — populated automatically when you equip a stream element
    private FireStream   activeStream;
    private float        nextFireTime = 0f;
 
    // ─── Update ───────────────────────────────────────────────────────────────────
 
    void Update()
    {
        if (Time.timeScale == 0) return;
        if (currentElement == null) return;
 
        if (currentElement.isStream)
        {
            HandleStream();
        }
        else
        {
            HandleProjectile();
        }
    }
 
    // ─── Projectile (unchanged behaviour) ────────────────────────────────────────
 
    void HandleProjectile()
    {
        // Stop any leftover stream if we switched elements
        StopStream();
 
        if (Mouse.current.leftButton.isPressed && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + currentElement.fireRate;
        }
    }
 
    void Shoot()
    {
        if (currentElement.projectilePrefab == null) return;
 
        GameObject projectile = Instantiate(
            currentElement.projectilePrefab,
            firePoint.position,
            firePoint.rotation);
 
        projectile.transform.localScale = Vector3.one * currentElement.projectileSize;
 
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = firePoint.forward * currentElement.projectileSpeed;
 
        ElementalProjectile projScript = projectile.GetComponent<ElementalProjectile>();
        if (projScript != null)
            projScript.Initialize(currentElement);
 
        Destroy(projectile, 5f);
    }
 
    // ─── Stream ───────────────────────────────────────────────────────────────────
 
    void HandleStream()
    {
        // Spawn the stream handler if it doesn't exist yet
        if (activeStream == null)
            SpawnStream();
 
        if (Mouse.current.leftButton.isPressed)
            activeStream.StartFiring();
        else
            activeStream.StopFiring();
    }
 
    void SpawnStream()
    {
        // Create a child GameObject to own the stream logic
        GameObject streamGO = new GameObject("FireStream");
        streamGO.transform.SetParent(firePoint);
        streamGO.transform.localPosition = Vector3.zero;
        streamGO.transform.localRotation = Quaternion.identity;
 
        activeStream = streamGO.AddComponent<FireStream>();
        activeStream.Initialise(currentElement, firePoint);
    }
 
    void StopStream()
    {
        if (activeStream != null)
        {
            activeStream.StopFiring();
            Destroy(activeStream.gameObject);
            activeStream = null;
        }
    }
 
    // ─── Element swap ─────────────────────────────────────────────────────────────
 
    public void EquipElement(ElementData element)
    {
        // Clean up stream if we were using one
        StopStream();
 
        currentElement = element;
        Debug.Log("Equipped element: " + element.elementName);
    }
}