using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float walkSpeed;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

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

            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * walkSpeed;

            spriteRenderer.flipX = player.position.x < transform.position.x;
        }
    }

    public void OnHit(int damage, Vector2 knockback, Vector2 hitPoint, Vector2 hitDirection)
    {
        StartCoroutine(KnockbackCoroutine(knockback));
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