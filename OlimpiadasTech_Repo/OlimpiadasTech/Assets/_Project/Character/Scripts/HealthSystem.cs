using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public Action playerDead;
    public float maxHealth = 100f;
    [Header("Blood screen")]
    [SerializeField] private CanvasGroup _bloodScreen;
    [SerializeField] private float _animFreq = 5f;
    [SerializeField] private float _animIntensity = 0.2f;

    private float _currentHealth;
    private bool _isDead;

    private Animator _anim;
    private ThirdPersonController _movement;
    private int _hurtedHash = Animator.StringToHash("Hurted");

    public float CurrentHealth { get => _currentHealth; }

    public void TakeDamage(float dmg)
    {
        _currentHealth = Mathf.Clamp(_currentHealth - dmg, 0f, maxHealth);

        if (dmg >= 25f)
        {
            _movement.canMove = false;
            _anim.SetTrigger(_hurtedHash);
        }

        if (_currentHealth <= 0f && !_isDead)
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

    public void OnHurtAnimationFinished()
    {
        _movement.canMove = true;
    }

    private void Awake()
    {
        _currentHealth = maxHealth;
        _anim = GetComponent<Animator>();
        _movement = GetComponent<ThirdPersonController>();
    }

    private void LateUpdate()
    {
        if (_currentHealth < maxHealth)
        {
            float currentAlpha = 1 - (_currentHealth / maxHealth);
            
            // Se le suma 0.5 para que quede entre
            float sine = (Mathf.Sin(Time.time * _animFreq) + 0.5f) * 0.5f * _animIntensity;

            _bloodScreen.alpha = currentAlpha - sine;
        }
    }
}