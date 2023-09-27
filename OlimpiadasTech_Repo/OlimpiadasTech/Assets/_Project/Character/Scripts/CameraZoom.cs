using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using StarterAssets;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private StarterAssetsInputs _input;
    [SerializeField] private float _zoomStrength = 5f;
    [SerializeField] private float _zoomSmoothness = 5f;
    [SerializeField] private Vector2 _zoomLimits = new Vector2(3f, 5f);
    [SerializeField] private LayerMask _maskWithPlayer;
    [SerializeField] private LayerMask _maskWithoutPlayer;
    [SerializeField] private Camera _camera;

    private Cinemachine3rdPersonFollow _cam;
    private float _targetZoom;

    private bool _layerRemoved = false;

    private void Update()
    {
        if (_input.zoom != 0f)
        {
            _targetZoom = _cam.CameraDistance - _input.zoom * _zoomStrength;
            _targetZoom = Mathf.Clamp(_targetZoom, _zoomLimits.x, _zoomLimits.y);
        }

        if (Mathf.Abs(_cam.CameraDistance - _targetZoom) > 0.1f)
        {
            _cam.CameraDistance = Mathf.Lerp(_cam.CameraDistance, _targetZoom, Time.deltaTime * _zoomSmoothness);
        }

        if (_cam.CameraDistance <= 0.7f)
        {
            if (!_layerRemoved)
            {
                _camera.cullingMask = _maskWithoutPlayer;
                _layerRemoved = true;
            }
        }
        else
        {
            if (_layerRemoved)
            {
                _camera.cullingMask = _maskWithPlayer;
                _layerRemoved = false;
            }

        }
    }

    private void Awake()
    {
        _cam = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        _targetZoom = 3f;
    }
}
