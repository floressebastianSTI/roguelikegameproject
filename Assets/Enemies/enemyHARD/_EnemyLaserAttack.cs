using System.Collections;
using UnityEngine;

public class EnemyLaserAttack : MonoBehaviour
{
    public GameObject laserPrefab;      // The laser visual effect prefab
    public GameObject explosionPrefab;  // Explosion effect prefab
    public Transform firePoint;         // The point where the laser originates
    public float laserLength = 5f;      // How far the laser reaches
    public float explosionDelay = 0.3f; // Time between explosions

    private Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public void FireLaser()
    {
        if (player == null) return;

        // Determine attack direction
        float direction = (player.position.x > transform.position.x) ? 1f : -1f;

        // Spawn the laser
        GameObject laser = Instantiate(laserPrefab, firePoint.position, Quaternion.identity);
        laser.transform.localScale = new Vector3(laserLength * direction, 1f, 1f); // Adjust length & direction

        // Start spawning explosions
        StartCoroutine(SpawnExplosions(firePoint.position, direction));

        // Destroy laser after some time
        Destroy(laser, 0.5f);
    }

    private IEnumerator SpawnExplosions(Vector3 startPos, float direction)
    {
        for (int i = 0; i < 3; i++)
        {
            Vector3 explosionPos = startPos + new Vector3((laserLength / 3) * i * direction, 0, 0);
            Instantiate(explosionPrefab, explosionPos, Quaternion.identity);
            yield return new WaitForSeconds(explosionDelay);
        }
    }
}
