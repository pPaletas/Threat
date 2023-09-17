using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoamingState : RobotState
{
    public RoamingState(RobotStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        stateMachine.detectionSystem.SetEnhancedSight(false);
        stateMachine.detectionSystem.SetEarsActive(true);
        stateMachine.movementSystem.SetSpeed(1f);
        stateMachine.movementSystem.SetStoppingDistance(0f);
        stateMachine.isVulnerable = true;
    }

    public override void Tick()
    {
        stateMachine.movementSystem.RoamAround();
        stateMachine.animationSystem.PlayAnimation();
        base.Tick();
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.movementSystem.StopAgent();
        stateMachine.detectionSystem.SetEarsActive(true);
        stateMachine.isVulnerable = false;
    }

    protected override void CheckTransitions()
    {
        base.CheckTransitions();

        if (stateMachine.detectionSystem.GetDetectionResult() != DetectionResult.None)
        {
            stateMachine.SetState(new AlertedState(stateMachine));
        }
    }
}