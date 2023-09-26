using System;
using System.Collections.Generic;
using UnityEngine;

public class BossRingsState : BossState
{
    private float _maxYValue = 270f;
    private bool _hasExploded = false;
    private bool _hasRingsExploded = false;

    private float _timeSinceLastSpawned = 0f;

    private List<Transform> _toDispose = new List<Transform>();
    private bool _dispose = false;

    public BossRingsState(BossStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        stateMachine.animator.SetTrigger("Explosion");

        stateMachine.onAnimationEvent += OnAnimationEvent;

        _timeSinceLastSpawned = stateMachine.spawnFreq;
    }

    public override void Tick()
    {
        if (!_hasExploded)
        {
            SpawnRings();

            DisposeDestroyedObjects();
        }
        else if (!_hasRingsExploded)
        {
            ExplodeRings();
            stateMachine.ringsContainer.DetachChildren();
            _hasRingsExploded = true;
        }

        base.Tick();
    }

    public override void Exit()
    {
        base.Exit();
        stateMachine.onAnimationEvent -= OnAnimationEvent;
    }

    private void SpawnRings()
    {
        if (_timeSinceLastSpawned >= stateMachine.spawnFreq)
        {
            Vector3 unit = (BossLevelSceneData.Instance.Player.transform.position - stateMachine.ringsContainer.position);
            unit.y = 0f;
            unit.Normalize();

            float randomAngle = UnityEngine.Random.Range(-stateMachine.randomAngleSpawn, stateMachine.randomAngleSpawn);

            Quaternion rot = Quaternion.AngleAxis(randomAngle, Vector3.up) * Quaternion.LookRotation(unit, Vector3.up);

            GameObject i = GameObject.Instantiate(stateMachine.ringPrefab, stateMachine.ringsContainer, false);
            i.transform.rotation = rot;

            _timeSinceLastSpawned = 0f;
        }
        else
        {
            _timeSinceLastSpawned += Time.deltaTime;
        }
    }

    private void DisposeDestroyedObjects()
    {
        if (_dispose)
        {
            for (int i = 0; i < _toDispose.Count; i++)
            {
                GameObject.Destroy(_toDispose[i].gameObject);
            }

            _toDispose.Clear();
            _dispose = false;
        }
    }

    private void ExplodeRings()
    {
        foreach (Transform ring in stateMachine.ringsContainer)
        {
            ring.GetComponent<Ring>().Explode();
        }
    }

    private void OnAnimationEvent(string obj)
    {
        if (obj == "explosion-finished")
        {
            stateMachine.SetState(new BossIdleState(stateMachine));
        }
        else if (obj == "explosion-started")
        {
            _hasExploded = true;
        }
    }

}