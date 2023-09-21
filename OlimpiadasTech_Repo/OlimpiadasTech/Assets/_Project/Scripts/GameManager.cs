using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Action onDataSave;

    public static GameManager Instance;
    private GameObject _plr;

    public void SaveAllData()
    {
        Vector3 pos = _plr.transform.position;
        Quaternion rot = _plr.transform.rotation;

        SavingSystem.Instance.SaveData("Position", pos);
        SavingSystem.Instance.SaveData("Rotation", rot);

        onDataSave?.Invoke();
    }

    private void OnPlayerDeath(int currentLives)
    {
        if (currentLives > 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _plr = SceneData.Instance.Player;

        _plr.transform.position = (Vector3)SavingSystem.Instance.LoadData("Position");
        _plr.transform.rotation = (Quaternion)SavingSystem.Instance.LoadData("Rotation");

        _plr.GetComponent<HealthSystem>().playerDead += OnPlayerDeath;
    }
}