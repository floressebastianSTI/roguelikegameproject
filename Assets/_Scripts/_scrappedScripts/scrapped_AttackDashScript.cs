using UnityEngine;
using UnityEngine.InputSystem;

public class AttackDash : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float[] dashDistances = new float[3]; // Dash distance for each attack in the combo
    [SerializeField] private float[] dashDurations = new float[3]; // Dash duration for each attack in the combo
    [SerializeField] private AnimationCurve dashCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1)); // Controls dash speed over time
    [SerializeField] private GameObject[] attackDashVFX;

    [Header("Script References")]
    [SerializeField] public AttackScript attackScript;
    [SerializeField] public PlayerMovement playerController;

    private bool isDashing = false;
    private float dashTimer = 0f;
    private Vector3 dashStartPosition;
    private Vector3 dashTargetPosition;

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        attackScript = GetComponentInChildren<AttackScript>();
        playerController = GetComponent<PlayerMovement>();

        if (attackScript == null)
        {
            Debug.LogError("AttackScript not found on the same GameObject!");
        }
        if (playerController == null)
        {
            Debug.LogError("PlayerController not found on the same GameObject!");
        }
    }

    void Update()
    {
        if (isDashing)
        {
            PerformDash();
        }
        else if (attackScript != null && Mouse.current.leftButton.wasPressedThisFrame && attackScript.canAttack)
        {
            StartDash();
        }
    }

    private void StartDash()
    {
        // Get the current attack index from the AttackScript
        int attackIndex = attackScript.GetCurrentAttackIndex();

        // Get mouse position in world coordinates
        Vector3 mousePosition = mainCam.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, mainCam.nearClipPlane));
        mousePosition.z = 0; // Ensure Z is 0 for 2D

        // Calculate dash direction and target position
        Vector3 dashDirection = (mousePosition - transform.position).normalized;
        dashTargetPosition = transform.position + dashDirection * dashDistances[attackIndex];

        // Initialize dash variables
        dashStartPosition = transform.position;
        isDashing = true;
        dashTimer = 0f;

        // Spawn the attack dash VFX
        if (attackDashVFX != null && attackIndex < attackDashVFX.Length && attackDashVFX[attackIndex] != null)
        {
            GameObject vfxInstance = Instantiate(attackDashVFX[attackIndex], transform.position, Quaternion.identity);

            // Calculate the angle for the VFX rotation
            float angle = Mathf.Atan2(dashDirection.y, dashDirection.x) * Mathf.Rad2Deg;

            // Flip the VFX if dashing left
            if (dashDirection.x < 0)
            {
                // Flip the VFX horizontally by scaling the X axis negatively
                vfxInstance.transform.localScale = new Vector3(-1, 1, 1);
                // Adjust the angle to prevent the VFX from being upside down
                angle += 180f;
            }

            // Apply the rotation to the VFX
            vfxInstance.transform.rotation = Quaternion.Euler(0, 0, angle);

            Destroy(vfxInstance, 1f); // Adjust the lifetime as needed
        }

        // Notify the PlayerController that an attack dash is happening
        playerController.isAttackDashing = true;
    }

    private void PerformDash()
    {
        // Get the current attack index from the AttackScript
        int attackIndex = attackScript.GetCurrentAttackIndex();

        if (dashTimer < dashDurations[attackIndex])
        {
            // Increment the timer
            dashTimer += Time.deltaTime;

            // Calculate the interpolation factor using the animation curve
            float t = dashTimer / dashDurations[attackIndex];
            float curveValue = dashCurve.Evaluate(t);

            // Move the character towards the target position
            transform.position = Vector3.Lerp(dashStartPosition, dashTargetPosition, curveValue);
        }
        else
        {
            // End the dash
            isDashing = false;
            transform.position = dashTargetPosition; // Snap to the final position

            // Notify the PlayerController that the attack dash is finished
            playerController.isAttackDashing = false;
        }
    }
}