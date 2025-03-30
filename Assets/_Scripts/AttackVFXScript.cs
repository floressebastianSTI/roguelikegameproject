using UnityEngine;
using UnityEngine.InputSystem;

public class AttackVFXScript : MonoBehaviour
{
    [Header("Attack VFX Settings")]
    public float projectileDuration = 1f;
    public float force; 

    public static float ultimateSizeMultiplier = 1f; 

    private Camera mainCam;
    private Rigidbody2D rb;
    private Vector3 targetDirection;

    private void Start()
    {
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody2D>();

        Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = mainCam.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, mainCam.nearClipPlane));
        mouseWorldPosition.z = 0;

        targetDirection = (mouseWorldPosition - transform.position).normalized;

        rb.linearVelocity = targetDirection * force;

        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        transform.localScale *= ultimateSizeMultiplier;

        Destroy(gameObject, projectileDuration);
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
}