using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;
public class AttackHandler : MonoBehaviour
{
    [SerializeField]
    public InputActionReference attackAction, moveAction, specialAction, ultAction, pointerPosition;

    [SerializeField]
    public Animator vfxAnimator;

    [SerializeField]
    public SpriteRenderer vfxRenderer;

    [SerializeField]
    public AnimEventHelper eventHandler;

    [SerializeField]
    public TrailRenderer attackTrail;

    private AnimEventHelper animEvent;

    public Vector2 PointerPosition { get; set; }

    //-----------------------------------------------// this is to figure out attack combos

    [SerializeField]
    private int numberofAttacks = 3;

    private int currentAttackCounter = 0;

    public int CurrentAttackCounter
    {
        get => currentAttackCounter;
        private set => currentAttackCounter = value >= numberofAttacks ? 0 : value;
    }

    //----------------------------------------------------------------------------------//

    public float delay = 0f;

        private bool attackBlocked;

    public bool isAnimationFinished;

    public bool isAttackDone;

    public bool isAttacking { get; private set; }

    public void ResetIsAttacking()
    {
        isAttacking = false;
    }

    public void AnimationFinishTrigger()
    {
        isAnimationFinished = true;
    }
    

    //---------------------------------------------------------------------------------//

    public void Start()
    {
        animEvent = GetComponent<AnimEventHelper>();
    }
    private void OnEnable()
    {
        attackAction.action.performed += OnAttack;
    }
    private void OnDisable()
    {
        attackAction.action.performed -= OnAttack;
    }

    private void Update()
    {

        transform.right = (PointerPosition - (Vector2)transform.position).normalized;
        Vector2 direction = (PointerPosition -(Vector2)transform.position).normalized;

        Vector2 scale = transform.localScale;
        if (direction.x < 0)
        {
            scale.y = -1;
        } else if (direction.x > 0)
        {
            scale.y = 1;
        }
        transform.localScale = scale;

    }
    private void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !attackBlocked)
        {

            print("animation working");
            StartCoroutine(PerformAttack());
        }
    }
    IEnumerator PerformAttack()
    {
        if (attackBlocked)
            yield return new WaitForSeconds(0f);
        isAttacking = true;

        attackBlocked = true;
        isAnimationFinished = false;
        Enter();
        attackTrail.emitting = true;

        yield return new WaitUntil(() => isAnimationFinished);

        Exit();
        attackTrail.emitting = false;
        attackBlocked = false;
    }
    public void Enter()
    {
        if (CurrentAttackCounter >= numberofAttacks)
        {
            currentAttackCounter = 0;
        }
        vfxAnimator.SetInteger("counter", CurrentAttackCounter);
    }
    private void Exit()
    {

        CurrentAttackCounter++;
    }

}
