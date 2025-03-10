using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float timeBetweenSpawns;
    float nextSpawnTime;

    public GameObject enemy;

    public Transform[] spawnPoints;

    public int maxEnemies; // Maximum number of enemies
    private int currentEnemyCount = 0; // Counter for spawned enemies

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
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
