/*
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float timeBetweenSpawns;
    float nextSpawnTime;

    [Header("Spawned Enemy")]
    public GameObject enemy;

    public Transform[] spawnPoints;

    public int maxEnemies; 
    private int currentEnemyCount = 0; 

    void Start()
    {
        
    }

    void Update()
    {
        if(Time.time > nextSpawnTime && currentEnemyCount < maxEnemies)
        {
            nextSpawnTime = Time.time + timeBetweenSpawns;
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(enemy, randomSpawnPoint.position, Quaternion.identity); 
            currentEnemyCount++;
        }
    }
}
*/

using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    [Header("Wave Settings")]
    public Wave[] waves;
    public Transform[] spawnPoints;

    private int currentWaveIndex = 0;
    private int enemiesKilledThisWave = 0;
    private List<GameObject> currentEnemies = new List<GameObject>();
    private float nextSpawnTime;
    private bool waveComplete = false;

    void Start()
    {
        StartNextWave();
    }

    void Update()
    {
        if (currentWaveIndex >= waves.Length) return;

        // Spawn enemies if wave isn't complete
        if (!waveComplete && Time.time >= nextSpawnTime &&
            enemiesKilledThisWave + currentEnemies.Count < waves[currentWaveIndex].TotalEnemiesToKill)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + waves[currentWaveIndex].spawnRate;
        }

        // Check if wave is complete (all enemies spawned AND all killed)
        if (!waveComplete &&
            enemiesKilledThisWave >= waves[currentWaveIndex].TotalEnemiesToKill &&
            currentEnemies.Count == 0)
        {
            waveComplete = true;
            CompleteWave();
        }
    }

    void StartNextWave()
    {
        if (currentWaveIndex >= waves.Length) return;

        enemiesKilledThisWave = 0;
        waveComplete = false;
        Debug.Log($"Starting Wave {currentWaveIndex + 1}");
    }

    void SpawnEnemy()
    {
        var availableTypes = new List<EnemyWaveEntry>();
        foreach (var type in waves[currentWaveIndex].enemyTypes)
        {
            if (type.count > 0) availableTypes.Add(type);
        }
        if (availableTypes.Count == 0) return;

        var typeToSpawn = availableTypes[Random.Range(0, availableTypes.Count)];
        typeToSpawn.count--;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = Instantiate(typeToSpawn.enemyPrefab, spawnPoint.position, Quaternion.identity);
        currentEnemies.Add(enemy);

        var watcher = enemy.AddComponent<EnemyDeathWatcher>();
        watcher.OnEnemyDestroyed += OnEnemyDeath;
    }

    void OnEnemyDeath(GameObject enemy)
    {
        if (currentEnemies.Contains(enemy))
        {
            currentEnemies.Remove(enemy);
            enemiesKilledThisWave++;
            Debug.Log($"Enemy killed: {enemiesKilledThisWave}/{waves[currentWaveIndex].TotalEnemiesToKill}");
        }
    }

    void CompleteWave()
    {
        Debug.Log($"Wave {currentWaveIndex + 1} complete!");
        currentWaveIndex++;

        if (currentWaveIndex < waves.Length)
        {
            Invoke("StartNextWave", waves[currentWaveIndex - 1].timeBeforeNextWave);
        }
    }
    void OnGUI()
    {
        if (currentWaveIndex < waves.Length)
        {
            GUI.Label(new Rect(10, 10, 300, 20), $"Wave: {currentWaveIndex + 1}/{waves.Length}");
            GUI.Label(new Rect(10, 30, 300, 20), $"Enemies: {enemiesKilledThisWave}/{waves[currentWaveIndex].TotalEnemiesToKill}");
            GUI.Label(new Rect(10, 50, 300, 20), $"Alive: {currentEnemies.Count}");
        }
    }
}