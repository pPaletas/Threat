using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public Action playerDead;
    public float maxHealth = 100f;

    private float _currentHealth;
    private bool _isDead;

    public float CurrentHealth { get => _currentHealth; }

    public void TakeDamage(float dmg)
    {
        _currentHealth = Mathf.Clamp(_currentHealth - dmg, 0f, maxHealth);

        if(_currentHealth <= 0f && !_isDead)
        {
            playerDead?.Invoke();
            _isDead = true;
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
}