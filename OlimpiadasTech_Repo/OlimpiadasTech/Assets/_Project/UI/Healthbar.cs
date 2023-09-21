using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Transform _livesContainer;
    private HealthSystem _healthSystem;

    private void Start()
    {
        _healthSystem = SceneData.Instance.Player.GetComponent<HealthSystem>();

        int currentLives = _healthSystem.CurrentLives;
        for (int i = 0; i < 3 - currentLives; i++)
        {
            Destroy(_livesContainer.GetChild(i).gameObject);
        }
    }

    private void Update()
    {
        _slider.value = _healthSystem.CurrentHealth / _healthSystem.maxHealth;
    }
}