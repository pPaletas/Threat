using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Transform _livesContainer;
    [SerializeField] private GameObject _chipsAmmout;
    [SerializeField] private GameObject _livePrefab;
    private HealthSystem _healthSystem;

    private IEnumerator ShowAddedChips()
    {
        _chipsAmmout.SetActive(true);
        yield return new WaitForSeconds(1f);
        _chipsAmmout.GetComponentInChildren<TextMeshProUGUI>().text = _healthSystem.CollectedChips.ToString();
        yield return new WaitForSeconds(1f);
        _chipsAmmout.SetActive(false);
    }

    private void Start()
    {
        _healthSystem = SceneData.Instance.Player.GetComponent<HealthSystem>();

        _healthSystem.onChipsAdded += OnChipsAdded;
        _healthSystem.onChipsRemoved += OnChipsRemoved;
        _healthSystem.livesAdded += OnLivesAdded;

        int currentLives = _healthSystem.CurrentLives;
        for (int i = 0; i < 3 - currentLives; i++)
        {
            Destroy(_livesContainer.GetChild(i).gameObject);
        }

        int collectedChips = _healthSystem.CollectedChips;
        _chipsAmmout.GetComponentInChildren<TextMeshProUGUI>().text = collectedChips.ToString();
    }

    private void OnDisable()
    {
        _healthSystem.onChipsAdded += OnChipsAdded;
        _healthSystem.onChipsRemoved += OnChipsRemoved;
    }

    private void Update()
    {
        _slider.value = _healthSystem.CurrentHealth / _healthSystem.maxHealth;
    }

    #region Callbacks
    private void OnChipsAdded(int amount)
    {
        StartCoroutine(ShowAddedChips());
    }

    private void OnChipsRemoved(int amount)
    {
        _chipsAmmout.GetComponentInChildren<TextMeshProUGUI>().text = _healthSystem.CollectedChips.ToString();
    }

    private void OnLivesAdded(int obj)
    {
        if (_livesContainer.childCount < 3)
        {
            Instantiate(_livePrefab, _livesContainer);
        }
    }
    #endregion
}