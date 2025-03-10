using System.Collections;
using UnityEngine;

public class AfterimageEffect : MonoBehaviour
{
    private Transform player;

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

    public Color color;

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
        alpha = alphaMultiplier;
        color = new Color(1f, 1f, 1f, alpha);
        sprite.color = color;

        if (Time.time >= (timeActivated + activeTime))
        {
            //_AfterimagePool.Instance.AddToPool(gameObject);
        }
    }    
}
