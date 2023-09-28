using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSensor : MonoBehaviour
{
    [SerializeField] private RutaNWall _wall;

    private GameObject _sensorGO;
    private GameObject _burnedSensorGO;

    [ContextMenu("Trigger")]
    public void Trigger()
    {
        if (!_wall.isOpened)
        {
            _sensorGO.SetActive(false);
            _burnedSensorGO.SetActive(true);
            _wall.Open();
        }
    }

    private void Awake()
    {
        _sensorGO = transform.Find("Sensor_Ground").gameObject;
        _burnedSensorGO = transform.Find("Sensor_Ground_Burned").gameObject;
    }
}
