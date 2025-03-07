using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementScript : MonoBehaviour
{
    private PlayerScript playerScript;
    private Rigidbody2D rb;

    [SerializeField]
    private float moveSpeed = 2f, accel = 50f, deAccel = 100f; //testing new movement acceleration formula, might commit to it

    [SerializeField]
    private float currentMoveSpeed = 0f;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private Vector2 momentum;
    public Vector2 moveInput { get; set; }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();

        spriteRenderer.flipX = false;
    }
    void FixedUpdate()
    {
        if (moveInput.magnitude > 0 && currentMoveSpeed >= 0)
        {
            momentum = moveInput;
            currentMoveSpeed += accel * moveSpeed * Time.deltaTime;
        }
        else
        {
            currentMoveSpeed -= deAccel * moveSpeed * Time.deltaTime;
        }
        currentMoveSpeed = Mathf.Clamp(currentMoveSpeed, 0, moveSpeed);
        rb.linearVelocity = momentum * currentMoveSpeed;
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
    }
}
