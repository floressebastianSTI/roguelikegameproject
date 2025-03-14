using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float walkSpeed;

    [SerializeField]
    public GameObject hitEffect;

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
            // Move towards the player
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * walkSpeed;

            // Flip the sprite based on the player's position
            spriteRenderer.flipX = player.position.x < transform.position.x;
        }
    }

    public void OnHit(int damage, Vector2 knockback, Vector2 hitPoint, Vector2 hitDirection)
    {
        SpawnHitEffect(hitPoint, hitDirection);
        StartCoroutine(KnockbackCoroutine(knockback));
    }

    public void SpawnHitEffect(Vector2 hitPoint, Vector2 hitDirection)
    {
        if (hitEffect)
        {
            // Instantiate hit effect at hit position
            GameObject HitEffect = Instantiate(hitEffect, transform.position, Quaternion.identity);

            // Rotate the effect to point away from the attack direction
            float angle = Mathf.Atan2(hitDirection.y, hitDirection.x) * Mathf.Rad2Deg;
            HitEffect.transform.rotation = Quaternion.Euler(0, 0, angle);

            Destroy(HitEffect, 1.5f); // Destroy after 0.5 seconds
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