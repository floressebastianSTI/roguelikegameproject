using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float walkSpeed;
    public float stoppingDistance = 1f; // Distance at which the enemy stops moving

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private int currentHealth;
    private bool isKnockedBack;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!isKnockedBack && player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer > stoppingDistance)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                rb.linearVelocity = direction * walkSpeed;
            }
            else
            {
                rb.linearVelocity = Vector2.zero; // Stop movement when within stopping distance
            }

            spriteRenderer.flipX = player.position.x < transform.position.x;
        }
    }

    public void OnHit(int damage, Vector2 knockback, Vector2 hitPoint, Vector2 hitDirection)
    {
        currentHealth -= damage; // Reduce health
        //if (currentHealth <= 0)
       // {
          //  Die(hitDirection); // Call Die method when health reaches zero
       // }
       // else
        {
            StartCoroutine(KnockbackCoroutine(knockback));
        }
    }

    private IEnumerator KnockbackCoroutine(Vector2 knockback)
    {
        isKnockedBack = true;
        rb.linearVelocity = Vector2.zero; // Stop movement
        rb.AddForce(knockback, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.2f);
        isKnockedBack = false;
    }

    void DealDamage(EnemyAI enemy, int damage, Vector2 knockback, Vector2 attackPosition)
    {
        if (enemy != null)
        {
            Vector2 hitPoint = enemy.transform.position; // Approximate hit position
            Vector2 hitDirection = (enemy.transform.position - (Vector3)attackPosition).normalized; // Direction of attack

            enemy.OnHit(damage, knockback, hitPoint, hitDirection);
        }
    }
}