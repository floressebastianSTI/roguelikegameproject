
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class SkypiercerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashSpeed = 20f;
    public float dashDistance = 3f;
    public float dashDuration = 0.2f;
    public float successfulDashCooldown = 1.5f;
    public float failedDashCooldown = 0.75f;
    public int maxDashes = 3;

    [Header("Hit Effect")]
    public int damage = 10;
    public float stunDuration = 1f;
    public Transform dashEffect;
    public Transform attackVFX;
    public Transform additionalVFX1;
    public Transform additionalVFX2; 
    public float dashSizeX = 3f;
    public float dashSizeY = 1.5f;

    private int remainingDashes;
    private bool canDash = true;
    private Rigidbody2D rb;
    private PlayerInput playerInput;
    private PlayerMovement playerMovement;
    private bool hitEnemy = false;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
        remainingDashes = maxDashes;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash && remainingDashes > 0 && !playerMovement.blockAllInputs) // Added blockAllInputs check
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector3 direction = (mousePosition - transform.position).normalized;
            direction = new Vector3(direction.x, direction.y, 0).normalized;
            StartCoroutine(Dash(direction));
        }
    }

    private IEnumerator Dash(Vector3 direction)
    {
        canDash = false;
        bool wasRunning = playerMovement.isRunning; // Store running state
        playerMovement.isAttackDashing = true;

        Vector3 startPosition = transform.position;
        Vector3 dashTargetPosition = startPosition + direction.normalized * dashDistance; // Fixed distance
        float dashTimer = 0f;
        hitEnemy = false; // Reset hit flag for this dash

        // Spawn dash effect
        Transform dashEffectTransform = Instantiate(dashEffect, startPosition, Quaternion.identity);
        float dashAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        dashEffectTransform.eulerAngles = new Vector3(0, 0, dashAngle);

        // Spawn attack VFX in front of the character
        Vector3 attackVFXPosition = startPosition + direction * 0.5f;
        Transform attackVFXInstance = Instantiate(attackVFX, attackVFXPosition, Quaternion.Euler(0, 0, dashAngle));
        attackVFXInstance.localScale = new Vector3(dashSizeX, dashSizeY, 1); // Adjust hitbox size

        // Spawn additional VFX for visuals
        Instantiate(additionalVFX1, startPosition, Quaternion.Euler(0, 0, dashAngle));
        Instantiate(additionalVFX2, startPosition, Quaternion.Euler(0, 0, dashAngle));

        while (dashTimer < dashDuration)
        {
            dashTimer += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, dashTargetPosition, dashTimer / dashDuration);
            yield return null;
        }

        transform.position = dashTargetPosition;
        playerMovement.isAttackDashing = false;
        playerMovement.isRunning = wasRunning; // Restore running state

        // Adjust dash effect size based on distance traveled
        float actualDashDistance = Vector3.Distance(startPosition, transform.position);
        dashEffectTransform.localScale = new Vector3(dashSizeX * (actualDashDistance / dashDistance), direction.x < 0 ? -dashSizeY : dashSizeY, 1);

        if (!hitEnemy)
        {
            remainingDashes = 0;
            StartCoroutine(CooldownRoutine(failedDashCooldown));
        }
        else
        {
            remainingDashes = Mathf.Max(remainingDashes - 1, 0);
            if (remainingDashes == 0)
            {
                StartCoroutine(CooldownRoutine(successfulDashCooldown));
            }
            else
            {
                canDash = true;
            }
        }
    }

    public void SetHitEnemy()
    {
        hitEnemy = true;
    }
    private IEnumerator CooldownRoutine(float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime);
        remainingDashes = maxDashes;
        canDash = true;
    }
}