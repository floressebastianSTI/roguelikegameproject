using UnityEngine;

[System.Serializable]
public class EnemyWaveEntry
{
    public GameObject enemyPrefab;
    public int count;
}

[System.Serializable]
public class Wave
{
    public EnemyWaveEntry[] enemyTypes;
    public float spawnRate = 1f;
    public float timeBeforeNextWave = 5f;

    public int TotalEnemiesToKill
    {
        get
        {
            if (enemyTypes == null) return 0;

            int total = 0;
            foreach (var entry in enemyTypes)
            {
                if (entry != null)
                {
                    total += entry.count;
                }
            }
            return total;
        }
    }
}