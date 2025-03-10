using UnityEngine;
using Unity.Cinemachine;

public class DamageScript : MonoBehaviour
{

    CinemachineImpulseSource impulseSource;
    Animator animator;

    [SerializeField]
    private HitFlashScript flashEffect;

    private KeyCode flashKey;

    [SerializeField]
    private int _health = 1000;
    public int hp
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;

            if (_health < 0)
                IsAlive = false;
        }
    }


    [SerializeField]
    public bool _isAlive = true;
    private bool IsAlive
    { get
        {
            return _isAlive;
        }
        set
        {
            _isAlive = value;
            animator.SetBool("IsAlive", value);
            Debug.Log("Still Alive." + value);
        }
    }

    public bool _IsHit = true;

    private bool IsHit
    {
        get
        {
            return _IsHit;
        }
        set
        {
            _IsHit = value;
            animator.SetBool("IsHit", value);
        }
    }
    [SerializeField]
    private bool isInvincible = false;

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

        if (Input.GetKeyDown(flashKey))
        {
            flashEffect.Flash();
        }
    }

    public void Hit(int damage)
    {
        if (IsAlive && !isInvincible)
        {
            hp -= damage;
            isInvincible = true;
            impulseSource.GenerateImpulse();
            flashEffect.Flash();
        }
    }
}

