using UnityEngine;

public class EnemyAIScript : MonoBehaviour
{
    public float moveSpeed = 2f; // Speed at which the enemy moves
    public float stoppingDistance = 1f; // Distance at which the enemy stops to attack
    public float avoidanceRadius = 1.5f; // Radius to avoid other enemies
    public float avoidanceForce = 5f; // Force applied to avoid other enemies

    private Transform player; // Reference to the player's transform
    private Rigidbody2D rb; // Reference to the enemy's Rigidbody2D
    private SpriteRenderer spriteRenderer; // Reference to the enemy's SpriteRenderer

    void Start()
    {
        // Find the player by tag (make sure your player GameObject is tagged as "Player")
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
    }

    void Update()
    {
        MoveTowardsPlayer();
        AvoidOtherEnemies();
        FlipSpriteBasedOnPlayerPosition(); // Flip the sprite based on player position
    }

    void MoveTowardsPlayer()
    {
        // Calculate the direction to the player
        Vector2 direction = (player.position - transform.position).normalized;

        // Calculate the distance to the player
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // If the enemy is not close enough to attack, move towards the player
        if (distanceToPlayer > stoppingDistance)
        {
            rb.linearVelocity = direction * moveSpeed;
        }
        else
        {
            // Stop moving when close enough to attack
            rb.linearVelocity = Vector2.zero;
            // Here you can trigger an attack animation or logic
        }
    }

    void AvoidOtherEnemies()
    {
        // Find all other enemies in the scene
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            if (enemy != gameObject) // Skip itself
            {
                // Calculate the distance to the other enemy
                float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);

                // If the enemy is too close, move away
                if (distanceToEnemy < avoidanceRadius)
                {
                    Vector2 avoidanceDirection = (transform.position - enemy.transform.position).normalized;
                    rb.linearVelocity += avoidanceDirection * avoidanceForce * Time.deltaTime;
                }
            }
        }
    }

    void FlipSpriteBasedOnPlayerPosition()
    {
        // Flip the sprite based on the player's position
        if (player.position.x > transform.position.x)
        {
            // Player is to the right, face right
            spriteRenderer.flipX = false;
        }
        else if (player.position.x < transform.position.x)
        {
            // Player is to the left, face left
            spriteRenderer.flipX = true;
        }
    }
}