using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum DroneState { Moving, Stationary, ChasingTarget }

public class Drone : MonoBehaviour
{
    [Header("Roaming state")]
    [SerializeField] private float _height = 4f;
    [SerializeField] Transform _path;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private AnimationCurve _speedCurve;
    [SerializeField] private float _rotationAmount = 5f;
    [SerializeField] private float _sineAmplitude = 0.5f;
    [SerializeField] private float _sineFrequency = 5f;
    [Header("Stationary state")]
    [SerializeField] private float _stationaryTime = 2f;
    [Header("Chasing state")]
    [SerializeField] private GameObject _explosionVFX;
    [SerializeField] private float _explosionRadius = 5f;
    [SerializeField] private float _chaseSpeed = 10f;
    [SerializeField] private float _aggressiveness = 10f;
    [SerializeField] private float _aggressivenessAngle = 25f;
    [SerializeField] private float _preExplosionTime = 3f;

    [Header("Player check")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _plrLayer;
    [SerializeField] private Light _light;

    private List<Transform> _roamingPoints = new List<Transform>();

    private DroneState _currentState = DroneState.Moving;

    private Transform _target;

    private int _currentPoint = 0;
    private float _currentStationaryTime = 0f;
    private float _currentValueInCurve = 0f;
    private float _currentRotation = 0f;
    private float _originalYPos;
    private float _currentPreExplosionTime = 0f;
    private bool _exploded = false;

    private float _debugRadius;
    private float _debugDistance;

    private bool ReachedDestination(Vector3 currentPos, Vector3 targetPos)
    {
        return Vector3.Distance(currentPos, targetPos) <= 0.01f;
    }

    private float MapCurveToRotation(float curveValue)
    {
        float relativeValue = Mathf.Sign(curveValue - 0.5f);
        relativeValue = Mathf.Clamp(relativeValue, 0f, 1f);

        return Mathf.Abs(curveValue - relativeValue);
    }

    private void RoamAround()
    {
        Vector3 currentVel = Vector3.zero;

        if (_currentState == DroneState.Moving)
        {
            Vector3 target = _roamingPoints[_currentPoint].position + Vector3.up * _height;

            _currentValueInCurve += Time.deltaTime * _speed;
            // El origen es el punto anterior
            Vector3 origin = _roamingPoints[Mathf.Abs(_currentPoint - 1)].position + Vector3.up * _height;
            transform.position = origin + (target - origin) * _speedCurve.Evaluate(_currentValueInCurve);

            // Rotación
            Vector3 pathUnit = (target - origin).normalized;
            Vector3 rotationAxis = Vector3.Cross(Vector3.up, pathUnit);

            float rotationValue = MapCurveToRotation(_speedCurve.Evaluate(_currentValueInCurve));
            transform.rotation = Quaternion.AngleAxis(_rotationAmount * rotationValue, rotationAxis) * Quaternion.identity;

            if (ReachedDestination(transform.position, target))
            {
                _currentState = DroneState.Stationary;
                _currentStationaryTime = 0f;
                _currentValueInCurve = 0f;
                _currentRotation = 0f;
            }
        }
        else if (_currentState == DroneState.Stationary)
        {
            _currentStationaryTime += Time.deltaTime;

            if (_currentStationaryTime >= _stationaryTime)
            {
                _currentPoint = Mathf.Abs(_currentPoint - 1);
                _currentState = DroneState.Moving;
            }
        }

        SineMovement();
        CheckForPlayer();
    }

    private void ChaseTarget()
    {
        if (_target == null) return;

        if (_currentPreExplosionTime < _preExplosionTime)
        {
            _currentPreExplosionTime += Time.deltaTime;

            transform.Rotate(Vector3.up * Time.deltaTime * _aggressiveness * 100f);

            float rotation = Mathf.Sin(Time.time * _aggressiveness) * _aggressivenessAngle;
            _light.transform.parent.localRotation = Quaternion.AngleAxis(rotation, Vector3.right) * Quaternion.identity;

            Vector3 targetPos = _target.position;

            Vector3 unit = (transform.position - targetPos).normalized;

            targetPos += unit;

            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * _chaseSpeed);
        }
        else
        {
            bool isCloseEnough = Vector3.Distance(_target.position, transform.position) <= _explosionRadius;

            if (isCloseEnough && _target.parent.TryGetComponent<HealthSystem>(out HealthSystem health))
                health.TakeDamage(33f);

            _explosionVFX.transform.SetParent(null);
            _explosionVFX.SetActive(true);

            GameObject.Destroy(gameObject, 0.1f);
            gameObject.SetActive(false);
        }
    }

    private void SineMovement()
    {
        Vector3 pos = transform.position;
        pos.y = _originalYPos + _sineAmplitude * Mathf.Sin(Time.time * _sineFrequency);

        transform.position = pos;
    }

    private void CheckForPlayer()
    {
        // Primero analizamos la situación
        RaycastHit hit;

        bool hitsGround = Physics.Raycast(_light.transform.position, _light.transform.forward, out hit, _originalYPos * 1.5f, _groundLayer);

        if (!hitsGround) return;

        float sphereCastRadius = Mathf.Tan((_light.spotAngle * 0.5f) * Mathf.Deg2Rad) * hit.distance;
        sphereCastRadius -= 0.05f;

        _debugRadius = sphereCastRadius;
        _debugDistance = hit.distance;

        bool hitsPlayer = Physics.SphereCast(_light.transform.position, sphereCastRadius, _light.transform.forward, out RaycastHit hit1, hit.distance, _plrLayer);

        if (hitsPlayer)
        {
            transform.rotation = Quaternion.identity;
            _currentState = DroneState.ChasingTarget;
            _light.gameObject.SetActive(false);
            _target = SceneData.Instance.Player.transform.Find("Center");
            _light.transform.parent.GetComponent<Animator>().enabled = false;
        }
    }

    private void Awake()
    {
        foreach (Transform point in _path)
        {
            _roamingPoints.Add(point);
        }

        _originalYPos = transform.position.y;
    }

    private void Update()
    {
        if (_currentState != DroneState.ChasingTarget)
        {
            RoamAround();
        }
    }

    private void LateUpdate()
    {
        if (_currentState == DroneState.ChasingTarget)
        {
            ChaseTarget();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, _light.transform.forward * _debugDistance);
        Gizmos.DrawWireSphere(transform.position + _light.transform.forward * _debugDistance, _debugRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
}