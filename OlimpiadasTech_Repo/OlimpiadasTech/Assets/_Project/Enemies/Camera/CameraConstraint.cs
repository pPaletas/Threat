using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraConstraint : MonoBehaviour
{
    [SerializeField] private Transform _neck;
    [SerializeField] private Transform _head;
    [SerializeField] private Transform _target;

    private Quaternion _neckOrigRotation;
    private Quaternion _headOrigRotation;

    private void LateUpdate()
    {
        Vector3 neckUnit = (_target.position - _neck.position);
        neckUnit.y = 0f;
        neckUnit.Normalize();

        Vector3 headUnit = (_target.position - _head.position);
        headUnit.Normalize();

        _neck.rotation = Quaternion.LookRotation(neckUnit);
        _head.LookAt(_target.position, Vector3.up);
        // _head.localEulerAngles = new Vector3(_neck.localEulerAngles.x, 0f, 0f);
    }

    private void Awake()
    {
        _neckOrigRotation = _neck.rotation;
        _headOrigRotation = _head.rotation;
    }
}
