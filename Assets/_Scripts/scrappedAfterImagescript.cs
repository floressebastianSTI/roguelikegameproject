using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AfterimageEffect : MonoBehaviour
{
    private Transform player;

    [SerializeField]
    private InputActionReference pointerPosition;

    [SerializeField]
    private float activeTime = 0.2f;

    private float timeActivated;
    private float alpha;

    [SerializeField]
    private float alphaSet = 0.1f;

    [SerializeField]
    private float alphaMultiplier = 0.85f;

    private SpriteRenderer sprite;
    private SpriteRenderer playerSprite;

    private Vector2 pointerInput;

    public Color color;

    private Vector2 GetPointerInput()
    {
        Vector3 mousePosition = pointerPosition.action.ReadValue<Vector2>();
        mousePosition.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    private void Awake()
    {
        sprite.flipX = false;
    }
    private void OnEnable()
    {
        sprite = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerSprite = player.GetComponent<SpriteRenderer>();

        alpha = alphaSet;
        sprite.sprite = playerSprite.sprite;
        transform.position = player.position;
        transform.rotation = player.rotation;
        timeActivated = Time.time;
    }

    private void Update()
    {
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, 0));
        mousePosition.z = 0;
        if (mouseScreenPosition != Vector2.zero)
        {
            bool isMouseRight = mousePosition.x > transform.position.x;
            sprite.flipX = !isMouseRight;
        }
            alpha = alphaMultiplier;
        color = new Color(1f, 1f, 1f, alpha);
        sprite.color = color;

        if (Time.time >= (timeActivated + activeTime))
        {
            //_AfterimagePool.Instance.AddToPool(gameObject);
        }
    }    
}
