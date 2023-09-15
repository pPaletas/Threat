using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DetectionResult { None, Indirect, Direct }

public class DetectionSystem : MonoBehaviour
{
    public Slider temp_Noisebar;

    [Header("Sight")]
    [SerializeField] private Transform _detectionPivot;
    [SerializeField] private VolumetricLight _light;
    [SerializeField] private float _detectionRange;
    [SerializeField] private float _detectionAngle;
    [SerializeField] private float _perifericAngle;

    [Header("Blocking")]
    [SerializeField] private LayerMask _wallsMask;
    [SerializeField] private Transform _detectionPointsContainer;

    [Header("Hearing")]
    [SerializeField] private float _hearingTriggerResetTime;

    private float _currentNoiseValue = 0;
    private float _timeSinceLastNoise = 0f;
    private bool _earsActive = true;

    private float _originalRange;
    private float _originalAngle;
    private float _spotLightOriginalRange;
    private float _spotLightOriginalAngle;

    public Transform DetectionPivot { get => _detectionPivot; }

    #region Main functionality

    public DetectionResult GetDetectionResult()
    {
        bool isPlrClose = IsPlayerCloseEnough();
        DetectionResult sight = GetSight();
        bool isBlocked = IsPlayerBlocked();
        bool triggeredBySound = _currentNoiseValue >= 100;

        // Detección directa
        if (isPlrClose && sight == DetectionResult.Direct && !isBlocked)
        {
            // Reiniciamos el ruido
            _currentNoiseValue = 0f;
            _timeSinceLastNoise = 0f;
            return DetectionResult.Direct;
        }
        else if ((isPlrClose && sight == DetectionResult.Indirect && !isBlocked) || triggeredBySound)
        {
            _currentNoiseValue = 0f;
            _timeSinceLastNoise = 0f;
            return DetectionResult.Indirect;
        }

        return DetectionResult.None;
    }

    public bool IsPlayerBlocked()
    {
        foreach (Transform point in _detectionPointsContainer)
        {
            foreach (Transform plrPoint in SceneData.Instance.PlayerDetectionPointsContainer)
            {
                bool blocked = Physics.Linecast(point.position, plrPoint.position, _wallsMask);

                Color lineColor = Color.red;
                if (!blocked) lineColor = Color.green;

                Debug.DrawLine(point.position, plrPoint.position, lineColor);

                // Si por lo menos uno no está bloqueado, entonces el jugador fue detectado
                if (!blocked) return false;
            }
        }

        return true;
    }

    public DetectionResult GetSight()
    {
        Vector3 unit = (SceneData.Instance.Player.transform.position - transform.position);
        unit.y = 0f;
        unit.Normalize();

        Vector3 fwd = transform.forward;
        fwd.y = 0f;
        fwd.Normalize();

        float dot = Vector3.Dot(fwd, unit);
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

        if (angle <= _detectionAngle)
        {

            return DetectionResult.Direct;
        }
        else if (angle <= _perifericAngle)
        {
            return DetectionResult.Indirect;
        }

        return DetectionResult.None;
    }

    public void TriggerEars(float triggerValue)
    {
        if (!_earsActive) return;
        _timeSinceLastNoise = 0f;
        _currentNoiseValue = Mathf.Clamp(_currentNoiseValue + triggerValue, 0, 100);
    }

    public void SetEarsActive(bool active)
    {
        _earsActive = active;
        _currentNoiseValue = 0;
        _timeSinceLastNoise = 0;
    }

    public void SetEnhancedSight(bool active)
    {
        float multiplier = 1.5f;

        if (active)
        {
            _detectionAngle = _originalAngle * multiplier;
            _detectionRange = _originalRange * multiplier;

            _light.SpotLight.spotAngle = _spotLightOriginalAngle * multiplier;
            _light.SpotLight.innerSpotAngle = _spotLightOriginalAngle * multiplier;
            _light.SpotLight.range = _spotLightOriginalRange * multiplier;
        }
        else
        {
            _detectionAngle = _originalAngle;
            _detectionRange = _originalRange;

            _light.SpotLight.spotAngle = _spotLightOriginalAngle;
            _light.SpotLight.innerSpotAngle = _spotLightOriginalAngle;
            _light.SpotLight.range = _spotLightOriginalRange;
        }
    }
    #endregion

    #region Utils
    public bool IsBlocked(Vector3 pos)
    {
        bool blocked = Physics.Linecast(transform.position + Vector3.up, pos, _wallsMask);

        if (!blocked) return false;

        return true;
    }
    #endregion

    private bool IsPlayerCloseEnough()
    {
        return Vector3.Distance(SceneData.Instance.Player.transform.position, transform.position) <= _detectionRange;
    }

    // Reinicia el ruido si no se oye nada por un tiempo
    private void CalmDown()
    {
        if (_currentNoiseValue > 0)
        {
            _timeSinceLastNoise += Time.deltaTime;

            if (_timeSinceLastNoise >= _hearingTriggerResetTime)
            {
                _currentNoiseValue = 0;
            }
        }
        else
        {
            _timeSinceLastNoise = 0f;
        }
    }

    private void Awake()
    {
        _originalAngle = _detectionAngle;
        _originalRange = _detectionRange;
        _spotLightOriginalAngle = _light.SpotLight.spotAngle;
        _spotLightOriginalRange = _light.SpotLight.range;
    }

    private void Update()
    {
        CalmDown();
        // temp_Noisebar.value = Mathf.Lerp(temp_Noisebar.value, _currentNoiseValue / 100, Time.deltaTime * 5);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(_detectionPivot.position, Quaternion.AngleAxis(_detectionAngle, _detectionPivot.up) * _detectionPivot.forward * _detectionRange);
        Gizmos.DrawRay(_detectionPivot.position, Quaternion.AngleAxis(-_detectionAngle, _detectionPivot.up) * _detectionPivot.forward * _detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(_detectionPivot.position, Quaternion.AngleAxis(_perifericAngle, _detectionPivot.up) * _detectionPivot.forward * _detectionRange);
        Gizmos.DrawRay(_detectionPivot.position, Quaternion.AngleAxis(-_perifericAngle, _detectionPivot.up) * _detectionPivot.forward * _detectionRange);
    }
}