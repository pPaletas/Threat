using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class BossState
{
    protected BossStateMachine stateMachine;

    public BossState(BossStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Tick() { CheckTransitions(); }
    public virtual void LateTick() { }
    public virtual void Exit() { }

    protected virtual void CheckTransitions() { }
}

public class BossStateMachine : MonoBehaviour
{
    public Action<string> onAnimationEvent;

    public Rig headRig;

    [Header("Idle")]
    public Transform head;
    public float rotationSmoothness = 5f;
    public Vector2 idleTime = new Vector2(3f, 5f);

    [Header("Summon")]
    public GameObject enemyPrefab;
    public Transform enemiesContainer;
    public int enemiesCount = 10;

    [Header("Rings")]
    public GameObject ringPrefab;
    public Transform ringsContainer;
    public float spawnFreq = 0.2f;
    public float ringScaleSpeed = 10f;
    public float randomAngleSpawn = 30f;

    [Header("Explosive entities")]
    public GameObject explosiveEntityPrefab;
    public Transform explosiveEntitiesContainer;
    public Transform center;
    public int entitiesAmount = 10;

    public int currentState = 0;

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

    private void LateUpdate()
    {
        _currentState?.LateTick();
    }
}