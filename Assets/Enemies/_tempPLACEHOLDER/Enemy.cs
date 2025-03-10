using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{

    public float walkSpeed;
    Transform player;
    Animator animator;
    DamageScript damageScript;
    Rigidbody2D rb;
    public int health;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
        damageScript = GetComponent<DamageScript>();
    }

    // Update is called once per frame
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
 
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.position, walkSpeed * Time.deltaTime);
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);
    }
}
