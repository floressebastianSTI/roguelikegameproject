using UnityEngine;

public class HitboxScript : MonoBehaviour
{
    public int attackDamage = 10;
    public float knockbackForce = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageScript damageable = collision.GetComponent<DamageScript>();

        if (damageable != null)
        {
            Vector2 attackerPosition = transform.position;
            damageable.Hit(attackDamage, attackerPosition);
        }
    }
}

