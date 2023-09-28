using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotState
{
    protected RobotStateMachine stateMachine;

    public RobotState(RobotStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Tick() { CheckTransitions(); }
    public virtual void Exit() { }

    protected virtual void CheckTransitions() { }
}

public class RobotStateMachine : MonoBehaviour
{
    public bool isEnemyActive = false;

    #region Systems Reference
    public GameObject smoke;
    public AudioSource movingAudio;
    public AudioSource deathAudio;
    public AudioSource shootingAudio;

    [HideInInspector] public MovementSystem movementSystem;
    [HideInInspector] public ShootingSystem shootingSystem;
    [HideInInspector] public DetectionSystem detectionSystem;
    [HideInInspector] public AnimationSystem animationSystem;
    [HideInInspector] public VolumetricLight volumetricLight;
    #endregion

    [HideInInspector] public GameObject questionMark;
    [HideInInspector] public GameObject exclamationMark;
    [HideInInspector] public bool isVulnerable = false;
    private RobotState currentState;

    public void SetState(RobotState state)
    {
        currentState.Exit();
        currentState = state;
        currentState.Enter();
    }

    public void SetupAnimation()
    {
        movingAudio.Stop();
        SetState(new CaughtState(this));
    }

    public void Kill()
    {
        smoke.SetActive(false);
        SetState(new KilledState(this));
        deathAudio.PlayDelayed(1.5f);
    }

    private void Awake()
    {
        // Reference systems
        movementSystem = GetComponent<MovementSystem>();
        shootingSystem = GetComponent<ShootingSystem>();
        detectionSystem = GetComponent<DetectionSystem>();
        animationSystem = GetComponent<AnimationSystem>();
        volumetricLight = GetComponentInChildren<VolumetricLight>();


        questionMark = transform.Find("Symbols/QuestionMark").gameObject;
        exclamationMark = transform.Find("Symbols/ExclamationMark").gameObject;
    }

    private void Start()
    {
        currentState = new RoamingState(this);
        currentState?.Enter();
    }

    private void Update()
    {
        if (isEnemyActive)
        {
            if (!detectionSystem.EarsActive) detectionSystem.SetEarsActive(true);
            currentState?.Tick();
        }
        else
        {
            if (detectionSystem.EarsActive)
                detectionSystem.SetEarsActive(false);
        }
    }
}