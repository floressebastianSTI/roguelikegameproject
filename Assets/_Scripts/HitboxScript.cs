using UnityEngine;

public class HitDetector : MonoBehaviour
{
    [SerializeField]
    public int attackDamage = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageScript damage = collision.GetComponent<DamageScript>();

        if(damage != null)
        {
            print("HIT!");
            damage.Hit(attackDamage);
        }
    }
}
