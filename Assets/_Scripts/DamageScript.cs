using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.UI;
using TMPro;

public class DamageScript : MonoBehaviour
{
    CinemachineImpulseSource impulseSource;
    Animator animator;

    [Header("VFX Settings")]
    [SerializeField] private HitFlashScript flashEffect;

    [Header("HP Value")]
    public Slider hpSlider1; //Board HP Bar
    public Slider hpSlider2; //Above player head HP Bar
    public TextMeshProUGUI hpValue; //HP Bar text amount value

    [SerializeField]
    private int _health;
    [SerializeField]
    public int maxHP = 250;
    public int hp
    {
        get { return _health; }
        set
        {
            _health = value;
            if (_health <= 0)
            {
                IsAlive = false;
            }
        }
    }

    [SerializeField] private bool _isAlive = true;
    private bool IsAlive
    {
        get { return _isAlive; }
        set
        {
            _isAlive = value;
            animator.SetBool("IsAlive", value);
            Debug.Log("Still Alive: " + value);
        }
    }

    public bool _IsHit = true;
    private bool IsHit
    {
        get { return _IsHit; }
        set
        {
            _IsHit = value;
            animator.SetBool("IsHit", value);
        }
    }

    [SerializeField] private bool isInvincible = false;
    private float timeSinceDamaged = 0;
    public float invincibilityTime = 0.4f;

    private void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isInvincible)
        {
            if (timeSinceDamaged > invincibilityTime)
            {
                isInvincible = false;
                timeSinceDamaged = 0;
            }
            timeSinceDamaged += Time.deltaTime;
        }
    }

    public void Hit(int damage, Vector2 attackerPosition)
    {
        if (IsAlive && !isInvincible)
        {
            isInvincible = true;
            flashEffect.Flash();

            hp -= damage;
            hpSlider1.value = hp;
            hpSlider2.value = hp;
            hpValue.text = $"{hp.ToString()} / {maxHp.ToString()}";

            EnemyAI enemyAI = GetComponent<EnemyAI>();
            RangedEnemyAI rangedEnemyAI = GetComponent<RangedEnemyAI>();

            if (enemyAI != null || rangedEnemyAI != null)
            {
                Vector2 hitPoint = transform.position;
                Vector2 hitDirection = (transform.position - (Vector3)attackerPosition).normalized;
                Vector2 knockbackDirection = (transform.position - (Vector3)attackerPosition).normalized;
                float knockbackForce = 5f;
                Vector2 knockback = knockbackDirection * knockbackForce;

                if (enemyAI != null)
                    enemyAI.OnHit(damage, knockback, hitPoint, hitDirection);
                if (rangedEnemyAI != null)
                    rangedEnemyAI.OnHit(damage, knockback, hitPoint, hitDirection);
            }
        }
    }

    public void PlayerHeal(int healAmount)
    {
        hp = Mathf.Min(hp + healAmount, maxHP); // Prevent overheal
        Debug.Log("Player Healed: " + healAmount + " | Current HP: " + hp);
    }

    public void TriggerFlashEffect()
    {
        if (flashEffect != null)
        {
            flashEffect.Flash();
        }
        else
        {
            Debug.LogWarning("Flash effect not assigned in DamageScript!");
        }
    }
}

