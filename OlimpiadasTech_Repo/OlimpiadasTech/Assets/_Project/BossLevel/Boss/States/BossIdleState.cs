using UnityEngine;

public class BossSweepState : BossState
{
    private bool _armRaised = false;
    private bool _detectedHitbox = false;

    public BossSweepState(BossStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.animator.SetTrigger("Sweep");
        stateMachine.onAnimationEvent += OnAnimationEvent;

        base.Enter();
    }

    public override void Tick()
    {
        if (_armRaised && !_detectedHitbox)
        {
            float currentHandY = stateMachine.hand.position.y;
            float currentPlayerY = BossLevelSceneData.Instance.Player.transform.position.y;

            if (currentHandY <= currentPlayerY)
            {
                _detectedHitbox = true;
                Debug.Log("HITED");
            }
        }

        base.Tick();
    }

    private void OnAnimationEvent(string obj)
    {
        if (obj == "sweep-finished")
        {
            stateMachine.SetState(new BossIdleState(stateMachine));
        }
        else if (obj == "arm-raised")
        {
            _armRaised = true;
        }
        else if(obj == "arm-downcomoalejo")
        {
            _armRaised = false;
        }
    }
}

public class BossIdleState : BossState
{
    private bool _isRotating = false;
    private Vector3 _targetDir;
    private float _randomIdleTime;

    private float _currentIdleTime = 0f;

    public BossIdleState(BossStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        _randomIdleTime = Random.Range(stateMachine.idleTime.x, stateMachine.idleTime.y);
    }

    public override void Tick()
    {
        RotateTowardsPlayer();
        TickIdleTime();

        base.Tick();
    }

    protected override void CheckTransitions()
    {
        if (_currentIdleTime >= _randomIdleTime)
        {
            stateMachine.SetState(new BossSweepState(stateMachine));
        }
    }

    private void CheckIfShouldRotate()
    {
        Vector3 unit = (BossLevelSceneData.Instance.Player.transform.position - stateMachine.head.position).normalized;
        unit.y = 0f;
        unit.Normalize();

        Vector3 fwd = stateMachine.transform.forward;
        fwd.y = 0f;
        fwd.Normalize();

        float dot = Vector3.Dot(fwd, unit);
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

        if (angle >= 30f)
        {
            _isRotating = true;
            _targetDir = unit;
        }
    }

    private void RotateTowardsPlayer()
    {
        Vector3 unit = (BossLevelSceneData.Instance.Player.transform.position - stateMachine.transform.position).normalized;
        unit.y = 0f;
        unit.Normalize();

        Quaternion targetRot = Quaternion.LookRotation(unit);
        stateMachine.transform.rotation = Quaternion.Slerp(stateMachine.transform.rotation, targetRot, Time.deltaTime * stateMachine.rotationSmoothness);

        Vector3 fwd = stateMachine.head.forward;
        fwd.y = 0f;
        fwd.Normalize();

        float dot = Vector3.Dot(fwd, stateMachine.transform.forward);
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

        if (angle <= 0.5f)
        {
            _isRotating = false;
        }
    }

    private void TickIdleTime()
    {
        _currentIdleTime += Time.deltaTime;
    }
}