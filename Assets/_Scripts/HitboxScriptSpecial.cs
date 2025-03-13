using UnityEngine;
using Unity.Cinemachine;

public class HitboxScriptSpecial : MonoBehaviour
{
    public int attackDamage = 10;
    public float knockbackForce = 5f;
    public int attackIndex; // This will be set when the attack is instantiated
    private CinemachineImpulseSource impulseSource;

    private void Start()
    {
        // Find Impulse Source in the Player (assumes Player has this component)
        impulseSource = FindAnyObjectByType<CinemachineImpulseSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageScript damageable = collision.GetComponent<DamageScript>();

        if (damageable != null)
        {
            Vector2 attackerPosition = transform.position;
            damageable.Hit(attackDamage, attackerPosition);

            // Apply hitstop only on the third hit (index 2)
            if (attackIndex == 2)
            {
                HitstopManager.Stop(0.15f, 0f); // Apply hitstop for impact

                // Trigger Camera Shake
                if (impulseSource != null)
                {
                    impulseSource.GenerateImpulseWithForce(2.0f); // Adjust intensity
                }
            }
        }
    }
}
