using Unity.Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBulletHitboxScript : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D rb;

    [Header("Projectile Speed Settings")]
    public float force;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");

        Vector3 direction = player.transform.position - transform.position;
        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * force;

        float rot = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot + 131);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ApplyDamage(other.gameObject);
        }
    }

    public void ApplyDamage(GameObject playerObject)
    {
        DamageScript damageScript = playerObject.GetComponent<DamageScript>();
        if (damageScript != null)
        {
            damageScript.hp -= 10;
        // Call the flash effect on the player's script
        DamageScript playerComponent = playerObject.GetComponent<DamageScript>();
        if (playerComponent != null)
        {
            playerComponent.TriggerFlashEffect();
        }
    }

    Destroy(gameObject);
    }


}
