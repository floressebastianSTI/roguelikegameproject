using UnityEngine;
using UnityEngine.InputSystem;

public class UltimateAttackScript : MonoBehaviour
{
    [Header("Ultimate Attack Settings")]
    [SerializeField] private GameObject[] attackPrefabs; // Array of attack prefabs for the combo
    [SerializeField] private Transform attackTransform;   // Regular attack position
    [SerializeField] private Transform roarTransform;    // Position for the roar attack
    [SerializeField] public float timeBetweenAttacks = 0.5f; // Cooldown between attacks
    [SerializeField] private float comboResetTime = 2f; // Time before the combo resets

    public bool canAttack = true;
    private float lastAttackTime;
    private int attackIndex = 0;

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        // Handle player rotation and sprite flipping based on mouse position
        HandlePlayerRotation();

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

        // Check if the player can attack and if the left mouse button is pressed
        if (canAttack && Mouse.current.leftButton.wasPressedThisFrame)
        {
            PerformAttack();
        }
    }

    private void HandlePlayerRotation()
    {
        // Get mouse position in world coordinates
        Vector3 mousePosition = mainCam.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, mainCam.nearClipPlane));

        // Calculate rotation and flip player sprite based on mouse position
        Vector3 rotation = mousePosition - transform.position;
        Vector2 scale = transform.localScale;
        scale.y = rotation.x < 0 ? -1 : 1; // Flip sprite based on mouse position
        transform.localScale = scale;

        // Rotate the player to face the mouse
        float rotateZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotateZ);
    }

    private void PerformAttack()
    {
        // Update the last attack time and disable attacking temporarily
        lastAttackTime = Time.time;
        canAttack = false;

        // If it's the roar attack (4th attack), instantiate it at the roarTransform position
        if (attackIndex == 3)
        {
            GameObject roar = Instantiate(attackPrefabs[attackIndex], roarTransform.position, Quaternion.identity);

            // Flip the roar attack direction based on player facing
            Vector3 roarScale = roar.transform.localScale;
            roarScale.x = transform.localScale.y; // Use the player's flipped scale for the roar attack
            roar.transform.localScale = roarScale;
        }
        else
        {
            // Instantiate the regular attack at the attackTransform position
            Instantiate(attackPrefabs[attackIndex], attackTransform.position, Quaternion.identity);
        }

        // Cycle through the attack combo
        attackIndex = (attackIndex + 1) % attackPrefabs.Length;
    }

    // Method to get the current attack index
    public int GetCurrentAttackIndex()
    {
        return attackIndex;
    }
}