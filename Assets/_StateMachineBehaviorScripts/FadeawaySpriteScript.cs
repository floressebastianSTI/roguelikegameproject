using UnityEngine;

public class FadeawaySpriteScript : StateMachineBehaviour
{
    public float fadeTime = 0.5f;
    private float timeElapsed = 0f;

    SpriteRenderer spriteRenderer;
    GameObject objectToRemove;
    Color startColor;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timeElapsed = 0f;
        spriteRenderer = animator.GetComponent<SpriteRenderer>();
        startColor = spriteRenderer.color;
        objectToRemove = animator.gameObject;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timeElapsed += Time.deltaTime;

        float newAlpha = startColor.a * (1 - (timeElapsed / fadeTime));

        spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);

        if (timeElapsed > fadeTime)
        {
            Destroy(objectToRemove);
        }
    }
}