using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLevelHealthSystem : MonoBehaviour
{
    public Action<int> playerDead;
    public Action<int> livesAdded;

    public float maxHealth = 100f;
    [Header("Blood screen")]
    [SerializeField] private CanvasGroup _bloodScreen;
    [SerializeField] private float _animFreq = 5f;
    [SerializeField] private float _animIntensity = 0.2f;

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
            playerDead?.Invoke(_currentLives);
        }
    }

    [ContextMenu("TakeDamage")]
    public void TakeDamageDebug()
    {
        _currentHealth = Mathf.Clamp(_currentHealth - (maxHealth / 3), 0f, maxHealth);
    }

    public void AddLives(int amount)
    {
        _currentLives++;
        livesAdded?.Invoke(amount);
    }

    private void Awake()
    {
        _currentHealth = maxHealth;
    }

    private void LateUpdate()
    {
        if (_currentHealth < maxHealth * 0.6f)
        {
            float currentAlpha = 1 - (_currentHealth / maxHealth);

            // Se le suma 0.5 para que quede entre
            float sine = (Mathf.Sin(Time.time * _animFreq) + 0.5f) * 0.5f * _animIntensity;

            _bloodScreen.alpha = currentAlpha - sine;
        }
        else
        {
            _bloodScreen.alpha = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BossArm"))
        {
            TakeDamage(30f);
        }
    }
}