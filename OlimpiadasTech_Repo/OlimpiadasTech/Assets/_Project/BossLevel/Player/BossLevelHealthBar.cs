using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossLevelHealthBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    private BossLevelHealthSystem _healthSystem;

    private void Start()
    {
        _healthSystem = BossLevelSceneData.Instance.Player.GetComponent<BossLevelHealthSystem>();
    }

    private void Update()
    {
        _slider.value = _healthSystem.CurrentHealth / _healthSystem.maxHealth;
    }
}