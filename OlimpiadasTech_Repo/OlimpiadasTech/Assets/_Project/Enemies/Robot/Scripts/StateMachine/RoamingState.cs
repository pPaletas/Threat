using System.Collections;
using System.Collections.Generic;

public class RoamingState : RobotState
{
    public RoamingState(RobotStateMachine stateMachine) : base(stateMachine) { }

    public override void Tick()
    {
        stateMachine.movementSystem.RoamAround();
        base.Tick();
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.movementSystem.StopAgent();
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