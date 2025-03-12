using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float walkSpeed;
    private Transform player;
    private Rigidbody2D rb;
    public int health;
    private bool isKnockedBack;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!isKnockedBack) // Prevent movement during knockback
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, walkSpeed * Time.deltaTime);
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        health -= damage;
        StartCoroutine(KnockbackCoroutine(knockback));

        if (health <= 0)
        {
            Die();
        }
    }

    private IEnumerator KnockbackCoroutine(Vector2 knockback)
    {
        isKnockedBack = true;
        rb.linearVelocity = Vector2.zero; 
        rb.AddForce(knockback, ForceMode2D.Impulse);

        yield return new WaitForSeconds(0.2f);
        isKnockedBack = false;
    }

    private void Die()
    {
        
    }
}
