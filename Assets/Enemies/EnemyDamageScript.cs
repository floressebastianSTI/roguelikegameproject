using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Splines;

public class EnemyDamageScript : MonoBehaviour
{
    CinemachineImpulseSource impulseSource;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    [Header("Stun Settings")]
    public bool isStunned = false;
    private float stunEndTime;
    private Color originalColor;
    private float originalAnimSpeed;
    public bool IsStunned { get; set; }

    [SerializeField] private HitFlashScript flashEffect;
    [SerializeField] private GameObject explosionEffect;

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
                if (explosionEffect != null)
                {
                    GameObject player = GameObject.FindGameObjectWithTag("Player"); // Find the player
                    if (player != null)
                    {
                        Vector2 direction = (transform.position - player.transform.position).normalized;
                        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Convert to degrees

                        // Instantiate explosion and apply rotation
                        Instantiate(explosionEffect, transform.position, Quaternion.Euler(0, 0, angle));
                    }
                    else
                    {
                        // Default explosion if player is not found
                        Instantiate(explosionEffect, transform.position, Quaternion.identity);
                    }
                }
            }
        }
    }

    [SerializeField]
    private bool _isAlive = true;
    public bool IsAlive
    {
        get => _isAlive;
        set
        {
            _isAlive = value;

            if (anim != null)
                anim.SetBool("IsAlive", value);
            Debug.Log("IsAlive set to: " + value);

            {
                if (!value)
                    anim.SetBool("IsAlive", false);
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
            if (anim != null)
            {
                anim.SetBool("IsHit", value);
            }
        }
    }

    [SerializeField]
    private bool isInvincible = false;
    private float timeSinceDamaged = 0;
    public float invincibilityTime = 0.4f;

    private EnemyKnockback knockback; // Reference to the EnemyKnockback script

    private void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
        playerUltimate = FindFirstObjectByType<UltimateMode>();
        knockback = GetComponent<EnemyKnockback>(); // Get the EnemyKnockback component
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isStunned && Time.time >= stunEndTime)
        {
            RemoveStun();
        }
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

    public void Hit(int damage, Vector2 playerPosition, float knockbackStrength)
    {
        if (IsAlive && !isInvincible)
        {
            hp -= damage;
            isInvincible = true;

            // Trigger flash effect
            flashEffect.Flash();

            // Apply knockback based on player's position and knockback strength
            if (knockback != null)
            {
                knockback.TakeKnockback(playerPosition, knockbackStrength);
            }

            // Trigger screen shake
            if (impulseSource != null)
            {
                impulseSource.GenerateImpulse();
            }

            // Add to player's ultimate meter
            if (playerUltimate != null)
            {
                playerUltimate.AddUltimateMeter(damage * 0.2f);
            }
            if (!isStunned && rb != null)
            {
                Vector2 knockbackDir = ((Vector2)transform.position - playerPosition).normalized;
                rb.AddForce(knockbackDir * knockbackStrength, ForceMode2D.Impulse);
            }

        // Optional: Play a sound effect
        // audioSource.PlayOneShot(hitSound);
    }
    }

    //TEST

    [System.Serializable]
    public class EnemyState
    {
        public bool isStunned;
        public float stunEndTime;
        public Vector2 preStunVelocity;
        public float preStunAnimSpeed;
        public Color preStunColor;
    }

    public EnemyState currentState = new EnemyState();

    public void ApplyStun(float duration)
    {
        if (isStunned) return; // Prevent overlapping stuns

        isStunned = true;
        stunEndTime = Time.time + duration;

        // Store original values
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
            spriteRenderer.color = new Color(0.7f, 0.7f, 1f, 1f); // Bluish tint
        }

        if (anim != null)
        {
            originalAnimSpeed = anim.speed;
            anim.speed = 0f; // Freeze animation
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Stop movement
        }
    }

    public void RemoveStun()
    {
        isStunned = false;

        if (!currentState.isStunned) return;

        var rb = GetComponent<Rigidbody2D>();
        var anim = GetComponent<Animator>();
        var renderer = GetComponent<SpriteRenderer>();

        // Restore pre-stun state
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = currentState.preStunVelocity;
        }
        if (anim != null) anim.speed = currentState.preStunAnimSpeed;
        if (renderer != null) renderer.color = currentState.preStunColor;

        currentState.isStunned = false;
    }
}