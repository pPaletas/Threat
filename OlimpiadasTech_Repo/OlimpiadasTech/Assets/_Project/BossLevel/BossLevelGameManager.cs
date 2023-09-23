using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossLevelGameManager : MonoBehaviour
{
    private void Start()
    {
        BossLevelSceneData.Instance.Player.GetComponent<BossLevelHealthSystem>().playerDead += OnPlayerDeath;
    }

    private void OnPlayerDeath(int obj)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}