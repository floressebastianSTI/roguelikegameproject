using UnityEngine;

public class RoarAttackScript : MonoBehaviour
{
    [Header("Roar Attack Settings")]
    [SerializeField] private float screenShakeIntensity = 3f; // Intensity of the screen shake
    [SerializeField] private float screenShakeDuration = 0.5f; // Duration of the screen shake
    [SerializeField] private Transform playerTransform; // Reference to the player's transform
    private bool isFacingRight; // Tracks the player's facing direction

    void Start()
    {
        isFacingRight = playerTransform.localScale.y > 0;

        // Flip the roar attack based on the player's facing direction
        FlipRoarAttack();
    }

    void OnEnable()
    {
        // Trigger screen shake when the roar attack is activated
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.ShakeCamera(screenShakeIntensity, screenShakeDuration);
        }
        else
        {
            Debug.LogWarning("CameraShake instance not found!");
        }
    }

    private void FlipRoarAttack()
    {
        // Flip the roar attack's direction based on the player's facing direction
        Vector3 scale = transform.localScale;
        scale.x = isFacingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
}