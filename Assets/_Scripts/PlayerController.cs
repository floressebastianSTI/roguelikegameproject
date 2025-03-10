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
    private float dashDistance = 4.3f;

    public float walkSpeed = 0f;
    public float runSpeed = 0f;
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
    private TESTAfterimageScript afterimageEffect;

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
            animator.SetBool("IsRunning", value);

        }

    }

    //----------------------------------------------------// TESTING THEORY
    void Start()
    {
        afterimageEffect = GetComponent<TESTAfterimageScript>();
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
        {
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
            }
            else
            {
                spriteRenderer.flipX = false;
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
            IsMoving = moveInput != Vector2.zero;
        }
        else
        {
            IsMoving = false;
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
            Invoke("StopAfterimage", 0.3f);
            IsRunning = false;
        }
    }
    //lastMoveDir = moveInput.normalized;
    //private Vector3 lastMoveDir;
    // Vector2 moveInput;
    public void OnUniqueSkill(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector3 beforeDashPosition = transform.position;
            Transform dashEffectTransform = Instantiate(dashEffect, beforeDashPosition, Quaternion.identity);

            float angle = UtilsClass.GetAngleFromVectorFloat(lastMoveDir);

            // Flip upside down when dashing left
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
        }
    }

}