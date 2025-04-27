
using UnityEngine;
using Unity.Cinemachine;

public class EnemyDamageScript : MonoBehaviour
{
    CinemachineImpulseSource impulseSource;
    Animator animator;

    [SerializeField]
    private HitFlashScript flashEffect;

    private UltimateMode playerUltimate;

    [SerializeField]
    private int _health = 1000;
    public int hp
    {
        get => _health;
        set
        {
            _health = value;
            if (_health <= 0) 
            {
                IsAlive = false;
            }
        }
    }

    [SerializeField]
    private bool _isAlive = true;
    private bool IsAlive
    {
        get => _isAlive;
        set
        {
            _isAlive = value;

            if (animator != null)
                animator.SetBool("IsAlive", value);
            Debug.Log("IsAlive set to: " + value);

            {
                if (!value)
                    animator.SetBool("IsAlive", false);
            }
        }
    }
    public bool _IsHit = true;
    private bool IsHit
    {
        get => _IsHit;
        set
        {
            _IsHit = value;
            if (animator != null)
            {
                animator.SetBool("IsHit", value);
            }
        }
    }

    [SerializeField]
    private bool isInvincible = false;
    private float timeSinceDamaged = 0;
    public float invincibilityTime = 0.4f;

    private void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        playerUltimate = FindFirstObjectByType<UltimateMode>();
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isInvincible)
        {
            timeSinceDamaged += Time.deltaTime;
            if (timeSinceDamaged > invincibilityTime)
            {
                isInvincible = false;
                timeSinceDamaged = 0;
            }
        }
    }

    public void Hit(int damage, Vector2 attackerPosition)
    {
        if (IsAlive && !isInvincible)
        {
            hp -= damage;
            isInvincible = true;
            flashEffect.Flash();

            if (playerUltimate != null)
            {
                playerUltimate.AddUltimateMeter(damage * 0.2f);
            }
        }
    }
}   
