using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneData : MonoBehaviour
{
    public static SceneData Instance;

    private GameObject _player;
    private Transform _playerDetectionPointsContainer;

    public GameObject Player { get => _player; }
    public Transform PlayerDetectionPointsContainer { get => _playerDetectionPointsContainer; }

    private void Awake()
    {
        Instance = this;
        _player = GameObject.FindObjectOfType<CharacterController>().gameObject;
        _playerDetectionPointsContainer = _player.transform.Find("DetectionPoints");
    }
}