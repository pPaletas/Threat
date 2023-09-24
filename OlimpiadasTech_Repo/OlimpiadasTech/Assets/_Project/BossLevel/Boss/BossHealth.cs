using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    public Action<int> bossDead;

    public float maxHealth = 100f;
    [SerializeField] private Slider _healthBar;

    private float _currentHealth;
    private int _currentLives = 3;
    private bool _isDead;

    public float CurrentHealth { get => _currentHealth; }
    public int CurrentLives { get => _currentLives; }

    public void TakeDamage(float dmg)
    {
        _currentHealth = Mathf.Clamp(_currentHealth - dmg, 0f, maxHealth);

        if (_currentHealth <= 0f && !_isDead)
        {
            _isDead = true;
            _currentLives--;
            bossDead?.Invoke(_currentLives);
        }
    }

    [ContextMenu("TakeDamage")]
    public void TakeDamageDebug()
    {
        _currentHealth = Mathf.Clamp(_currentHealth - (maxHealth / 3), 0f, maxHealth);
    }

    private void Awake()
    {
        _currentHealth = maxHealth;
    }

    private void Update()
    {
        _healthBar.value = CurrentHealth / maxHealth;
    }
}