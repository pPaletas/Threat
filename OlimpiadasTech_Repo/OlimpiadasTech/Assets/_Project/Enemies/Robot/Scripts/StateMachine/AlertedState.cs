using UnityEngine;

public class AlertedState : RobotState
{
    private bool _isPlrBlocked;
    private Vector3 _possiblePlrPosition;

    private float _waitTime = 1f;
    private float _checkingTime = 2f;

    private float _currentWaitTime = 0f;
    private float _currentCheckedTime = 0f;

    private bool _spotChecked = false;

    public AlertedState(RobotStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        _isPlrBlocked = stateMachine.detectionSystem.IsPlayerBlocked();
        _possiblePlrPosition = SceneData.Instance.Player.transform.position;
    }

    public override void Tick()
    {
        if (_currentWaitTime < _waitTime) _currentWaitTime += Time.deltaTime;
        else if (!_spotChecked)
        {
            if (_isPlrBlocked)
            {
                MoveToOrigin();
            }
            else
            {
                LookTowardsOrigin();
            }

            base.Tick();
        }
    }

    protected override void CheckTransitions()
    {
        base.CheckTransitions();

        if(_spotChecked)
        {
            _currentCheckedTime += Time.deltaTime;

            if(_currentCheckedTime >= _checkingTime)
            {
                stateMachine.SetState(new RoamingState(stateMachine));
            }
        }

        if(stateMachine.detectionSystem.GetSight() == DetectionResult.Direct)
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

        if (Quaternion.Angle(stateMachine.transform.rotation, targetRotation) <= 5f) _spotChecked = true;
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