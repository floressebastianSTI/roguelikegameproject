using UnityEngine;
using UnityEngine.InputSystem;

public class AttackScript : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private GameObject[] attackPrefabs; // Array of attack prefabs for the combo
    [SerializeField] private Transform crosshairTransform; // The orbiting crosshair/attack point
    [SerializeField] public float timeBetweenAttacks = 0.5f; // Cooldown between attacks
    [SerializeField] private float comboResetTime = 2f; // Time before the combo resets

    public bool canAttack = true;
    private float lastAttackTime;
    private int attackIndex = 0;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement component not found on the same GameObject!");
        }
    }

    void Update()
    {
        // Check if the player can attack and if the left mouse button is pressed
        if (canAttack && Mouse.current.leftButton.wasPressedThisFrame)
        {
            PerformAttack();
        }

        // Handle attack cooldown
        if (!canAttack)
        {
            if (Time.time - lastAttackTime >= timeBetweenAttacks)
            {
                canAttack = true;
            }
        }

        // Reset the combo if too much time has passed between attacks
        if (Time.time - lastAttackTime >= comboResetTime)
        {
            attackIndex = 0;
        }
    }

    private void PerformAttack()
    {
        // Skip attack if inputs are blocked (during Fafnir skill)
        if (playerMovement != null && playerMovement.blockAllInputs)
        {
            return;
        }

        // Update the last attack time and disable attacking temporarily
        lastAttackTime = Time.time;
        canAttack = false;

        // Instantiate the attack effect at the crosshair's position and rotation
        GameObject attackInstance = Instantiate(attackPrefabs[attackIndex], crosshairTransform.position, crosshairTransform.rotation);

        // Cycle through the attack combo
        attackIndex = (attackIndex + 1) % attackPrefabs.Length;
    }

    // Method to get the current attack index
    public int GetCurrentAttackIndex()
    {
        return attackIndex;
    }
}