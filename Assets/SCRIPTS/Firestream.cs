using UnityEngine;
using System.Collections.Generic;
 
/// <summary>
/// Spawned dynamically by PlayerShooting when a stream element is equipped.
/// Handles the particle system, raycasting, and continuous damage.
/// You never need to manually add this — PlayerShooting does it for you.
/// </summary>
public class FireStream : MonoBehaviour
{
    private ElementData    elementData;
    private Transform      firePoint;
    private ParticleSystem particles;
    private bool           isFiring = false;
 
    // Track enemies currently being damaged so we apply DOT correctly
    private readonly Dictionary<EnemyAI, float> hitEnemies = new();
 
    // ─── Setup ────────────────────────────────────────────────────────────────────
 
    public void Initialise(ElementData data, Transform origin)
    {
        elementData = data;
        firePoint   = origin;
 
        // Spawn the particle system from the prefab assigned in the ScriptableObject
        if (data.streamParticlePrefab != null)
        {
            particles = Instantiate(data.streamParticlePrefab, transform);
            particles.transform.localPosition = Vector3.zero;
            particles.transform.localRotation = Quaternion.identity;
            particles.Stop();
        }
        else
        {
            // Fallback: build a basic particle system in code so it works
            // even if you haven't made a prefab yet
            BuildFallbackParticles(data.elementColor, data.streamRange);
        }
    }
 
    // ─── Control ──────────────────────────────────────────────────────────────────
 
    public void StartFiring()
    {
        if (isFiring) return;
        isFiring = true;
        if (particles != null && !particles.isPlaying)
            particles.Play();
    }
 
    public void StopFiring()
    {
        if (!isFiring) return;
        isFiring = false;
        if (particles != null && particles.isPlaying)
            particles.Stop();
        hitEnemies.Clear();
    }
 
    // ─── Per Frame ────────────────────────────────────────────────────────────────
 
    private void Update()
    {
        if (!isFiring) return;
 
        // Keep particles aimed the same direction as the firePoint
        if (particles != null)
            particles.transform.rotation = firePoint.rotation;
 
        DamageEnemiesInStream();
    }
 
    // ─── Damage ───────────────────────────────────────────────────────────────────
 
    private void DamageEnemiesInStream()
    {
        // SphereCastAll gives us a cone-like volume along the forward direction
        // This is more forgiving than a thin raycast for a flame stream
        float   radius    = 0.6f;
        float   range     = elementData.streamRange;
        Vector3 origin    = firePoint.position;
        Vector3 direction = firePoint.forward;
 
        RaycastHit[] hits = Physics.SphereCastAll(origin, radius, direction, range);
 
        // Track which enemies we hit this frame so we can remove stale ones
        HashSet<EnemyAI> hitThisFrame = new();
 
        foreach (var hit in hits)
        {
            EnemyAI enemy = hit.collider.GetComponent<EnemyAI>();
            if (enemy == null) continue;
 
            hitThisFrame.Add(enemy);
 
            // Continuous base damage
            enemy.TakeDamage(
                Mathf.RoundToInt(elementData.streamDamagePerSecond * Time.deltaTime));
 
            // DOT — only apply once per enemy, not every frame
            if (elementData.hasDOT && !hitEnemies.ContainsKey(enemy))
            {
                enemy.ApplyDOT(elementData.dotDamage, elementData.dotDuration);
            }
 
            // Knockback — small continuous push (good for water, optional for fire)
            if (elementData.knockbackForce > 0)
            {
                Rigidbody rb = enemy.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 pushDir = (enemy.transform.position - origin).normalized;
                    rb.AddForce(pushDir * elementData.knockbackForce * Time.deltaTime,
                                ForceMode.Force);
                }
            }
 
            // Record time we first hit this enemy
            if (!hitEnemies.ContainsKey(enemy))
                hitEnemies[enemy] = Time.time;
        }
 
        // Remove enemies no longer in the stream
        List<EnemyAI> toRemove = new();
        foreach (var kv in hitEnemies)
            if (!hitThisFrame.Contains(kv.Key)) toRemove.Add(kv.Key);
        foreach (var e in toRemove)
            hitEnemies.Remove(e);
    }
 
    // ─── Fallback Particle Builder ────────────────────────────────────────────────
    // This runs if you haven't assigned a particle prefab yet.
    // It gives you a visible stream so you can test immediately.
    // Replace it with your own particle prefab when you're ready.
 
    private void BuildFallbackParticles(Color col, float range)
    {
        GameObject go = new GameObject("StreamParticles");
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
 
        particles = go.AddComponent<ParticleSystem>();
 
        // Main module
        var main          = particles.main;
        main.loop         = true;
        main.startLifetime = range / 12f;   // lifetime so particles reach ~range
        main.startSpeed   = 12f;
        main.startSize    = new ParticleSystem.MinMaxCurve(0.15f, 0.35f);
        main.startColor   = new ParticleSystem.MinMaxGradient(
                                col,
                                new Color(col.r, col.g * 0.5f, 0f, 0.6f));
        main.simulationSpace = ParticleSystemSimulationSpace.World;
 
        // Emission
        var emission      = particles.emission;
        emission.rateOverTime = 60f;
 
        // Shape — cone pointing forward
        var shape         = particles.shape;
        shape.shapeType   = ParticleSystemShapeType.Cone;
        shape.angle       = 8f;
        shape.radius      = 0.1f;
 
        // Colour over lifetime — fades out at the end
        var col2          = particles.colorOverLifetime;
        col2.enabled      = true;
        Gradient grad     = new Gradient();
        grad.SetKeys(
            new[] { new GradientColorKey(col, 0f), new GradientColorKey(Color.black, 1f) },
            new[] { new GradientAlphaKey(1f, 0f),  new GradientAlphaKey(0f, 1f) });
        col2.color        = grad;
 
        // Size over lifetime — shrinks toward the end
        var size          = particles.sizeOverLifetime;
        size.enabled      = true;
        AnimationCurve curve = new AnimationCurve();
        curve.AddKey(0f, 1f);
        curve.AddKey(1f, 0.2f);
        size.size         = new ParticleSystem.MinMaxCurve(1f, curve);
 
        particles.Stop();
    }
}