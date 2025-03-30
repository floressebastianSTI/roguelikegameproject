using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public SpriteRenderer ghostSprite;
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] public Material distortionMaterial;
    [SerializeField] public Transform dashEffect;
    [SerializeField] public Transform runEffect;
    [SerializeField] private float runEffectInterval = 0.2f;
    [SerializeField] private float runEffectTimer = 0f;

    [Header("Dash Settings")]
    [SerializeField] private float dashDistance = 4.3f;
    [SerializeField] public float dashCooldown = 1.5f;
    [SerializeField] private GameObject regularDashVFX;
    [SerializeField] public float dashSize = 1f; // Adjusted to a reasonable size

    [Header("Attack Dash Integration")]
    public bool isAttackDashing = false;

    private bool canDash = true;
    public float walkSpeed = 0f;
    public float runSpeed = 0f;
    private float stopRunThreshold = 0.1f;
    private float stopRunTimer = 0f;
    private float stopRunDelay = 0.2f;

    public Vector2 moveInput;
    private Vector2 pointerInput;
    private Vector3 lastMoveDir;

    public float CurrentMoveSpeed
    {
        get
        {
            if (IsMoving)
            {
                if (IsRunning)
                {
                    return runSpeed;
                }
                else
                {
                    return walkSpeed;
                }
            }
            else
            {
                return 0;
            }
        }
    }

    public bool isAlive
    {
        get
        {
            return animator.GetBool("IsAlive");
        }
    }

    Animator animator;
    public Rigidbody2D rb;

    private AfterimageScript afterimageEffect;

    [SerializeField] private InputActionReference pointerPosition;

    [SerializeField] private bool walking = false;
    [SerializeField] private bool running = false;

    public bool canMove;

    public bool CanMove
    {
        get
        {
            return animator.GetBool("canMove");
        }
    }

    public bool IsMoving
    {
        get
        {
            return walking;
        }
        set
        {
            walking = value;
            animator.SetBool("IsMoving", value);
        }
    }

    public bool IsRunning
    {
        get
        {
            return running;
        }
        set
        {
            running = value;
            animator.SetBool("IsRunning", value);
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.flipX = false;
    }

    public void FixedUpdate()
    {
        rb.linearVelocity = moveInput * CurrentMoveSpeed;

        if (isAlive)
        {
            // Get mouse position in world coordinates
            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, 0));
            mousePosition.z = 0;

            // Calculate direction from player to mouse
            Vector2 direction = (mousePosition - transform.position).normalized;

            // Update facing direction based on mouse position
            UpdateFacingDirection(direction);

            // Determine if the movement is forward or backward relative to the facing direction
            Vector2 facingDirection = GetFacingDirection();
            bool isMovingForward = Vector2.Dot(facingDirection, moveInput) > 0;

            // Apply running animations based on movement direction
            animator.SetBool("IsRunningForward", IsRunning && isMovingForward);
            animator.SetBool("IsRunningBackward", IsRunning && !isMovingForward);

            Debug.Log($"FacingDirection: {facingDirection}, MoveInput: {moveInput}, IsMovingForward: {isMovingForward}");
            Debug.Log($"IsRunningForward: {animator.GetBool("IsRunningForward")}, IsRunningBackward: {animator.GetBool("IsRunningBackward")}");
        }
        else
        {
            spriteRenderer.flipX = false;
        }

        if (IsRunning && IsMoving)
        {
            runEffectTimer += Time.fixedDeltaTime;

            if (runEffectTimer >= runEffectInterval)
            {
                Transform runEffectTransform = Instantiate(runEffect, transform.position, Quaternion.identity);

                // Flip the effect based on movement direction
                bool isMovingLeft = lastMoveDir.x < 0;
                runEffectTransform.localScale = new Vector3(isMovingLeft ? -1f : 1f, 1f, 1f);

                runEffectTimer = 0f;
            }
        }
    }
    private void UpdateFacingDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (angle < 0)
        {
            angle += 360; // Normalize angle to 0-360 degrees
        }

        // Determine the closest cardinal direction
        if (angle >= 45 && angle < 135)
        {
            // Facing up
            animator.SetInteger("FacingDirection", 1); // Up
            spriteRenderer.flipX = false; // Reset flipX for up
            Debug.Log("Facing: Up");
        }
        else if (angle >= 135 && angle < 225)
        {
            // Facing left (use flipX for left)
            animator.SetInteger("FacingDirection", 0); // Right (but flipped)
            spriteRenderer.flipX = true; // Flip the sprite for left
            Debug.Log("Facing: Left");
        }
        else if (angle >= 225 && angle < 315)
        {
            // Facing down
            animator.SetInteger("FacingDirection", 2); // Down
            spriteRenderer.flipX = false; // Reset flipX for down
            Debug.Log("Facing: Down");
        }
        else
        {
            // Facing right
            animator.SetInteger("FacingDirection", 0); // Right
            spriteRenderer.flipX = false; // Do not flip the sprite for right
            Debug.Log("Facing: Right");
        }
    }
    private Vector2 GetFacingDirection()
    {
        int facingDirection = animator.GetInteger("FacingDirection");
        switch (facingDirection)
        {
            case 0: return spriteRenderer.flipX ? Vector2.left : Vector2.right; // Left or Right
            case 1: return Vector2.up; // Up
            case 2: return Vector2.down; // Down
            default: return Vector2.right;
        }
    }

    public Vector2 GetPointerInput()
    {
        Vector3 mousePosition = pointerPosition.action.ReadValue<Vector2>();
        mousePosition.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (isAlive)
        {
            bool wasMoving = IsMoving;
            IsMoving = moveInput.sqrMagnitude > stopRunThreshold;

            if (!IsMoving && wasMoving)
            {
                stopRunTimer += Time.deltaTime;
                if (stopRunTimer >= stopRunDelay)
                {
                    IsRunning = false;
                    stopRunTimer = 0f;
                }
            }
            else
            {
                stopRunTimer = 0f;
            }
        }
        else
        {
            IsMoving = false;
            IsRunning = false;
        }

        if (moveInput.sqrMagnitude > 0.01f)
        {
            lastMoveDir = moveInput.normalized;
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsRunning = true;
            Debug.Log("IsRunning: True");
        }
        else if (context.canceled)
        {
            IsRunning = false;
            Debug.Log("IsRunning: False");
        }
    }

    public void OnUniqueSkill(InputAction.CallbackContext context)
    {
        // Check if the player is moving before allowing the dash
        if (context.performed && canDash && !isAttackDashing && moveInput.sqrMagnitude > 0.1f)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;

        // Spawn the regular dash VFX
        if (regularDashVFX != null)
        {
            GameObject vfxInstance = Instantiate(regularDashVFX, transform.position, Quaternion.identity);

            // Calculate the angle for the VFX rotation
            float angle = Mathf.Atan2(lastMoveDir.y, lastMoveDir.x) * Mathf.Rad2Deg;

            // Apply the rotation to the VFX
            vfxInstance.transform.rotation = Quaternion.Euler(0, 0, angle);

            Destroy(vfxInstance, 1f); // Adjust the lifetime as needed
        }

        // Perform the dash
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
            dashEffectTransform.localScale = new Vector3(dashSize, -dashSize, 1); // Flip vertically
        }
        else
        {
            dashEffectTransform.localScale = new Vector3(dashSize, dashSize, 1);
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

    // Add the UpdateMovementState method back
    public void UpdateMovementState()
    {
        if (moveInput != Vector2.zero)
        {
            IsMoving = true;
            animator.SetBool("IsMoving", true);
        }
        else
        {
            IsMoving = false;
            animator.SetBool("IsMoving", false);
        }
    }
}