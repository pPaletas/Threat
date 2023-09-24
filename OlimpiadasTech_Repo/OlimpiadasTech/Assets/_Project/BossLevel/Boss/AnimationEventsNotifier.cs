using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventsNotifier : MonoBehaviour
{
    private BossStateMachine _stateMachine;

    public void OnAnimationNotification(string animationName)
    {
        _stateMachine.NotifyAnimationFinished(animationName);
    }

    private void Awake()
    {
        _stateMachine = GetComponentInParent<BossStateMachine>();
    }
}