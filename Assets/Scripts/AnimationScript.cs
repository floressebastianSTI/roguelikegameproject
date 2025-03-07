using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class AnimationScript : MonoBehaviour
{
    private int comboStep = 0;
    public UnityEvent OnAnimationTrigger;
    private Animator animator;
    private PlayerInput playerInput;

    void Awake()
    {
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();

        // Subscribe Attack method to input system
        playerInput.actions["Attack"].performed += AttackAnim;
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        playerInput.actions["Attack"].performed -= AttackAnim;
    }

    public void RunAnimation(Vector2 moveInput)
    {
        animator.SetBool("isMoving", moveInput.magnitude > 0);
    }

    private void AttackAnim(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            animator.SetTrigger("AnimTrigger");
            animator.SetInteger("ComboStep", comboStep);
            comboStep = (comboStep + 1) % 3;
        }
    }

    public void TriggerEvent()
    {
        OnAnimationTrigger?.Invoke();
    }
}
