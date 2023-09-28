using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RutaNWall : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [HideInInspector] public bool isOpened = false;

    private bool isOpening = false;
    private Vector3 _originalPos;
    private Vector3 _targetPos;

    public void Open()
    {
        isOpened = true;
        isOpening = true;
    }

    private void Awake()
    {
        _originalPos = transform.localPosition;
        _targetPos = _originalPos + Vector3.down * 2f;
    }

    private void Update()
    {
        if (isOpening)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, _targetPos, Time.deltaTime * _speed);

            if (transform.localPosition.y <= -1f)
            {
                Vector3 pos = transform.localPosition;
                pos.y = -1f;
                transform.localPosition = pos;

                isOpening = false;
            }
        }
    }
}