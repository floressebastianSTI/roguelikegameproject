using UnityEngine;
using UnityEngine.InputSystem;

public class AttackVFXScript : MonoBehaviour
{
    [Header("Attack VFX Settings")]
    public float projectileDuration = 1f;
    public float force;

    private Vector3 mousePosition;
    private Camera mainCam;
    private Rigidbody2D rb;
    private Collider2D hitbox;

    private void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        rb = GetComponent<Rigidbody2D>();

        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        mousePosition = mainCam.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, mainCam.nearClipPlane));
        Vector3 direction = mousePosition - transform.position;
        Vector3 rotation = transform.position - mousePosition;

        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * force;
        float rotate = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotate);

        Destroy(gameObject, projectileDuration);    
    }
}
