using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum RoamingEnumState { Walking, Idle, Rotating }

public class MovementSystem : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _speed = 2f;

    [Header("Roaming")]
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float _idleTime = 1f;
    [SerializeField] private bool _loop = false;
    [SerializeField] private bool _randomRoaming = false;
    [SerializeField] private Transform _path;

    private List<Transform> _roamingPoints = new List<Transform>();
    private NavMeshAgent _agent;

    private int _currentDirection = 1;
    private int _currentIndex = 0;
    private const float TARGET_ROTATION_THRESHOLD = 5f;

    private RoamingEnumState _lastState = RoamingEnumState.Rotating;
    private RoamingEnumState _currentState = RoamingEnumState.Idle;

    public NavMeshAgent Agent { get => _agent; }

    public void SetStoppingDistance(float stoppingDistance)
    {
        _agent.stoppingDistance = stoppingDistance;
    }

    public void SetSpeed(float multiplier)
    {
        _agent.speed = _speed * multiplier;
    }

    public void MoveTowards(Vector3 position)
    {
        if (!_agent.isOnNavMesh) _agent.Warp(_agent.transform.position);

        _agent.isStopped = false;
        _agent.destination = position;
    }

    public void RoamAround()
    {
        if (_currentState == RoamingEnumState.Idle)
        {
            if (_currentState != _lastState) StartCoroutine(StartIdleTime());

            _lastState = _currentState;
        }
        else if (_currentState == RoamingEnumState.Walking)
        {
            MoveTowards(_roamingPoints[_currentIndex].position);

            _lastState = _currentState;

            if (_agent.HasReachedDestination())
            {
                StopAgent();
                _currentState = RoamingEnumState.Idle;
            }
        }
        else if (_currentState == RoamingEnumState.Rotating)
        {
            if (_currentState != _lastState)
            {
                _currentIndex = GetNextIndex();
            }

            _lastState = _currentState;

            RotateSmoothly();
        }
    }

    public void ResetRoaming()
    {

    }

    public void StopAgent()
    {
        _agent.isStopped = true;
    }

    private IEnumerator StartIdleTime()
    {
        yield return new WaitForSeconds(_idleTime);
        _currentState = RoamingEnumState.Rotating;
    }

    private IEnumerator StartMovingAfterRotation()
    {
        yield return new WaitForSeconds(_idleTime);

        // Revisamos que el agente no se haya parado aproposito
        if (!_agent.isStopped)
            MoveTowards(_roamingPoints[_currentIndex].position);
    }

    private int GetNextIndex()
    {
        int nextPoint;

        // Obtenemos un valor aleatorio que nos da -1 o 1
        if (_randomRoaming) _currentDirection = Mathf.RoundToInt(Mathf.Sign(Random.Range(-1, 1)));

        if (!_loop)
        {
            nextPoint = _currentIndex + _currentDirection;
            if (nextPoint < 0 || nextPoint >= _roamingPoints.Count)
            {
                _currentDirection *= -1;
                nextPoint = _currentIndex + _currentDirection;
            }
        }
        else
            nextPoint = OlimpiadasUtils.ModulusInt((_currentIndex + _currentDirection), _roamingPoints.Count);

        return Mathf.Clamp(nextPoint, 0, _roamingPoints.Count);
    }

    private void RotateSmoothly()
    {
        Vector3 directionToNextTarget = (_roamingPoints[_currentIndex].position - _agent.transform.position);
        directionToNextTarget.y = 0;
        directionToNextTarget.Normalize();

        Quaternion targetRot = Quaternion.LookRotation(directionToNextTarget);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, _rotationSpeed * Time.deltaTime);

        // Si ya está mirando la siguiente posición, entonce dirigirse a ella
        if (Quaternion.Angle(transform.rotation, targetRot) <= TARGET_ROTATION_THRESHOLD)
        {
            StartCoroutine(StartMovingAfterRotation());
            _currentState = RoamingEnumState.Walking;
        }
    }

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _speed;

        foreach (Transform child in _path)
        {
            _roamingPoints.Add(child);
        }
    }
}