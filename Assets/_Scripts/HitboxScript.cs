using Unity.Cinemachine;
using UnityEngine;

public class HitboxScript : MonoBehaviour
{
    [SerializeField] private float shakeIntensity = 0f;
    [SerializeField] private float shakeDuration = 0f;
    [SerializeField] private float lifestealPercentage = 0.1f; // 10% Lifesteal

    public int attackDamage = 10;
    public float knockbackForce = 5f;
    private CinemachineImpulseSource impulseSource;
    private DamageScript playerDamageScript; // Reference to player's damage script

    private void Start()
    {
        impulseSource = FindAnyObjectByType<CinemachineImpulseSource>();
        playerDamageScript = FindAnyObjectByType<DamageScript>(); // Get player's damage script
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyDamageScript damageable = collision.GetComponent<EnemyDamageScript>();

        if (damageable != null)
        {
            CameraShake.Instance.ShakeCamera(shakeIntensity, shakeDuration);
            Vector2 attackerPosition = transform.position;
            damageable.Hit(attackDamage, attackerPosition);

            // Apply lifesteal
            int healAmount = Mathf.RoundToInt(attackDamage * lifestealPercentage);
            playerDamageScript.PlayerHeal(healAmount);
        }
    }
}

