using UnityEngine;

public class ConfusedState : RobotState
{
    private float _lastCheckTime = 0.3f;
    private float _currentCheckTime = 0f;

    private float _confusedTime = 2f;
    private float _currentConfusedTime = 0f;

    public ConfusedState(RobotStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        stateMachine.detectionSystem.SetEarsActive(true);
        stateMachine.questionMark.SetActive(true);
    }

    public override void Tick()
    {
        if (_currentCheckTime < _lastCheckTime)
        {
            // Se va a mover por esos segundos
            stateMachine.movementSystem.MoveTowards(SceneData.Instance.Player.transform.position);
            _currentCheckTime += Time.deltaTime;
        }
        else
        {
            stateMachine.questionMark.SetActive(false);
            stateMachine.movementSystem.StopAgent();
        }

        base.Tick();
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.detectionSystem.SetEarsActive(false);

        stateMachine.questionMark.SetActive(false);
    }

    protected override void CheckTransitions()
    {
        base.CheckTransitions();

        if (stateMachine.detectionSystem.GetSight() == DetectionResult.Direct && !stateMachine.detectionSystem.IsPlayerBlocked())
        {
            stateMachine.SetState(new ChasingState(stateMachine));
        }
        else if (stateMachine.detectionSystem.GetDetectionResult() != DetectionResult.None)
        {
            stateMachine.SetState(new AlertedState(stateMachine));
        }
        // Si no detectó nada, entonces se queda bobito por un rato, y pasa a romear de nuevo xd
        else
        {
            if (_currentCheckTime >= _lastCheckTime && _currentConfusedTime < _confusedTime) _currentConfusedTime += Time.deltaTime;

            if (_currentConfusedTime >= _confusedTime) stateMachine.SetState(new RoamingState(stateMachine));
        }
    }
}
