using UnityEngine;

public class LaserSpawnPoint : MonoBehaviour
{
    public Transform laserSpawnPoint; // The empty GameObject where the laser spawns
    private Transform player;

    private void Start()
    {
        // Automatically find the player by tag
        player = GameObject.FindWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("[LaserSpawnPoint] Player not found! Make sure the player has the 'Player' tag.");
            return;
        }

        // Set the initial position
        FlipSpawnPoint();
    }

    private void Update()
    {
        if (player != null)
        {
            FlipSpawnPoint();
        }
    }

    private void FlipSpawnPoint()
    {
        if (laserSpawnPoint == null)
        {
            Debug.LogError("[LaserSpawnPoint] Missing laserSpawnPoint reference!");
            return;
        }

        Vector3 localPos = laserSpawnPoint.localPosition;

        if (player.position.x < transform.position.x)
        {
            // Flip to the left side
            localPos.x = -Mathf.Abs(localPos.x);
        }
        else
        {
            // Flip to the right side
            localPos.x = Mathf.Abs(localPos.x);
        }

        laserSpawnPoint.localPosition = localPos;
    }
}
