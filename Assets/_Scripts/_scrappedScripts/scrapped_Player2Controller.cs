using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player2Controller : MonoBehaviour
{
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

    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

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
    public void OnMove(InputAction.CallbackContext context2)
    {
        moveInput = context2.ReadValue<Vector2>();

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
    public void OnRun(InputAction.CallbackContext context2)
    {
        if (context2.started)
        {
            IsRunning = true;
        }
        else if (context2.canceled)
        {
            IsRunning = false;
        }
    }
    //lastMoveDir = moveInput.normalized;
    //private Vector3 lastMoveDir;
    // Vector2 moveInput;
    public void OnUniqueSkill(InputAction.CallbackContext context2)
    {
      
    }

}
