using UnityEngine;
using System.Collections;

public class EnemyKnockback : MonoBehaviour
{
    public float defaultHitstunDuration = 0.5f; // Default duration
    private float currentHitstunDuration;

    private Rigidbody2D rb;
    private EnemyAIScript enemyAI; // Changed from MeleeEnemyAI to EnemyAIScript
    private Animator animator;
    private bool isInHitstun = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyAI = GetComponent<EnemyAIScript>();
        animator = GetComponent<Animator>();
        currentHitstunDuration = defaultHitstunDuration;
    }

    // Public method to set custom hitstun duration
    public void SetHitstunDuration(float duration)
    {
        currentHitstunDuration = duration;
    }

    // Reset to default duration
    public void ResetHitstunDuration()
    {
        currentHitstunDuration = defaultHitstunDuration;
    }

    public void TakeKnockback(Vector2 sourcePosition, float knockbackStrength)
    {
        if (isInHitstun) return;

        Vector2 enemyPosition = transform.position;
        Vector2 knockbackDirection = (enemyPosition - sourcePosition).normalized;

        if (knockbackDirection.magnitude < 0.1f)
        {
            knockbackDirection = Vector2.up;
        }

        rb.AddForce(knockbackDirection * knockbackStrength, ForceMode2D.Impulse);
        StartCoroutine(HandleHitstun());
    }

    private IEnumerator HandleHitstun()
    {
        isInHitstun = true;

        if (enemyAI != null)
        {
            enemyAI.enabled = false;
        }

        if (animator != null)
        {
            animator.speed = 0;
        }

        yield return new WaitForSeconds(currentHitstunDuration);

        if (enemyAI != null)
        {
            enemyAI.enabled = true;
        }

        if (animator != null)
        {
            animator.speed = 1;
        }

        isInHitstun = false;
        ResetHitstunDuration(); // Reset to default after each hitstun
    }

    // Optional: Add this if you want to be able to interrupt hitstun early
    public void EndHitstunEarly()
    {
        if (isInHitstun)
        {
            StopAllCoroutines();
            isInHitstun = false;

            if (enemyAI != null) enemyAI.enabled = true;
            if (animator != null) animator.speed = 1;
            ResetHitstunDuration();
        }
    }
}