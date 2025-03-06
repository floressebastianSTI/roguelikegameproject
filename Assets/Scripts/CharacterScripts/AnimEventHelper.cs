using System;
using UnityEngine;
using UnityEngine.Events;
public class AnimEventHelper : MonoBehaviour
{

    public event Action OnAnimationEventTriggered;

    public void TriggerEvent()
    {
        OnAnimationEventTriggered?.Invoke();
    }
}
