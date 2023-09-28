using UnityEngine;
using UnityEngine.AI;

public class ChasingState : RobotState
{
    private float _waitTime = 1f;
    private float _currentWaitTime = 0f;
    private float _maxBlindChaseTime = 5f;

    private float _currentBlindChaseTime = 0f;

    private Vector3 _lastSeenPos;

    public ChasingState(RobotStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        stateMachine.volumetricLight.SetColor(Color.red);
        stateMachine.movementSystem.StopAgent();
        stateMachine.movementSystem.SetStoppingDistance(1f);
        stateMachine.movementSystem.Agent.updateRotation = false;
        stateMachine.exclamationMark.SetActive(true);
    }

    public override void Tick()
    {
        if (_currentWaitTime < _waitTime)
        {
            _currentWaitTime += Time.deltaTime;
            LookTowardsPlayer();
        }
        else
        {
            // Establece todo apenas inicia a perseguirlo
            if (stateMachine.movementSystem.Agent.isStopped)
            {
                stateMachine.movementSystem.SetSpeed(2f);
                stateMachine.animationSystem.EnableHeadIk(true);
                stateMachine.exclamationMark.SetActive(false);
            }

            float dist = Vector3.Distance(stateMachine.transform.position, SceneData.Instance.Player.transform.position);

            // Claramente solo le va a disparar si lo tiene a la vista
            if (!stateMachine.detectionSystem.IsPlayerBlocked())
            {
                LookTowardsPlayer();
                stateMachine.movementSystem.Agent.updateRotation = false;
                stateMachine.animationSystem.EnableHeadIk(true);
                stateMachine.shootingSystem.Shoot();
                if (!stateMachine.shootingAudio.isPlaying) stateMachine.shootingAudio.Play();
                _lastSeenPos = SceneData.Instance.Player.transform.position;

                _currentBlindChaseTime = 0f;
            }
            else
            {
                stateMachine.movementSystem.Agent.updateRotation = true;
                stateMachine.animationSystem.EnableHeadIk(false);
                stateMachine.shootingAudio.Stop();

                _currentBlindChaseTime += Time.deltaTime;
            }

            // bool isWalkable = NavMesh.SamplePosition(SceneData.Instance.Player.transform.position, out NavMeshHit hit, 0.5f, 1);
            // Si está lo suficientemente lejos lo sigue, sino, entonces para y dispara
            if (/*isWalkable &&*/ (dist >= stateMachine.movementSystem.Agent.stoppingDistance * 5f || stateMachine.detectionSystem.IsPlayerBlocked()))
            {
                stateMachine.movementSystem.MoveTowards(_lastSeenPos);
            }
            else
            {
                stateMachine.movementSystem.StopAgent();
            }

            stateMachine.animationSystem.PlayAnimation();
            base.Tick();
        }
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.shootingAudio.Stop();
        stateMachine.movementSystem.SetStoppingDistance(0f);
        stateMachine.movementSystem.Agent.updateRotation = true;
        stateMachine.movementSystem.SetSpeed(1f);
        stateMachine.animationSystem.EnableHeadIk(false);
        stateMachine.volumetricLight.SetColor(Color.green);

        stateMachine.exclamationMark.SetActive(false);
    }

    protected override void CheckTransitions()
    {
        base.CheckTransitions();

        // Si el jugador pasa por una parte sin navmesh y ya no ve al jugadir
        // if (!NavMesh.SamplePosition(SceneData.Instance.Player.transform.position, out NavMeshHit hit, 0.5f, 0) && stateMachine.detectionSystem.IsPlayerBlocked())
        // {
        //     stateMachine.SetState(new RoamingState(stateMachine));
        // }
        // Si llegó al destino, pero no ve al jugador
        bool hasReachedDestination = stateMachine.movementSystem.Agent.HasReachedDestination();
        if (hasReachedDestination && stateMachine.detectionSystem.IsPlayerBlocked() || _currentBlindChaseTime >= _maxBlindChaseTime)
        {
            stateMachine.SetState(new ConfusedState(stateMachine));
        }
    }

    private void LookTowardsPlayer()
    {
        Vector3 unit = (SceneData.Instance.Player.transform.position - stateMachine.transform.position);
        unit.y = 0f;
        unit.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(unit);
        stateMachine.movementSystem.transform.rotation = Quaternion.Lerp(stateMachine.movementSystem.transform.rotation, targetRotation, Time.deltaTime * 5f);
    }
}