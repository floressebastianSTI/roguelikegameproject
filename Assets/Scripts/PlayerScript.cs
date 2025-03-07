using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
 
    private MovementScript moveScript;
    private AnimationScript animScript;
    private CrosshairScript crosshairScript;
    private AttackScript attackScript;

    [SerializeField]
    public SpriteRenderer spriteRenderer;

    [SerializeField]
    private InputActionReference moveAction, attackAction, pointerPosition;

    private Vector2 movementInput;

    private Vector2 pointerInput;
 
    private void Awake()
    {
        animScript = GetComponent<AnimationScript>();
        moveScript = GetComponent<MovementScript>();
        attackScript = GetComponentInChildren<AttackScript>();
        crosshairScript = GetComponentInChildren<CrosshairScript>();
    }
    private void OnEnable()
    {
        attackAction.action.performed += context => attackScript.OnAttack(context);
    }
    private void OnDisable()
    {
        attackAction.action.performed -= context => attackScript.OnAttack(context);
    }
    void Update()
    {
        
        pointerInput = GetPointerInput();
        movementInput = moveAction.action.ReadValue<Vector2>().normalized;

        moveScript.moveInput = movementInput;

        AnimateCharacter();
    }
    public void FixedUpdate()
    {
  
    }
    void AnimateCharacter()
    {
        animScript.RunAnimation(movementInput);
    }
    private Vector2 GetPointerInput()
    {
        Vector3 mousePosition = pointerPosition.action.ReadValue<Vector2>();
        mousePosition.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }    
}
