using Unity.Cinemachine;
using UnityEngine;

public class HitboxScript : MonoBehaviour
{
    [Header("Camera Shake Settings")]
    [SerializeField] private float shakeIntensity = 0f;
    [SerializeField] private float shakeDuration = 0f;

    [Header("Orpheus Lifesteal Settings")]
    [SerializeField] private float lifestealPercentage = 0.1f; // 10% Lifesteal

    [Header("Attack Properties")]
    public int attackDamage = 10;
    public float knockbackForce = 5f;
    private CinemachineImpulseSource impulseSource;
    private DamageScript playerDamageScript;

    private void Start()
    {
        impulseSource = FindAnyObjectByType<CinemachineImpulseSource>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerDamageScript = player.GetComponent<DamageScript>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Early exit if not an enemy
        if (!collision.TryGetComponent<EnemyDamageScript>(out var damageable))
            return;

        // Safe camera shake
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.ShakeCamera(shakeIntensity, shakeDuration);
        }

        // Apply damage and knockback
        damageable.Hit(attackDamage, transform.position, knockbackForce);

        // Safe lifesteal application
        if (playerDamageScript != null)
        {
            int healAmount = Mathf.RoundToInt(attackDamage * lifestealPercentage);
            playerDamageScript.PlayerHeal(healAmount);
        }
    }
}