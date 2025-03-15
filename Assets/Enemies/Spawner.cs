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
