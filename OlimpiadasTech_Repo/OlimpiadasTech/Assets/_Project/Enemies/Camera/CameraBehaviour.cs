using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum HostilCameraState { Active, Stationary, Deactivated, PlayerFound }

public class CameraBehaviour : HackableObject
{
    [SerializeField] private Transform _sight;
    [Header("Surveillance")]
    [SerializeField] private Transform _path;
    [SerializeField] private AnimationCurve _speedCurve;
    [SerializeField] private float _stationaryTime = 1f;
    [SerializeField] private float _speed = 1f;
    [Header("Detection")]
    [SerializeField] private LayerMask _wallsMask;
    [Header("Deactivation")]
    [SerializeField] private float _deactivationTime = 10f;

    private List<Transform> _pathPoints = new List<Transform>();
    private Transform _target;
    private VolumetricLight _spotLight;
    private Animator _anim;

    private HostilCameraState _currentState = HostilCameraState.Active;
    private HostilCameraState _lastState = HostilCameraState.Active;

    private float _currentStationaryTime = 0f;
    private int _currentPoint = 0;
    private float _currentValueInCurve = 0f;
    private float _currentUnactiveTime = 0f;

    // Animation
    private int _deactivaionHash = Animator.StringToHash("Deactivated");
    private int _activationHash = Animator.StringToHash("Activated");

    protected override void OnLoaded_E()
    {
        _lastState = _currentState;
        _currentState = HostilCameraState.Deactivated;
        _anim.SetTrigger(_deactivaionHash);
        base.OnLoaded_E();
    }

    private bool IsPlayerBlocked()
    {
        foreach (Transform plrPoint in SceneData.Instance.PlayerDetectionPointsContainer)
        {
            bool blocked = Physics.Linecast(_sight.position, plrPoint.position, _wallsMask);
            // Si por lo menos uno no está bloqueado, entonces el jugador fue detectado
            if (!blocked) return false;
        }

        return true;
    }

    private bool ReachedDestination(Vector3 currentPos, Vector3 targetPos)
    {
        return Vector3.Distance(currentPos, targetPos) <= 0.01f;
    }

    private void Watch()
    {
        if (_currentState == HostilCameraState.Active)
        {
            Vector3 target = _pathPoints[_currentPoint].position;

            _currentValueInCurve += Time.deltaTime * _speed;
            // El origen es el punto anterior
            Vector3 origin = _pathPoints[Mathf.Abs(_currentPoint - 1)].position;
            _target.position = origin + (target - origin) * _speedCurve.Evaluate(_currentValueInCurve);

            if (ReachedDestination(_target.position, target))
            {
                _currentState = HostilCameraState.Stationary;
                _currentStationaryTime = 0f;
                _currentValueInCurve = 0f;
            }
        }
        else if (_currentState == HostilCameraState.Stationary)
        {
            _currentStationaryTime += Time.deltaTime;

            if (_currentStationaryTime >= _stationaryTime)
            {
                _currentPoint = Mathf.Abs(_currentPoint - 1);
                _currentState = HostilCameraState.Active;

                Vector3 unitToTargetRot = (transform.position - _pathPoints[_currentPoint].position).normalized;
            }
        }

        WatchForPlayer();
    }

    private void WatchForPlayer()
    {
        Vector3 plrPos = SceneData.Instance.Player.transform.position;

        // Angle
        Vector3 unit = (plrPos - _sight.position).normalized;
        Vector3 fwd = _sight.forward;

        float dot = Vector3.Dot(fwd, unit);
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

        bool isInAngle = angle <= _spotLight.SpotLight.spotAngle * 0.5f - 2f;
        bool isInDistance = Vector3.Distance(plrPos, _sight.position) <= _spotLight.SpotLight.range;
        bool isBlocked = IsPlayerBlocked();

        if (isInAngle && isInDistance && !isBlocked)
        {
            _spotLight.SetColor(Color.red);
            _currentState = HostilCameraState.PlayerFound;
            SceneData.Instance.Player.GetComponent<HealthSystem>().Defeat();
        }
    }

    private void TickUnactiveTime()
    {
        _currentUnactiveTime += Time.deltaTime;

        if (_currentUnactiveTime >= _deactivationTime)
        {
            _currentUnactiveTime = 0f;
            _anim.ResetTrigger(_deactivaionHash);
            _anim.SetTrigger(_activationHash);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        _target = transform.Find("Target");
        _spotLight = transform.GetComponentInChildren<VolumetricLight>();
        _anim = GetComponent<Animator>();

        foreach (Transform child in _path)
        {
            _pathPoints.Add(child);
        }
    }

    private void Update()
    {
        if (_currentState == HostilCameraState.Active || _currentState == HostilCameraState.Stationary)
        {
            Watch();
        }
        else if (_currentState == HostilCameraState.Deactivated)
        {
            TickUnactiveTime();
        }
    }

    #region Callbacks
    public void ActivationAnimationEnded()
    {
        _anim.ResetTrigger(_activationHash);
        _currentState = _lastState;
        isActive = true;
    }
    #endregion
}