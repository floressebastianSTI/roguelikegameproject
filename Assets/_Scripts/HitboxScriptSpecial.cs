using UnityEngine;
using Unity.Cinemachine;

public class HitboxScriptSpecial : MonoBehaviour
{

    [Header("Camera Shake Settings")]
    [SerializeField]
    private float shakeIntensity = 0f;

    [SerializeField]
    private float shakeDuration = 0f;

    [Header("Attack Interaction Settings")]
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
            CameraShake.Instance.ShakeCamera(shakeIntensity, shakeDuration);
            Vector2 attackerPosition = transform.position;
            damageable.Hit(attackDamage, attackerPosition);

            }
        }
    }
