using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Unity.Netcode;

public class PlayerDash : MonoBehaviour
{
    [Header("Dash Settings")]
    public float dashDistance = 3f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public Transform dashEffect; // Assign a dash effect prefab
    public float dashSizeX = 1f;
    public float dashSizeY = 1f;

    private PlayerMovement playerMovement;
    private Rigidbody2D rb;
    private bool canDash = true;
    private Vector3 lastMoveDir;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash && playerMovement.GetMovementInput().sqrMagnitude > 0.1f)
        {
            lastMoveDir = playerMovement.GetMovementInput(); // Get last movement direction
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        canDash = false; // Add this line to disable dashing during cooldown

        Vector3 dashTargetPosition = transform.position + lastMoveDir * dashDistance;
        Vector3 beforeDashPosition = transform.position;
        float dashDuration = 0.2f; // Adjust as needed
        float dashTimer = 0f;

        // Spawn the dash effect
        Transform dashEffectTransform = Instantiate(dashEffect, beforeDashPosition, Quaternion.identity);

        // Calculate the angle for the dash effect rotation
        float dashAngle = Mathf.Atan2(lastMoveDir.y, lastMoveDir.x) * Mathf.Rad2Deg;

        // Apply the rotation to the dash effect
        dashEffectTransform.eulerAngles = new Vector3(0, 0, dashAngle);

        // Flip the dash effect if dashing left
        if (lastMoveDir.x < 0)
        {
            dashEffectTransform.localScale = new Vector3(dashSizeX, -dashSizeY, 1); // Flip vertically
        }
        else
        {
            dashEffectTransform.localScale = new Vector3(dashSizeX, -dashSizeY, 1);
        }

        while (dashTimer < dashDuration)
        {
            dashTimer += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, dashTargetPosition, dashTimer / dashDuration);
            yield return null;
        }

        // Snap to the final position
        transform.position = dashTargetPosition;

        // Cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}