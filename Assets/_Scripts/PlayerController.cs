using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    public Transform dashEffect;

    [SerializeField]
    public Transform runEffect;

    [SerializeField]
    private float runEffectInterval = 0.2f;
    private float runEffectTimer = 0f;

    [SerializeField]
    private float dashDistance = 4.3f;

    [SerializeField]
    private float dashCooldown = 1.5f;

    private bool canDash = true;

    public float walkSpeed = 0f;
    public float runSpeed = 0f;
    private float stopRunThreshold = 0.1f; 
    private float stopRunTimer = 0f;
    private float stopRunDelay = 0.2f; 

    Vector2 moveInput;
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
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    private AfterimageScript afterimageEffect;

    [SerializeField]
    private InputActionReference pointerPosition;

    [SerializeField]
    private bool walking = false;

    [SerializeField]
    private bool running = false;

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
            animator.SetBool("IsWalking", value);

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
        }
    }

    //----------------------------------------------------// TESTING THEORY
    void Start()
    {
        afterimageEffect = GetComponent<AfterimageScript>();
    }
    void StopAfterimage()
    {
        afterimageEffect.StopAfterimage();
    }

    //---------------------------------------------------------------------//

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.flipX = false;
    }
    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * CurrentMoveSpeed;

        if (isAlive)
        {
            Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, 0));
            mousePosition.z = 0;

            if (mouseScreenPosition != Vector2.zero)
            {
                bool isMouseRight = mousePosition.x > transform.position.x;
                spriteRenderer.flipX = !isMouseRight;
            }

            // Determine the facing direction
            Vector2 facingDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right;

            // Determine if the movement is opposite to the facing direction
            bool isMovingBackward = Vector2.Dot(facingDirection, moveInput) < 0;

            // Apply running animation based on movement direction
            animator.SetBool("IsRunningBackward", IsRunning && isMovingBackward);
            animator.SetBool("IsRunningForward", IsRunning && !isMovingBackward);
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

    private Vector2 GetPointerInput()
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
            afterimageEffect.StartAfterimage();
            IsRunning = true;
        }
        else if (context.canceled)
        {
            StartCoroutine(DelayedStopAfterimage());
            IsRunning = false;
        }
    }

    private IEnumerator DelayedStopAfterimage()
    {
        yield return new WaitForSeconds(0.3f);
        if (!IsRunning) 
        {
            afterimageEffect.StopAfterimage();
        }
    }


    //lastMoveDir = moveInput.normalized;
    //private Vector3 lastMoveDir;
    // Vector2 moveInput;
    public void OnUniqueSkill(InputAction.CallbackContext context)
    {
        if (context.performed && canDash) // Check if dash is available
        {
            StartCoroutine(Dash()); // Start the dash coroutine
        }
    }
    private IEnumerator Dash()
    {
        canDash = false;

        Vector3 beforeDashPosition = transform.position;
        Transform dashEffectTransform = Instantiate(dashEffect, beforeDashPosition, Quaternion.identity);

        float angle = UtilsClass.GetAngleFromVectorFloat(lastMoveDir);
        if (lastMoveDir.x < 0)
        {
            dashEffectTransform.eulerAngles = new Vector3(180, 0, -angle);
        }
        else
        {
            dashEffectTransform.eulerAngles = new Vector3(0, 0, angle);
        }

        float dashEffectWidth = 5f;
        dashEffectTransform.localScale = new Vector3(dashDistance / dashEffectWidth, 1f, 1f);
        transform.position += lastMoveDir * dashDistance;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }
}