using System.Collections;
using UnityEngine;

public class RangedEnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float stopDistance = 5f;
    public float retreatDistance = 2f;
    public float separationDistance = 1.5f; // Minimum distance between enemies
    public float separationForce = 2f; // How strongly enemies push away from each other

    [Header("Hit Effect")]
    [SerializeField] private GameObject hitEffect;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool isKnockedBack;
    private bool isAlive = true;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        animator.SetBool("IsAlive", isAlive);
    }

    void Update()
    {
        if (player == null || !isAlive) return;

        if (!isKnockedBack)
        {
            HandleMovement();
            FlipSprite();
        }
    }

    private void HandleMovement()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        Vector2 moveDirection = (player.position - transform.position).normalized;

        if (distanceToPlayer > stopDistance)
        {
            rb.linearVelocity = moveDirection * moveSpeed;
            animator.SetBool("IsMoving", true);
        }
        else if (distanceToPlayer < retreatDistance)
        {
            rb.linearVelocity = -moveDirection * moveSpeed;
            animator.SetBool("IsMoving", true);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("IsMoving", false);
        }

        ApplySeparation();
    }

    private void ApplySeparation()
    {
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, separationDistance, LayerMask.GetMask("Enemy"));
        Vector2 separationVector = Vector2.zero;
        int count = 0;

        foreach (var enemy in nearbyEnemies)
        {
            if (enemy.gameObject != gameObject)
            {
                Vector2 pushDirection = (transform.position - enemy.transform.position).normalized;
                separationVector += pushDirection;
                count++;
            }
        }

        if (count > 0)
        {
            separationVector /= count;
            rb.linearVelocity += separationVector * separationForce * Time.deltaTime;
        }
    }

    private void FlipSprite()
    {
        spriteRenderer.flipX = player.position.x < transform.position.x;
    }

    public void OnHit(int damage, Vector2 knockback, Vector2 hitPoint, Vector2 hitDirection)
    {
        SpawnHitEffect(hitPoint, hitDirection);
        StartCoroutine(KnockbackCoroutine(knockback));
    }

    private void SpawnHitEffect(Vector2 hitPoint, Vector2 hitDirection)
    {
        if (hitEffect)
        {
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            float angle = Mathf.Atan2(hitDirection.y, hitDirection.x) * Mathf.Rad2Deg;
            effect.transform.rotation = Quaternion.Euler(0, 0, angle);
            Destroy(effect, 1.5f);
        }
    }

    private IEnumerator KnockbackCoroutine(Vector2 knockback)
    {
        isKnockedBack = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockback, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.2f);
        isKnockedBack = false;
    }

    public void DealDamage(RangedEnemyAI enemy, int damage, Vector2 knockback, Vector2 attackPosition)
    {
        if (enemy != null)
        {
            Vector2 hitPoint = enemy.transform.position;
            Vector2 hitDirection = (enemy.transform.position - (Vector3)attackPosition).normalized;
            enemy.OnHit(damage, knockback, hitPoint, hitDirection);
        }
    }
}

