using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class AnimationSystem : MonoBehaviour
{
    [SerializeField] private Rig _headIk;
    private Animator _animator;
    private NavMeshAgent _agent;

    private int _bodySpeedHash = Animator.StringToHash("BodySpeed");
    private int _wheelsSpeedHash = Animator.StringToHash("WheelsSpeed");
    private int _killedHash = Animator.StringToHash("Killed");

    private float _headIkTarget = 0;

    public void StopAnimation()
    {
        _animator.SetFloat(_bodySpeedHash, 0f);
        _animator.SetFloat(_wheelsSpeedHash, 0f);
    }

    public void PlayAnimation()
    {
        _animator.SetFloat(_bodySpeedHash, 1);
        _animator.SetFloat(_wheelsSpeedHash, _agent.velocity.magnitude / _agent.speed);
    }

    public void EnableHeadIk(bool enable)
    {
        _headIkTarget = enable ? 1f : 0f;
    }

    public void PlayDeathAnimation()
    {
        _animator.SetTrigger(_killedHash);
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Mathf.Abs(_headIk.weight - _headIkTarget) > 0.01f)
        {
            _headIk.weight = Mathf.Lerp(_headIk.weight, _headIkTarget, Time.deltaTime * 5f);
        }
    }
}