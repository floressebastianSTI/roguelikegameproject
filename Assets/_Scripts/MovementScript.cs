using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Attack Dash Integration")]
    public bool isAttackDashing = false;

    [Header("Input Control")]
    public bool blockAllInputs = false;

    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public Vector2 movementInput;
    public Vector2 GetMovementInput()
    {
        return movementInput;
    }

    public Rigidbody2D rb;
    public Animator animator;
    private Camera mainCamera;
    public SpriteRenderer spriteRenderer;

    public bool isRunning;
    public bool IsMoving;
    private Vector2 facingDirection = Vector2.right; // Default facing right
    public float deadzoneAngle = 15f; // Deadzone to prevent jittering at corners

    public bool isAlive
    {
        get
        {
            return animator.GetBool("IsAlive");
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isAttackDashing) return; // Skip movement updates during attack dash

        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 directionToMouse = (mousePosition - (Vector2)transform.position).normalized;

        // Update facing direction based on mouse position
        if (directionToMouse.magnitude > 0.1f) // Prevent small jittering when mouse is near
        {
            facingDirection = directionToMouse;
        }
        string direction = GetFacingDirection(facingDirection);

        // Determine if running opposite or forward
        bool isFacingMovement = Vector2.Dot(movementInput, facingDirection) > 0;

        // Set animations
        if (movementInput != Vector2.zero)
        {
            animator.SetBool("IsMoving", true);
            animator.SetBool("IsRunning", isRunning);
            animator.SetBool("IsOppositeRun", isRunning && !isFacingMovement);
            animator.Play((isRunning && !isFacingMovement ? "Run_Opposite_" : isRunning ? "Run_" : "Walk_") + (direction == "Left" ? "Right" : direction));
        }
        else
        {
            animator.SetBool("IsMoving", false);
            animator.Play("Idle_" + (direction == "Left" ? "Right" : direction)); // Play idle animation based on facing direction
        }

        // Flip sprite for left direction
        spriteRenderer.flipX = (direction == "Left");
    }

    void FixedUpdate()
    {
        if (isAttackDashing)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Only apply movement if we have input
        if (movementInput != Vector2.zero)
        {
            rb.linearVelocity = movementInput * (isRunning ? runSpeed : walkSpeed);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (isAlive && !isAttackDashing && !blockAllInputs)
        {
            movementInput = context.ReadValue<Vector2>().normalized;
        }
        else
        {
            movementInput = Vector2.zero;
            IsMoving = false;
            isRunning = false;
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (!blockAllInputs)
        {
            isRunning = context.ReadValue<float>() > 0;
        }
    }

    private string GetFacingDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Apply deadzone to prevent jittering at diagonal angles
        if (Mathf.Abs(angle - 45f) < deadzoneAngle ||
            Mathf.Abs(angle - 135f) < deadzoneAngle ||
            Mathf.Abs(angle + 45f) < deadzoneAngle ||
            Mathf.Abs(angle + 135f) < deadzoneAngle)
        {
            return facingDirection.y > 0 ? "Up" : "Down";
        }

        if (angle >= -135f && angle < -45f)
        {
            return "Down";
        }
        else if (angle >= 45f && angle < 135f)
        {
            return "Up";
        }
        else if (angle >= -45f && angle < 45f)
        {
            return "Right";
        }
        else
        {
            return "Left";
        }
    }

    public void UpdateMovementState()
    {
        if (movementInput != Vector2.zero)
        {
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }

    //TEST//

    public void ForceStopMovement()
    {
        movementInput = Vector2.zero;
        isRunning = false;
        rb.linearVelocity = Vector2.zero;
        UpdateMovementState();
    }

    public void ClearMovementState()
    {
        // Only clear physics, keep input states
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("IsMoving", false);
    }

    //----//
}