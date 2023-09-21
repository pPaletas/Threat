using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    private SavingSystem _savingSystem;

    public void RestartGame()
    {
        _savingSystem.ResetData();
        SceneManager.LoadScene(0);
    }

    private void Awake()
    {
        _savingSystem = GetComponent<SavingSystem>();
        Cursor.lockState = CursorLockMode.None;
    }
}