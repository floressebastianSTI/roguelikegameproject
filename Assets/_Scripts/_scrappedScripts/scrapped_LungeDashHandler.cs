using System.Collections;
using UnityEngine;

public class LungeDash : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 dashDirection;
    private bool isDashing;

    public float dashDuration = 0.1f; // How long the dash lasts
    public float[] dashSpeeds = { 6f, 4f, 3f }; // Different speeds for each attack
    private float dashSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void PerformDash(Vector2 targetDirection, int attackIndex)
    {
        if (!isDashing)
        {
            dashDirection = targetDirection.normalized;
            dashSpeed = dashSpeeds[Mathf.Clamp(attackIndex, 0, dashSpeeds.Length - 1)];
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        float timer = 0;

        while (timer < dashDuration)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
            timer += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        isDashing = false;
    }
}