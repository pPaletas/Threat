using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotState
{
    protected RobotStateMachine stateMachine;

    public RobotState(RobotStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Tick() { CheckTransitions(); }
    public virtual void Exit() { }

    protected virtual void CheckTransitions() { }
}

public class RobotStateMachine : MonoBehaviour
{
    #region Systems Reference
    [HideInInspector] public MovementSystem movementSystem;
    [HideInInspector] public ShootingSystem shootingSystem;
    [HideInInspector] public DetectionSystem detectionSystem;
    #endregion

    private RobotState currentState;

    public void SetState(RobotState state)
    {
        currentState.Exit();
        currentState = state;
        currentState.Enter();
    }

    private void Awake()
    {
        // Reference systems
        movementSystem = GetComponent<MovementSystem>();
        shootingSystem = GetComponent<ShootingSystem>();
        detectionSystem = GetComponent<DetectionSystem>();

        currentState = new RoamingState(this);
        currentState?.Enter();
    }

    private void Update()
    {
        currentState?.Tick();
    }
}