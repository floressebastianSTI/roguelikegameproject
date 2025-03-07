using UnityEngine;

public class AnimationEventScript : MonoBehaviour
{
    public void TriggerEvent()
    {
        AttackScript attackScript = GetComponentInParent<AttackScript>();
        if (attackScript != null)
        {
            attackScript.TriggerEvent();
        }
    }
}