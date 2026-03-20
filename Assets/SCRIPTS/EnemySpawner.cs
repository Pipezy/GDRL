using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs; // Array of different enemy types
    [SerializeField] private int enemiesPerWave = 3;
    [SerializeField] private float spawnRadius = 10f;
    [SerializeField] private float timeBetweenWaves = 2f;
    
    private int currentWave = 0;
    private int enemiesAlive = 0;
    private bool isSpawningWave = false;
    
    void Start()
    {
        StartNextWave();
    }
    
    void Update()
    {
        if (enemiesAlive <= 0 && !isSpawningWave)
        {
            StartCoroutine(WaveDelay());
        }
    }
    
    IEnumerator WaveDelay()
    {
        isSpawningWave = true;
        yield return new WaitForSeconds(timeBetweenWaves);
        StartNextWave();
        isSpawningWave = false;
    }
    
    void StartNextWave()
    {
        currentWave++;
        int enemiesToSpawn = enemiesPerWave + (currentWave * 2);
        
        Debug.Log("Wave " + currentWave + " starting! Spawning " + enemiesToSpawn + " enemies");
        
        UIManager.Instance.UpdateWaveText(currentWave);
        
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            SpawnEnemy();
        }
    }
    
    void SpawnEnemy()
    {
        Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
        
        // Pick a random enemy type
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        enemiesAlive++;
        
        EnemyAI enemyAI = enemy.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.OnDeath += OnEnemyDied;

            enemyAI.ScaleStats(currentWave);
        }
    }
    
    void OnEnemyDied()
    {
        enemiesAlive--;
    }
}