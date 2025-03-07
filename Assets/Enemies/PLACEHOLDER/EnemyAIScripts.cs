using UnityEngine;

public class EnemyAIScripts : MonoBehaviour
{
    public int health = 30;
    public float moveSpeed = 2f;
    public float knockbackRecoveryTime = 0.5f;

    private Transform player;
    private Rigidbody2D rb;
    private Vector2 knockbackVelocity;
    private bool isKnockedBack = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!isKnockedBack && player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
        }
    }

    public void TakeDamage(float damage, Vector2 knockback)
    {
        health -= (int)damage;

        // Apply knockback
        rb.linearVelocity = knockback;
        isKnockedBack = true;
        Invoke(nameof(ResetKnockback), knockbackRecoveryTime);

        // Destroy enemy if health reaches 0
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void ResetKnockback()
    {
        isKnockedBack = false;
    }
}
