using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class MainDoorPuzzle : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private Cinemachine.CinemachineVirtualCamera _vCamSelf;

    private Transform _wheelContainer;

    private int _openingPiece = -1;
    private int _currentDirection = 1;

    private bool _rotatingPuzzle = false;
    private bool _openingDoor = false;

    [ContextMenu("Open Piece")]
    public void OpenPiece()
    {
        StartCoroutine(SwitchCamera());
    }

    private IEnumerator SwitchCamera()
    {
        _vCamSelf.Priority = 20;
        yield return new WaitForSeconds(1f);
        _openingPiece++;
        _rotatingPuzzle = true;
    }

    private IEnumerator CloseAfterOpening()
    {
        yield return new WaitForSeconds(1f);
        _vCamSelf.Priority = 5;
    }

    private void Start()
    {
        _wheelContainer = transform.Find("Wheel/Container");
        GameManager.Instance.onPuzzleCompleted += OnPuzzleCompleted;
    }

    private void OnDisable()
    {
        GameManager.Instance.onPuzzleCompleted -= OnPuzzleCompleted;
    }

    private void Update()
    {
        if (_rotatingPuzzle)
        {
            Quaternion currentRot = _wheelContainer.Find(_openingPiece.ToString() + ".L").localRotation;
            currentRot = Quaternion.Slerp(currentRot, Quaternion.identity, Time.deltaTime * _speed);

            _wheelContainer.Find(_openingPiece.ToString() + ".L").localRotation = currentRot;
            _wheelContainer.Find(_openingPiece.ToString() + ".R").localRotation = currentRot;

            float angle = Quaternion.Angle(currentRot, Quaternion.identity);

            if (Math.Abs(angle) <= 0.5f)
            {
                _wheelContainer.Find(_openingPiece.ToString() + ".L").localRotation = Quaternion.identity;
                _wheelContainer.Find(_openingPiece.ToString() + ".R").localRotation = Quaternion.identity;

                if (_openingPiece >= 2)
                {
                    _openingDoor = true;
                    _rotatingPuzzle = false;
                    StartCoroutine(CloseAfterOpening());
                }
                else
                {
                    _rotatingPuzzle = false;
                    _currentDirection *= -1;
                    _vCamSelf.Priority = 5;
                }
            }
        }

        if (_openingDoor)
        {
            Vector3 rightDoorPos = transform.Find("Doors/RightDoor").localPosition;
            Vector3 rightDoorPosLerp = Vector3.Lerp(rightDoorPos, new Vector3(3f, 1f, 0f), Time.deltaTime * _speed);

            Vector3 dif = rightDoorPosLerp - rightDoorPos;

            // Derecha
            transform.Find("Doors/RightDoor").localPosition += dif;
            _wheelContainer.Find("0.R").position += dif;
            _wheelContainer.Find("1.R").position += dif;
            _wheelContainer.Find("2.R").position += dif;

            // Izquierda
            transform.Find("Doors/LeftDoor").localPosition -= dif;
            _wheelContainer.Find("0.L").position -= dif;
            _wheelContainer.Find("1.L").position -= dif;
            _wheelContainer.Find("2.L").position -= dif;
        }
    }

    private void OnPuzzleCompleted(string obj)
    {
        OpenPiece();
    }
}