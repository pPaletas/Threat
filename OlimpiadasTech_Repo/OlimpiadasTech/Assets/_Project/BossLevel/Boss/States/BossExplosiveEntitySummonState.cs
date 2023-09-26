using UnityEngine;

public class BossExplosiveEntitySummonState : BossState
{
    public BossExplosiveEntitySummonState(BossStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        stateMachine.animator.SetTrigger("Summon");

        stateMachine.onAnimationEvent += OnAnimationEvent;
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.onAnimationEvent -= OnAnimationEvent;
    }

    private void SummonEntities()
    {
        for (int i = 0; i < stateMachine.entitiesAmount; i++)
        {
            float randomX = Random.Range(-180, 180);
            float randomY = Random.Range(-180f, 180f);

            float randomDist = Random.Range(100f, 250f);
            Vector3 spawnDirection = Quaternion.AngleAxis(randomX, Vector3.right) * Quaternion.AngleAxis(randomY, Vector3.up) * stateMachine.center.forward;

            Vector3 spawnPoint = stateMachine.center.position + spawnDirection * randomDist;

            GameObject clone = GameObject.Instantiate(stateMachine.explosiveEntityPrefab, stateMachine.explosiveEntitiesContainer);
            clone.transform.position = spawnPoint;
        }
    }

    #region Callbacks
    private void OnAnimationEvent(string obj)
    {
        if (obj == "summon-finished")
        {
            stateMachine.SetState(new BossIdleState(stateMachine));
        }
        else if (obj == "summon-started")
        {
            SummonEntities();
        }
    }
    #endregion
}
