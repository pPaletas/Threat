using UnityEngine;

public class AlertedState : RobotState
{
    private bool _isPlrBlocked;
    private Vector3 _possiblePlrPosition;

    private float _waitTime = 1f;
    private float _checkingTime = 2f;

    private float _currentWaitTime = 0f;
    private float _currentCheckedTime = 0f;

    private bool _spotInSight = false;

    public AlertedState(RobotStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        stateMachine.detectionSystem.SetEnhancedSight(true);
        _isPlrBlocked = stateMachine.detectionSystem.IsPlayerBlocked();
        _possiblePlrPosition = SceneData.Instance.Player.transform.position;
        stateMachine.animationSystem.StopAnimation();
        stateMachine.isVulnerable = true;
        stateMachine.questionMark.SetActive(true);
    }

    public override void Tick()
    {
        if (_currentWaitTime < _waitTime) _currentWaitTime += Time.deltaTime;
        else if (!_spotInSight)
        {
            stateMachine.questionMark.SetActive(false);
            if (_isPlrBlocked)
            {
                MoveToOrigin();
                stateMachine.animationSystem.PlayAnimation();
            }
            else
            {
                LookTowardsOrigin();
            }

        }
        base.Tick();
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.isVulnerable = false;

        // En caso de que por alguna razón no se haya desactivado
        stateMachine.questionMark.SetActive(false);
    }

    protected override void CheckTransitions()
    {
        base.CheckTransitions();

        if (_spotInSight)
        {
            _currentCheckedTime += Time.deltaTime;

            if (_currentCheckedTime >= _checkingTime)
            {
                stateMachine.SetState(new RoamingState(stateMachine));
            }
        }

        if (stateMachine.detectionSystem.GetSight() == DetectionResult.Direct && !stateMachine.detectionSystem.IsPlayerBlocked())
        {
            stateMachine.SetState(new ChasingState(stateMachine));
        }
    }

    private void LookTowardsOrigin()
    {
        Vector3 unit = (_possiblePlrPosition - stateMachine.transform.position);
        unit.y = 0f;
        unit.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(unit);
        stateMachine.movementSystem.transform.rotation = Quaternion.Lerp(stateMachine.movementSystem.transform.rotation, targetRotation, Time.deltaTime * 5f);

        if (Quaternion.Angle(stateMachine.transform.rotation, targetRotation) <= 5f) _spotInSight = true;
    }

    private void MoveToOrigin()
    {
        Vector3 offset = Vector3.up;

        if (stateMachine.detectionSystem.IsBlocked(_possiblePlrPosition + offset))
        {
            stateMachine.movementSystem.MoveTowards(_possiblePlrPosition);
        }
        else
        {
            stateMachine.movementSystem.StopAgent();
            LookTowardsOrigin();
        }
    }
}