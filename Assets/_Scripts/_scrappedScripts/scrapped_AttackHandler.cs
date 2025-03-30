using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System.Collections;
using System;

public class OldAttackScript : MonoBehaviour
{
    public UnityEvent OnAnimationTrigger;
    public void TriggerEvent()
    {
        OnAnimationTrigger?.Invoke();
    }

    private Animator animator;
    private GameObject baseGameObject;

    private int currentAttackCounter;

    [SerializeField]
    private int numberOfAttacks;
    public int CurrentAttackCounter
    {
        get => CurrentAttackCounter;
        private set => currentAttackCounter = value >= numberOfAttacks ? 0 : value;
    }
    public event Action OnExit;
    public void Enter()
    {
        print("SLASH!");

        animator.SetBool("Active", true);
        animator.SetInteger("Counter", currentAttackCounter);
    }
    private void Exit()
    {
        animator.SetBool("Active", false);

        CurrentAttackCounter++;

        OnExit?.Invoke();
    }
    private void Awake()
    {
        animator = baseGameObject.GetComponent<Animator>();
        
    }
}
    
