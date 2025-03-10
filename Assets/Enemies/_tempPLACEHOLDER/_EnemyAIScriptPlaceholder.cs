using UnityEngine;

public class _EnemyAIScriptPlaceholder : MonoBehaviour
{
    public float walkSpeed = 2f;
    private Transform player;

    Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * walkSpeed;
    }
}
