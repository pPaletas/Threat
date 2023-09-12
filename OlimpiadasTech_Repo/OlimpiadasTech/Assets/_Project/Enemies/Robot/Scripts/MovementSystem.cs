using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    private int _currentPoint = -1;
    private bool _isReachingDestination = true;
    private bool _isInIdleTime = false;

    private const float TARGET_ROTATION_THRESHOLD = 5f;

    public void SetStoppingDistance(float stoppingDistance)
    {
        _agent.stoppingDistance = stoppingDistance;
    }

    public void MoveTowards(Vector3 position)
    {
        if (!_agent.isOnNavMesh) _agent.Warp(_agent.transform.position);

        _agent.isStopped = false;
        _agent.destination = position;
    }

    public void RoamAround()
    {
        _agent.autoBraking = true;

        if (!_agent.HasReachedDestination())
        {
            _isReachingDestination = true;
            MoveTowards(_roamingPoints[_currentPoint].position);
        }
        else
        {
            // Este bloque se ejecutará solo una vez despues de haber llegado al target
            if (_isReachingDestination)
            {
                _isReachingDestination = false;
                _currentPoint = GetNextIndex();
                StartCoroutine(StartIdleTime());
            }

            if (!_isInIdleTime) RotateSmoothly();
        }
    }

    public void StopAgent()
    {
        _agent.isStopped = true;
    }

    private IEnumerator StartIdleTime()
    {
        _isInIdleTime = true;
        yield return new WaitForSeconds(_idleTime);
        _isInIdleTime = false;
    }

    private IEnumerator StartMovingAfterRotation()
    {
        yield return new WaitForSeconds(_idleTime);

        // Revisamos que el agente no se haya parado aproposito
        if (!_agent.isStopped)
            MoveTowards(_roamingPoints[_currentPoint].position);
    }


    private int GetNextIndex()
    {
        int nextPoint;

        // Obtenemos un valor aleatorio que nos da -1 o 1
        if (_randomRoaming) _currentDirection = Mathf.RoundToInt(Mathf.Sign(Random.Range(-1, 1)));

        if (!_loop)
        {
            nextPoint = _currentPoint + _currentDirection;
            if (nextPoint < 0 || nextPoint >= _roamingPoints.Count)
            {
                _currentDirection *= -1;
                nextPoint = _currentPoint + _currentDirection;
            }
        }
        else
            nextPoint = OlimpiadasUtils.ModulusInt((_currentPoint + _currentDirection), _roamingPoints.Count);

        return Mathf.Clamp(nextPoint, 0, _roamingPoints.Count);
    }

    private void RotateSmoothly()
    {
        Vector3 directionToNextTarget = (_roamingPoints[_currentPoint].position - _agent.transform.position);
        directionToNextTarget.y = 0;
        directionToNextTarget.Normalize();

        Quaternion targetRot = Quaternion.LookRotation(directionToNextTarget);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, _rotationSpeed * Time.deltaTime);

        // Si ya está mirando la siguiente posición, entonce dirigirse a ella
        if (Quaternion.Angle(transform.rotation, targetRot) <= TARGET_ROTATION_THRESHOLD)
        {
            StartCoroutine(StartMovingAfterRotation());
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