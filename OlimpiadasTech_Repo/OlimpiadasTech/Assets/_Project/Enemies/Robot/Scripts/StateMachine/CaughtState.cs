using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaughtState : RobotState
{
    public CaughtState(RobotStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        stateMachine.GetComponent<Collider>().enabled = false;
        stateMachine.movementSystem.Agent.velocity = Vector3.zero;
        stateMachine.movementSystem.StopAgent();
        stateMachine.animationSystem.StopAnimation();
    }
}

public class KilledState : RobotState
{
    public KilledState(RobotStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        stateMachine.animationSystem.PlayDeathAnimation();
    }
}