using UnityEngine;

public class _HitboxScript : MonoBehaviour
{
    [SerializeField]
    private Collider2D hitbox;

    [SerializeField]
    private int damage;

    [SerializeField]
    private float knockbackForce;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // Check if we hit an enemy
        {
            EnemyAIScripts enemy = other.GetComponent<EnemyAIScripts>(); // Get enemy script
            if (enemy != null)
            {
                // Calculate knockback direction
                Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;

                // Apply damage and knockback
                enemy.TakeDamage(damage, knockbackDirection * knockbackForce);

                Debug.Log("FUCK");
            }
        }
    }
}
