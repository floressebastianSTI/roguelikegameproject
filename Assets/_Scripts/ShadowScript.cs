using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ShadowEffect : MonoBehaviour
{
    [Header("Shadow Settings")]
    [SerializeField] private Color shadowColor = new Color(0f, 0f, 0f, 0.5f);
    [SerializeField] private Vector3 shadowOffset = new Vector3(0f, -0.2f, 0f); // Now only vertical offset
    [SerializeField] private Vector3 shadowScale = new Vector3(1f, 1f, 1f);
    [SerializeField] private float shadowRotation = 180f; // Flip upside down
    [SerializeField] private bool matchXPosition = true; // Shadow follows X movement
    [SerializeField] private float yPositionOffset = -0.1f; // How much below the player

    private GameObject shadowObject;
    private SpriteRenderer originalRenderer;
    private SpriteRenderer shadowRenderer;
    private Animator originalAnimator;
    private Animator shadowAnimator;

    void Start()
    {
        // Get the original components
        originalRenderer = GetComponent<SpriteRenderer>();
        originalAnimator = GetComponent<Animator>();

        // Create the shadow object
        shadowObject = new GameObject("Shadow");
        shadowObject.transform.SetParent(transform);
        shadowObject.transform.localPosition = shadowOffset;
        shadowObject.transform.localRotation = Quaternion.Euler(0, 0, shadowRotation);
        shadowObject.transform.localScale = shadowScale;

        // Add and configure the shadow's SpriteRenderer
        shadowRenderer = shadowObject.AddComponent<SpriteRenderer>();
        shadowRenderer.sprite = originalRenderer.sprite;
        shadowRenderer.color = shadowColor;
        shadowRenderer.sortingLayerID = originalRenderer.sortingLayerID;
        shadowRenderer.sortingOrder = originalRenderer.sortingOrder - 1;

        // Copy the animator if it exists
        if (originalAnimator != null)
        {
            shadowAnimator = shadowObject.AddComponent<Animator>();
            shadowAnimator.runtimeAnimatorController = originalAnimator.runtimeAnimatorController;
            shadowAnimator.avatar = originalAnimator.avatar;
            shadowAnimator.applyRootMotion = originalAnimator.applyRootMotion;
        }

        // Initial flip setup
        UpdateShadowFlip();
    }

    void LateUpdate()
    {
        if (shadowRenderer == null || originalRenderer == null) return;

        // Update shadow sprite to match original
        shadowRenderer.sprite = originalRenderer.sprite;

        // Update position - stays below player regardless of movement
        Vector3 newPosition = shadowObject.transform.localPosition;
        newPosition.y = yPositionOffset;
        if (matchXPosition)
        {
            newPosition.x = shadowOffset.x;
        }
        shadowObject.transform.localPosition = newPosition;

        // Match the original's animation state
        if (shadowAnimator != null && originalAnimator != null)
        {
            shadowAnimator.Play(originalAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0,
                               originalAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
        }

        UpdateShadowFlip();
    }

    private void UpdateShadowFlip()
    {
        // Match the X flip but keep the shadow upside down
        shadowRenderer.flipX = originalRenderer.flipX;
        shadowRenderer.flipY = !originalRenderer.flipY; // Additional vertical flip
    }

    void OnDestroy()
    {
        if (shadowObject != null)
        {
            Destroy(shadowObject);
        }
    }
}