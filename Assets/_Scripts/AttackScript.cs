using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackScript : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 mousePosition;

    [SerializeField]
    public GameObject Attack1, Attack2, Attack3;

    public Transform attackTransform;

    public bool canAttack;
    public float timeBetweenAttacks;
    public float resetAttackTime = 2f;
    private float timer;
    private float lastAttackTime;
    private int attackIndex = 0;

    private GameObject[] attackObjects;

    void Start()
    {
        mainCam = Camera.main;
        attackObjects = new GameObject[] { Attack1, Attack2, Attack3 };
    }

    private void Update()
    {
        mousePosition = mainCam.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, mainCam.nearClipPlane));

        Vector3 rotation = mousePosition - transform.position;
        Vector2 scale = transform.localScale;

        scale.y = rotation.x < 0 ? -1 : 1;
        transform.localScale = scale;

        float rotateZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotateZ);

        if (!canAttack)
        {
            timer += Time.deltaTime;
            if (timer > timeBetweenAttacks)
            {
                canAttack = true;
                timer = 0;
            }
        }

        if (Time.time - lastAttackTime > resetAttackTime)
        {
            attackIndex = 0;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame && canAttack)
        {
            canAttack = false;
            lastAttackTime = Time.time; // Update last attack time
            Instantiate(attackObjects[attackIndex], attackTransform.position, Quaternion.identity);
            attackIndex = (attackIndex + 1) % attackObjects.Length; // Cycle through attacks
        }
    }

    //-----------------------------------------------------// TESTING THEORY

    //-----------------------------------------------------------------------//
}
