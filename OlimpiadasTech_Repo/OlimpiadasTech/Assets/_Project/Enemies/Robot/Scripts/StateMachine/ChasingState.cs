public class ChasingState : RobotState
{
    public ChasingState(RobotStateMachine stateMachine) : base(stateMachine) { }

    // TODO: ARREGLAR ESE BUG QUE CUANDO LO EMPIEZA A PERSEGUIR, HACE UN GESTO RARO
    public override void Tick()
    {
        stateMachine.movementSystem.MoveTowards(SceneData.Instance.Player.transform.position);
        base.Tick();
    }
}