using System;
using UnityEngine;

public class BossEnemiesSummonState : BossState
{
    public BossEnemiesSummonState(BossStateMachine stateMachine) : base(stateMachine) { }

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

    private void SummonEnemies()
    {
        for (int i = 0; i < stateMachine.enemiesCount; i++)
        {
            Vector3 unit = (BossLevelSceneData.Instance.Player.transform.position - stateMachine.enemiesContainer.position);
            unit.y = 0f;
            unit.Normalize();

            float randomAngle = UnityEngine.Random.Range(-45f, 45f);
            float randomDist = UnityEngine.Random.Range(100f, 200f);

            Vector3 rotatedUnit = Quaternion.AngleAxis(randomAngle, Vector3.up) * unit;
            rotatedUnit *= randomDist;

            GameObject clone = GameObject.Instantiate(stateMachine.enemyPrefab, stateMachine.enemiesContainer, false);
            clone.transform.localPosition += rotatedUnit;

            float randomSpawnAngle = UnityEngine.Random.Range(-180f, 180f);

            clone.transform.rotation = Quaternion.AngleAxis(randomSpawnAngle, Vector3.up) * clone.transform.rotation;
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
            SummonEnemies();
        }
    }
    #endregion
}
