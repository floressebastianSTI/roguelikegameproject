using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System.Collections;

public class AttackScript : MonoBehaviour
{
    public SpriteRenderer attackSprite;
    public Animator animator;
    public Transform AttackHitboxes;
    public LayerMask enemyLayers;

    [SerializeField]
    private GameObject[] AttackVFX;

    [SerializeField]
    private GameObject[] Hitboxes;

    [SerializeField]
    public float attackRange = 0.5f;

    [SerializeField]
    private float attackCooldown = 0f;

    [SerializeField]
    private float comboResetTime = 0f;

    [SerializeField]
    private float attackDelay = 0f;

    [SerializeField]
    private float attackDuration = 0f;

    [SerializeField]
    private float attackRadius = 1.0f;

    private int comboStep = 0;
    private float lastAttackTime;
    public bool isAttacking { get; private set; }

    private void ResetIsAttacking()
    {
        isAttacking = false;
    }

    private Coroutine resetComboCoroutine;

    public UnityEvent OnAnimationTrigger;

    private void Start()
    {
        attackSprite.enabled = false;
        OnAnimationTrigger.AddListener(HideAttackSprite);
    }

    private void Update()
    {
        UpdateAttackHitboxesPosition();
    }

    private void UpdateAttackHitboxesPosition()
    { 
        if (!isAttacking)
            return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0;

        Vector3 direction = (mousePos - transform.position).normalized;
        AttackHitboxes.position = transform.position + direction * attackRadius;

        Vector2 scale = transform.localScale;
        if (direction.x < 0)
        {
            scale.y = -1;
        }else if (direction.x > 0)
        {
            scale.y = 1;
        }
        transform.localScale = scale;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        AttackHitboxes.rotation = Quaternion.Euler(0, 0, angle);
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && !isAttacking)
        {
            StartCoroutine(AttackDelay());
        }
    }

    private IEnumerator AttackDelay()
    {
        isAttacking = true;
        yield return new WaitForSeconds(attackDelay);

        Attack();
        isAttacking = false;
    }

    private void Attack()
    {
        lastAttackTime = Time.time;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0;
        AttackHitboxes.right = (mousePos - AttackHitboxes.position).normalized;

        attackSprite.enabled = true;
        animator.SetInteger("ComboStep", comboStep);
        animator.SetTrigger("AttackTrigger");

        if (resetComboCoroutine != null)
        {
            StopCoroutine(resetComboCoroutine);
        }
        resetComboCoroutine = StartCoroutine(ResetComboAfterDelay());


        comboStep = (comboStep + 1) % 3;
    }

    public void TriggerEvent()
    {
        OnAnimationTrigger?.Invoke();
    }

    private IEnumerator ResetComboAfterDelay()
    {
        yield return new WaitForSeconds(comboResetTime);
        comboStep = 0;
    }

    private void HideAttackSprite()
    {
        attackSprite.enabled = false;
    }
}