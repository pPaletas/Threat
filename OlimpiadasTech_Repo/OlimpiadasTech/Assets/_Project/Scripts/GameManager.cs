using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        SceneData.Instance.Player.transform.position = (Vector3)SavingSystem.Instance.LoadData("Position");
        SceneData.Instance.Player.transform.rotation = (Quaternion)SavingSystem.Instance.LoadData("Rotation");

        SceneData.Instance.Player.GetComponent<HealthSystem>().playerDead += OnPlayerDeath;
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
}