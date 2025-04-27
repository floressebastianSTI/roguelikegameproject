/*
using System.Collections.Generic;
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
        if (Time.time > nextSpawnTime && currentEnemyCount < maxEnemies)
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

    private int currentWaveIndex = -1;
    private int enemiesKilledThisWave = 0;
    private int enemiesSpawnedThisWave = 0;
    private List<GameObject> currentEnemies = new List<GameObject>();
    private float nextSpawnTime;
    private bool isWaveActive = false;
    private List<EnemyWaveEntry> currentWaveEnemies = new List<EnemyWaveEntry>();

    void Start()
    {
        StartNextWave();
    }

    void Update()
    {
        if (currentWaveIndex >= waves.Length) return;

        if (isWaveActive && Time.time >= nextSpawnTime &&
            enemiesSpawnedThisWave < waves[currentWaveIndex].TotalEnemiesToKill)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + waves[currentWaveIndex].spawnRate;
        }

        if (isWaveActive &&
            enemiesKilledThisWave >= waves[currentWaveIndex].TotalEnemiesToKill &&
            currentEnemies.Count == 0)
        {
            CompleteWave();
        }
    }

    void StartNextWave()
    {
        currentWaveIndex++;
        if (currentWaveIndex >= waves.Length)
        {
            Debug.Log("All waves completed!");
            return;
        }

        // Reset counters and load current wave enemies
        enemiesKilledThisWave = 0;
        enemiesSpawnedThisWave = 0;
        currentEnemies.Clear();
        currentWaveEnemies.Clear();

        // Create a spawnable list of enemies for this wave
        foreach (var enemyType in waves[currentWaveIndex].enemyTypes)
        {
            for (int i = 0; i < enemyType.count; i++)
            {
                currentWaveEnemies.Add(new EnemyWaveEntry
                {
                    enemyPrefab = enemyType.enemyPrefab,
                    count = 1
                });
            }
        }

        // Shuffle for random spawning order
        Shuffle(currentWaveEnemies);

        isWaveActive = true;
        Debug.Log($"Starting Wave {currentWaveIndex + 1} - {waves[currentWaveIndex].TotalEnemiesToKill} enemies");
    }

    void SpawnEnemy()
    {
        if (currentWaveEnemies.Count == 0) return;

        // Get next enemy type to spawn
        var nextEnemy = currentWaveEnemies[0];
        currentWaveEnemies.RemoveAt(0);

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = Instantiate(nextEnemy.enemyPrefab, spawnPoint.position, Quaternion.identity);
        currentEnemies.Add(enemy);
        enemiesSpawnedThisWave++;
        /*
        var watcher = enemy.AddComponent<EnemyDeathWatcher>();
        watcher.OnEnemyDestroyed += OnEnemyDeath;
        */
    }

    void OnEnemyDeath(GameObject enemy)
    {
        if (currentEnemies.Contains(enemy))
        {
            currentEnemies.Remove(enemy);
            enemiesKilledThisWave++;
            Debug.Log($"Killed {enemiesKilledThisWave}/{waves[currentWaveIndex].TotalEnemiesToKill}");
        }
    }

    void CompleteWave()
    {
        isWaveActive = false;
        Debug.Log($"Wave {currentWaveIndex + 1} completed!");

        if (currentWaveIndex < waves.Length - 1)
        {
            Invoke("StartNextWave", waves[currentWaveIndex].timeBeforeNextWave);
        }
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
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