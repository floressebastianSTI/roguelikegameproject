using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public Vector2 PointerPosition { get; set; }

    //-------------------------------------------------// For character animation locking

    private bool isAttacking = false;

    //-------------------------------------------------//

    public ContactFilter2D moveFilter;

    Vector2 moveDirection;
    Vector2 moveInput;

    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;

    [SerializeField]
    public AnimEventHelper eventHandler;

    private Vector2 pointerInput;
    private Vector2 PointerInput => pointerInput;

    private AttackHandler attackHandler;
    private CrosshairAim crosshairAim;

    List<RaycastHit2D> castCollision = new List<RaycastHit2D>();

    //VARIABLES FROM HERE ARE INPUT VARIABLES FOR ATTACKS AND ABILITIES//

    //-----------------------------------------------------------------//
    //DASH MECHANIC FOR ORPHEUS
    [SerializeField]
    private InputActionReference attackAction, moveAction, specialAction, ultAction, pointerPosition;

    [SerializeField]
    private TrailRenderer trail;

    [SerializeField]
    float dashSpeed = 20f;

    [SerializeField]
    float dashDuration = 1f;

    [SerializeField]
    float dashCooldown = 2f;

    bool isDashing;
    bool canDash = true;

    public bool isAnimationFinished;
    public void AnimationFinishTrigger()
    {
        isAnimationFinished = true;
    }

    //-----------------------------------------------------------------//
    //SPECIAL DASH MOTION FOR ATTACKS

    [SerializeField]
    private TrailRenderer attackTrail;

    [SerializeField]
    float attackDashSpeed = 25f;

    [SerializeField]
    float attackDashLimit = 0.2f;

    [SerializeField]
    float attackDashCooldown = 2f;

    bool isAttackDashing;

    bool canAttackDash = true;

    [SerializeField]
    private int numberofAttacks = 3;

    private int currentAttackCounter = 0;

    public int CurrentAttackCounter
    {
        get => currentAttackCounter;
        private set => currentAttackCounter = value >= numberofAttacks ? 0 : value;
    }

    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        attackHandler = GetComponentInChildren<AttackHandler>(); //this is so the crosshair in CrosshairAim actually works as intended
        animator = GetComponent<Animator>(); //dont forget to aalways initiallize components
        spriteRenderer = GetComponent<SpriteRenderer>();
        crosshairAim = GetComponentInChildren<CrosshairAim>();

        //moveAction basically enables the new InputSystem to work through the SerializeField reference, ARGHHHHH!!!!!!!!!! (took me 2 hours to figure out)
        moveAction.action.performed += Move;
        moveAction.action.canceled += Move;

    }

    private void Update()
    {
        //this is to actually use the serialize field of pointerPosition and get the OnPointerPosition input 
        pointerInput = GetPointerInput();


    }
    private void FixedUpdate()
    {
        crosshairAim.PointerPosition = pointerInput;
        attackHandler.PointerPosition = pointerInput; //related to CrosshairAim script, disregard for the code underneath

        //this is for future enemy collission interactions, irrelevant for now until Kai gives me enemy code
        if (isAttackDashing)
        {
            return;
        }
        if (isDashing)
        {
            return;
        }

        if (moveInput != Vector2.zero)
        {
            bool success = TryMove(moveInput);

            {
                if (isAttackDashing)
                    return;
            }
            if (!success && moveInput.x > 0)
            {
                success = TryMove(new Vector2(moveInput.x, 0));


                if (!success && moveInput.y > 0)
                {
                    success = TryMove(new Vector2(0, moveInput.y));
                }
            }

            //booleans to make this work thru animator, yes
            animator.SetBool("isMoving", success);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }


        //THIS IS A RECENT CHANGE!!!!!!!! so character turns with the mouse position instead of turning with the side strafe keys
        if (isAttacking)
            return;
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, 0));
        mousePosition.z = 0;

        bool isMouseRight = mousePosition.x < transform.position.x;
        spriteRenderer.flipX = isMouseRight;
        StartCoroutine(UnlockAttackDirection());
        //update: it works lol!

    }

    private Vector2 GetPointerInput()
    {
        Vector3 mousePos = pointerPosition.action.ReadValue<Vector2>();
        mousePos.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    private bool TryMove(Vector2 direction)
    {
        int count = rb.Cast(direction, moveFilter, castCollision, moveSpeed * Time.fixedDeltaTime + collisionOffset);

        if (count == 0)
        {
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
            return true;
        }
        else
        {
            return false;
        }
    }

    //to trigger movement inputs
    void Move(InputAction.CallbackContext context)

    {
        moveInput = context.ReadValue<Vector2>();
    }

    //more in-game inputs here

    IEnumerator UnlockAttackDirection()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        isAttacking = false;
    }
    IEnumerator Dash() //EXCLUSIVE TO CHARACTER ORPHEUS
    {
        if (!canDash || isDashing || moveInput == Vector2.zero)
            yield break;

        canDash = false;
        isDashing = true;
        trail.emitting = true;

        //normalize the input for consistent dash speed
        Vector2 dashDirection = moveInput.normalized;
        rb.linearVelocity = dashDirection * dashSpeed;

        {
            if (isAttackDashing)
                yield break;
        }

        yield return new WaitForSeconds(dashDuration);


        rb.linearVelocity = Vector2.zero; //to stop dash
        isDashing = false;
        trail.emitting = false;

        yield return new WaitForSeconds(dashCooldown); //so fuckers cant spam it LOL
        canDash = true;
    }
    IEnumerator AttackOneDash()
    {
        if (!canAttackDash || isAttackDashing)
            yield break;

        if (isDashing)
        {
            StopCoroutine(nameof(Dash));
            isDashing = false;
            trail.emitting = true;
            rb.linearVelocity = Vector2.zero;
        }
        canAttackDash = false;
        isAttackDashing = true;
        attackTrail.emitting = true;

        Vector2 attackDashDirection = (pointerInput - (Vector2)transform.position).normalized; //direction toward the mouse pointer
        rb.linearVelocity = attackDashDirection * attackDashSpeed;

        yield return new WaitForSeconds(attackDashLimit);

        rb.linearVelocity = Vector2.zero;  //stop attack dash
        isAttackDashing = false;
        trail.emitting = false;

        yield return new WaitForSeconds(attackDashCooldown);
        canAttackDash = true;

    }
    IEnumerator AttackState()
    {
        StartCoroutine(AttackOneDash());
        StartCoroutine(UnlockAttackDirection());
        isAttacking = true;
        isAnimationFinished = false;
        Enter();
        attackTrail.emitting = true;

        yield return new WaitUntil(() => isAnimationFinished);

        attackTrail.emitting = false;
        Exit();

    }
    public void Enter()
    {
        if (CurrentAttackCounter >= numberofAttacks)
        {
            currentAttackCounter = 0;
        }
        animator.SetInteger("counter", CurrentAttackCounter);
    }
    private void Exit()
    {

        CurrentAttackCounter++;
    }
    void OnAttack()
    {
        print("SLASH!");
        StartCoroutine(AttackState());
    }
void OnUniqueMove()
    {
        print("DASH!");
            StartCoroutine(Dash());

    }

    void OnUltimate()
    {
        print("ULTIMATE!");
    }

    }


