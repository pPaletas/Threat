using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerPuzzle : MonoBehaviour
{
    [SerializeField] private Vector2 _scaleRange = new Vector2(0.3f, 1f);
    [SerializeField] private float _growthSpeed = 10f;

    private Animator _anim;
    private Vector3 _a, _b;
    private float _targetScale = 1f;
    private float _currentScale = 1f;
    private bool _puzzleStarted = false;

    public void OpenDoor()
    {
        _anim.SetTrigger("Open");
    }

    private void Awake()
    {
        _a = transform.Find("A").position;
        _b = transform.Find("B").position;
        _anim = transform.Find("CheckerDoor").GetComponent<Animator>();
    }

    private void OnTriggerStay(Collider other)
    {
        _puzzleStarted = true;

        float maxDist = Mathf.Abs(_b.z - _a.z);
        float distPercentage = Mathf.Abs(SceneData.Instance.Player.transform.position.z - _a.z) / maxDist;
        float percInScaleRange = _scaleRange.x + ((_scaleRange.y - _scaleRange.x) * distPercentage);
        _targetScale = Mathf.Clamp(percInScaleRange, _scaleRange.x, _scaleRange.y);
    }

    private void OnTriggerExit(Collider other)
    {
        _targetScale = 1f;
    }

    private void LateUpdate()
    {
        if (_puzzleStarted)
        {
            _currentScale = Mathf.Lerp(_currentScale, _targetScale, Time.deltaTime * _growthSpeed);
            SceneData.Instance.Player.transform.localScale = Vector3.one * _currentScale;
        }
    }
}