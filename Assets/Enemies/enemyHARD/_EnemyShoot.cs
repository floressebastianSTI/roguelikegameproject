using System.Collections;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{

    public GameObject projectilePrefab;
    public Transform firePoint;
    public float attackCooldown = 2f;
    public int projectileCount = 7;
    public float spreadAngle = 60f;
    public float projectileSpeed = 5f;

    public Animator animator;
    private float attackTimer;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            StartCoroutine(AttackRoutine());
            attackTimer = attackCooldown;
        }
    }

    private IEnumerator AttackRoutine()
    {
        animator.SetBool("IsAttacking", true);
        yield return new WaitForSeconds(0.2f); // Small delay for animation sync
        ShootVolley();
        yield return new WaitForSeconds(0.2f); // Allow some time before stopping animation
        animator.SetBool("IsAttacking", false);
    }

    private void ShootVolley()
    {
        float angleStep = spreadAngle / (projectileCount - 1);
        float startAngle = -spreadAngle / 2f;

        for (int i = 0; i < projectileCount; i++)
        {
            float angle = startAngle + (angleStep * i);
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector2 direction = rotation * Vector2.right;

            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, rotation);
            Rigidbody2D rbProjectile = projectile.GetComponent<Rigidbody2D>();
            if (rbProjectile != null)
            {
                rbProjectile.linearVelocity = direction * projectileSpeed;
            }
        }
    }
}
