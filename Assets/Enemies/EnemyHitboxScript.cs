using UnityEngine;

public class EnemyHitboxScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Collider2D detectionCollider;  // Detects player to trigger attack
    [SerializeField] private Collider2D attackHitboxCollider; // Deals damage when attacking
    [SerializeField] private int attackDamage = 10;

    private Animator animator;
    public bool playerInRange = false;

    private void Start()
    {
        animator = GetComponentInParent<Animator>(); // Ensure Animator is from the enemy parent
        attackHitboxCollider.enabled = false; // Attack hitbox should be disabled until attacking
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player entered the detection area
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            playerInRange = true;
            animator.SetBool("IsAttacking", true);
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        // Check if the player left the detection area
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            playerInRange = false;
            animator.SetBool("IsAttacking", false);
        }
    }

    // Called as an animation event to enable attack hitbox at the right time
    public void EnableAttackHitbox()
    {
        attackHitboxCollider.enabled = true;
    }

    // Called as an animation event to disable attack hitbox after attack is done
    public void DisableAttackHitbox()
    {
        attackHitboxCollider.enabled = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Apply damage when the attack hitbox is enabled
        if (attackHitboxCollider.enabled && ((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            DamageScript damageable = other.GetComponent<DamageScript>();
            if (damageable != null)
            {
                print("DAMAGED!");
                Vector2 attackerPosition = transform.position;
                damageable.Hit(attackDamage, attackerPosition);
            }
        }
    }


}
