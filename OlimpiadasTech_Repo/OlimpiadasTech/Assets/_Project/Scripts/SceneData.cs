using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneData : MonoBehaviour
{
    public static SceneData Instance;

    // Player
    private GameObject _player;
    private Transform _playerDetectionPointsContainer;

    // Enemies
    private List<GameObject> _robots = new List<GameObject>();

    public GameObject Player { get => _player; }
    public Transform PlayerDetectionPointsContainer { get => _playerDetectionPointsContainer; }

    public List<GameObject> Robots { get => _robots; }

    private void Awake()
    {
        Instance = this;
        _player = GameObject.FindObjectOfType<CharacterController>().gameObject;
        _playerDetectionPointsContainer = _player.transform.Find("DetectionPoints");

        foreach (Transform robot in GameObject.Find("Robots").transform)
        {
            _robots.Add(robot.Find("Robot").gameObject);
        }
    }
}