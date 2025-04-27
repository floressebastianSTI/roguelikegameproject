using UnityEngine;

public class PunchVFX : MonoBehaviour
{
    [Header("Stun Settings")]
    [SerializeField] private float freezeDuration = 3f;
    [SerializeField] private Color freezeColor = new Color(0.7f, 0.7f, 1f, 1f);

    private FafnirSkill fafnirSkill;
    private float range;
    private float timer = 0f;
    private bool hasHit = false;
    public float lifetime = 0.5f;

    public void Initialize(FafnirSkill skill, float attackRange, Vector2 direction)
    {
        fafnirSkill = skill;
        range = attackRange;

        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasHit) return;
        
        if (IsEnemy(collision))
        {
            RegisterHit(collision.gameObject);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    public void RegisterHit(GameObject enemy)
    {
        hasHit = true;
        ApplyGuaranteedStun(enemy);
        fafnirSkill?.RegisterPunchHit(enemy);
    }

    private void DetectAndFreezeEnemies()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, range, fafnirSkill.EnemyLayers);
        foreach (Collider2D enemy in enemies)
        {
            if (!hasHit)
            {
                ApplyGuaranteedStun(enemy.gameObject);
                fafnirSkill?.RegisterPunchHit(enemy.gameObject);
                hasHit = true;
                break;
            }
        }
    }

    public bool IsEnemy(Collider2D collider)
    {
        return ((1 << collider.gameObject.layer) & fafnirSkill.EnemyLayers) != 0;
    }

    private void ApplyGuaranteedStun(GameObject enemy)
    {
        if (enemy == null) return;

        // Apply the bulletproof stun
        var stunComponent = enemy.GetComponent<GuaranteedStun>();
        if (stunComponent != null)
        {
            stunComponent.ApplyStun(freezeDuration);
        }
        else
        {
            Debug.LogWarning($"No GuaranteedStun component found on {enemy.name}");
        }
    
        var renderer = enemy.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = freezeColor;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}