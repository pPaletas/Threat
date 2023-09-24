using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossState
{
    protected BossStateMachine stateMachine;

    public BossState(BossStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Tick() { CheckTransitions(); }
    public virtual void Exit() { }

    protected virtual void CheckTransitions() { }
}

public class BossStateMachine : MonoBehaviour
{
    public Action<string> onAnimationEvent;

    [Header("Idle state")]
    public Transform head;
    public float rotationSmoothness = 5f;
    public Vector2 idleTime = new Vector2(3f, 5f);

    [Header("Sweep")]
    public float sweepRotationOffset = 0f;
    public Transform hand;

    [HideInInspector] public Animator animator;

    private BossState _currentState;

    public void SetState(BossState state)
    {
        _currentState.Exit();
        _currentState = state;
        _currentState.Enter();
    }
    
    public void NotifyAnimationFinished(string animationName)
    {
        onAnimationEvent?.Invoke(animationName);
    }

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        _currentState = new BossIdleState(this);
        _currentState?.Enter();
    }

    private void Update()
    {
        _currentState?.Tick();
    }
}