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
    }

    private void Update()
    {
        _slider.value = _healthSystem.CurrentHealth / _healthSystem.maxHealth;
    }
}