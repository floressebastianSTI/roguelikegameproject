using UnityEngine;

public class VFXHitbox : MonoBehaviour
{
    public int damage = 10;
    public float stunDuration = 1f;
    private SkypiercerDash skypiercerDash;

    private void Start()
    {
        skypiercerDash = FindFirstObjectByType<SkypiercerDash>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyDamageScript enemy = other.GetComponent<EnemyDamageScript>();
            if (enemy != null)
            {
                enemy.Hit(damage, transform.position, stunDuration);
                skypiercerDash.SetHitEnemy(); // Notify SkypiercerDash of successful hit
            }
        }
    }
}