using UnityEngine;

public class AttackHandlerScript : MonoBehaviour
{
    public Vector2 PointerPosition { get; set; }

    private void Update()
    {
        transform.right = (PointerPosition - (Vector2)transform.position).normalized;
        Vector2 direction = (PointerPosition - (Vector2)transform.position).normalized;

        Vector2 scale = transform.localScale;
        if (direction.x < 0)
        {
            scale.y = -1;
        }
        else if (direction.x > 0)
        {
            scale.y = 1;
        }
        transform.localScale = scale;
    }
}
