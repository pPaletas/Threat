using UnityEngine;

public class BossIdleState : BossState
{
    private bool _isRotating = false;
    private Vector3 _targetDir;
    private float _randomIdleTime;

    private float _currentIdleTime = 0f;

    public BossIdleState(BossStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        stateMachine.headRig.weight = 1f;
        _randomIdleTime = Random.Range(stateMachine.idleTime.x, stateMachine.idleTime.y);
    }

    public override void Tick()
    {
        stateMachine.headRig.weight = 1f;
        RotateTowardsPlayer();
        TickIdleTime();

        base.Tick();
    }

    public override void Exit()
    {
        base.Exit();

        stateMachine.headRig.weight = 0f;
    }

    protected override void CheckTransitions()
    {

        if (_currentIdleTime >= _randomIdleTime)
        {
            int randomNum = stateMachine.currentState < 3 ? stateMachine.currentState : Random.Range(0, 3);

            if (randomNum == 0)
            {
                stateMachine.SetState(new BossEnemiesSummonState(stateMachine));
            }
            else if (randomNum == 1)
            {
                stateMachine.SetState(new BossRingsState(stateMachine));
            }
            else
            {
                stateMachine.SetState(new BossExplosiveEntitySummonState(stateMachine));
            }

            stateMachine.currentState++;
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
        if (stateMachine.enemiesContainer.childCount <= 0 && stateMachine.explosiveEntitiesContainer.childCount <= 0)
            _currentIdleTime += Time.deltaTime;
    }
}