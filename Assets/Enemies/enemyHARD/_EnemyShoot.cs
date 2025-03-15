using System.Collections;
using UnityEngine;

public class RangedEnemyShooter : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField]
    public GameObject projectilePrefab;

    [SerializeField]
    public Transform firePoint;

    public float attackRange = 7f; // Only shoots if player is within this range
    public float shootCooldown = 1.5f;
    public int projectileCount = 5;
    public float spreadAngle = 60f;
    public float projectileSpeed = 10f;

    private Transform player;
    private bool canShoot = true;
    private Animator animator;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && canShoot)
        {
            StartCoroutine(Shoot());
        }
    }

    private IEnumerator Shoot()
    {
        canShoot = false;
        animator.SetBool("IsAttacking", true);

        yield return new WaitForSeconds(0.3f); // Sync with animation

        float angleStep = spreadAngle / (projectileCount - 1);
        float startAngle = -spreadAngle / 2f;
        Vector2 directionToPlayer = (player.position - firePoint.position).normalized;

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = startAngle + (angleStep * i);
            Vector2 spreadDirection = Quaternion.AngleAxis(angle, Vector3.forward) * directionToPlayer;

            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.linearVelocity = spreadDirection * projectileSpeed;
            }
        }

        animator.SetBool("IsAttacking", false);
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }
}
