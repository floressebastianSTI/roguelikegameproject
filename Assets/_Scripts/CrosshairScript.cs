using UnityEngine;
using UnityEngine.InputSystem;

public class OrbitingCrosshair : MonoBehaviour
{
    [Header("Orbit Settings")]
    [SerializeField] private float orbitRadius = 2f; // Distance from the player
    [SerializeField] public float rotationSpeed = 5f; // Speed of rotation

    private Transform playerTransform;
    private Camera mainCam;
    private PlayerInput playerInput;

    void Start()
    {
        playerTransform = transform.parent; // Assuming the crosshair is a child of the player
        mainCam = Camera.main;
        playerInput = GetComponentInParent<PlayerInput>(); // Get the PlayerInput component from the player
    }

    void Update()
    {
        // Get the mouse position using the New Input System
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = mainCam.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, mainCam.nearClipPlane));
        mouseWorldPosition.z = 0; // Ensure Z is 0 for 2D

        // Calculate the direction from the player to the mouse
        Vector3 direction = (mouseWorldPosition - playerTransform.position).normalized;

        // Position the crosshair at the orbit radius
        transform.position = playerTransform.position + direction * orbitRadius;

        // Rotate the crosshair to face the mouse position
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}