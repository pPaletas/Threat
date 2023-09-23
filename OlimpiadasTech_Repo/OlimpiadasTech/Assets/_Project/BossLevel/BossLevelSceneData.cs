using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BossLevelSceneData : MonoBehaviour
{
    public static BossLevelSceneData Instance;

    // Player
    private GameObject _player;

    // Enemies
    private List<GameObject> _enemies = new List<GameObject>();
    private List<GameObject> _toDispose = new List<GameObject>();
    private bool _hasItemsToDispose = false;

    public GameObject Player { get => _player; }

    public List<GameObject> Enemies { get => _enemies; }

    public void NotifyDeletion(GameObject obj)
    {
        _toDispose.Add(obj);
        _hasItemsToDispose = true;
    }

    private void Awake()
    {
        Instance = this;
        _player = GameObject.FindObjectOfType<PlayerInput>().gameObject;

        foreach (Transform enemy in GameObject.Find("Enemies").transform)
        {
            _enemies.Add(enemy.gameObject);
        }
    }

    private void LateUpdate()
    {
        if (_hasItemsToDispose)
        {
            foreach (var item in _toDispose)
            {
                _enemies.Remove(item);
            }

            _hasItemsToDispose = false;
            _toDispose.Clear();
        }

    }
}