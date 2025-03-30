using System.Collections;
using UnityEngine;

public class LaserAttack : MonoBehaviour
{
    public GameObject explosionPrefab; // Explosion effect prefab
    public Transform[] explosionPoints; // Assign empty GameObjects as explosion points
    public Vector3[] explosionScales; // Unique scale for each explosion
    public float explosionDelay = 0.2f; // Delay between each explosion

    private Transform player;
    private SpriteRenderer spriteRenderer;
    private bool isFlipped = false;

    private void Start()
    {
        // Find player at runtime
        player = GameObject.FindWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("[LaserAttack] Player not found! Make sure the player has the correct tag.");
            return;
        }

        // Get the SpriteRenderer (avoid scaling issues)
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("[LaserAttack] No SpriteRenderer found! Make sure your laser has one.");
            return;
        }

        // Flip the laser and explosion points correctly
        FlipLaser();

        // Start explosion sequence
        StartCoroutine(TriggerExplosions());
    }

    private void FlipLaser()
    {
        // Determine if the laser should flip
        isFlipped = player.position.x < transform.position.x;
        spriteRenderer.flipX = isFlipped;

        // Flip explosion points correctly (mirror them around the laser)
        for (int i = 0; i < explosionPoints.Length; i++)
        {
            Vector3 localPos = explosionPoints[i].localPosition;

            if (isFlipped)
            {
                // Move explosion to the opposite side while keeping its relative distance
                localPos.x = -Mathf.Abs(localPos.x);
            }
            else
            {
                localPos.x = Mathf.Abs(localPos.x);
            }

            explosionPoints[i].localPosition = localPos;
        }
    }

    private IEnumerator TriggerExplosions()
    {
        if (explosionPoints == null || explosionPoints.Length == 0)
        {
            Debug.LogError("[LaserAttack] No explosion points assigned!");
            yield break;
        }

        for (int i = 0; i < explosionPoints.Length; i++)
        {
            if (explosionPoints[i] != null && explosionPrefab != null)
            {
                GameObject explosion = Instantiate(explosionPrefab, explosionPoints[i].position, Quaternion.identity);

                // Apply different sizes per explosion (if available)
                if (explosionScales != null && i < explosionScales.Length)
                {
                    explosion.transform.localScale = explosionScales[i];
                }

                explosion.transform.SetParent(null); // Detach from laser to prevent scaling issues

                Debug.Log($"[LaserAttack] Spawned explosion {i + 1} at {explosionPoints[i].position} with size {explosion.transform.localScale}");

                yield return new WaitForSeconds(explosionDelay); // Wait before spawning the next explosion
            }
        }
    }
}
