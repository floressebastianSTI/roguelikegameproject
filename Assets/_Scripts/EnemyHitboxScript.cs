using UnityEngine;

public class EnemyHitboxScript : MonoBehaviour
{
    [SerializeField]
    public LayerMask playerLayer;

    [SerializeField]
    public int attackDamage = 10;

    private void OnTriggerEnter2D(Collider2D enemy)
    {
        if (((1 << enemy.gameObject.layer) & playerLayer) != 0)
        {
            DamageScript damageable = enemy.GetComponent<DamageScript>();

            if (damageable != null)
            {
                print("DAMAGED!");
                Vector2 attackerPosition = transform.position;
                damageable.Hit(attackDamage, attackerPosition);
            }
        }
    }
}

