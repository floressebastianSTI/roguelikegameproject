using System.Collections;
using UnityEngine;

public class GuaranteedStun : MonoBehaviour
{
    [System.Serializable]
    public class StunState
    {
        public bool isStunned;
        public float stunEndTime;
        public Vector2 preStunVelocity;
        public float preStunAnimSpeed;
        public Color preStunColor;
        public RigidbodyType2D preStunBodyType;
    }

    public StunState currentStun = new StunState();
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ApplyStun(float duration)
    {
        // Don't refresh stun if already stunned for longer
        if (currentStun.isStunned && Time.time + duration <= currentStun.stunEndTime)
            return;

        currentStun.isStunned = true;
        currentStun.stunEndTime = Time.time + duration;

        // Store original state
        if (rb != null)
        {
            currentStun.preStunVelocity = rb.linearVelocity;
            currentStun.preStunBodyType = rb.bodyType;
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        if (anim != null)
        {
            currentStun.preStunAnimSpeed = anim.speed;
            anim.speed = 0f;
        }

        if (spriteRenderer != null)
        {
            currentStun.preStunColor = spriteRenderer.color;
            spriteRenderer.color = new Color(0.7f, 0.7f, 1f, 1f);
        }
    }

    private void Update()
    {
        if (currentStun.isStunned && Time.time >= currentStun.stunEndTime)
        {
            RemoveStun();
        }

        // Continuously enforce stun if active
        if (currentStun.isStunned)
        {
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }
    }

    private void RemoveStun()
    {
        if (!currentStun.isStunned) return;

        if (rb != null)
        {
            rb.bodyType = currentStun.preStunBodyType;
            rb.linearVelocity = currentStun.preStunVelocity;
        }

        if (anim != null) anim.speed = currentStun.preStunAnimSpeed;
        if (spriteRenderer != null) spriteRenderer.color = currentStun.preStunColor;

        currentStun.isStunned = false;
    }

    private void OnDisable()
    {
        RemoveStun();
    }
}