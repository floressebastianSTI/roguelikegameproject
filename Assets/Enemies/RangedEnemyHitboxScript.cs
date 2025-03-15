using UnityEngine;

public class RangedEnemyHitboxScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private int attackDamage = 0;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            DamageScript damageable = other.GetComponent<DamageScript>();
            if (damageable != null)
            {
                Vector2 attackerPosition = transform.position;
                damageable.Hit(attackDamage, attackerPosition);
            }

            Destroy(gameObject);
        }
    }
}
