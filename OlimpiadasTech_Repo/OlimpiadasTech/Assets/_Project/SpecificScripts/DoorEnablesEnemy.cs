using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorEnablesEnemy : MonoBehaviour
{
    [SerializeField] private CameraBehaviour _camera;
    [SerializeField] private RobotStateMachine _robot;
    private HackableDoor _door;

    private bool _activated = false;

    private void Update()
    {
        if (_door.isActive == false && !_activated)
        {
            _activated = true;

            if(_camera != null) _camera.isEnemyActive = true;
            if(_robot != null) _robot.isEnemyActive = true;
        }
    }

    private void Awake()
    {
        _door = GetComponent<HackableDoor>();
    }
}