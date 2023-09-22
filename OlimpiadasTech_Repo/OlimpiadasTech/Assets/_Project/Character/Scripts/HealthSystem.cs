using System;
using StarterAssets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthSystem : MonoBehaviour
{
    public Action<int> playerDead;
    public Action<int> onChipsAdded;
    public Action<int> onChipsRemoved;

    public float maxHealth = 100f;
    [Header("Blood screen")]
    [SerializeField] private CanvasGroup _bloodScreen;
    [SerializeField] private float _animFreq = 5f;
    [SerializeField] private float _animIntensity = 0.2f;

    private float _currentHealth;
    private int _currentLives = 3;
    private int _collectedChips = 0;
    private bool _isDead;

    private ThirdPersonController _movement;

    // Animation
    private Animator _anim;
    private int _hurtedHash = Animator.StringToHash("Hurted");
    private int _defeathHash = Animator.StringToHash("Defeat");

    public float CurrentHealth { get => _currentHealth; }
    public int CurrentLives { get => _currentLives; }
    public int CollectedChips { get => _collectedChips; }

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
            _isDead = true;
            _currentLives--;
            SavingSystem.Instance.SaveData("Lives", _currentLives);
            playerDead?.Invoke(_currentLives);
        }
    }

    [ContextMenu("TakeDamage")]
    public void TakeDamageDebug()
    {
        _currentHealth = Mathf.Clamp(_currentHealth - (maxHealth / 3), 0f, maxHealth);
    }

    public void Defeat()
    {
        _movement.canMove = false;
        _anim.SetTrigger(_defeathHash);
    }

    public void AddChips(int amount)
    {
        _collectedChips += amount;
        onChipsAdded?.Invoke(amount);
    }

    public void RemoveChips(int amount)
    {
        _collectedChips -= amount;
        onChipsRemoved?.Invoke(amount);
    }

    private void Awake()
    {
        _currentHealth = maxHealth;
        _anim = GetComponent<Animator>();
        _movement = GetComponent<ThirdPersonController>();
    }

    private void Start()
    {
        _currentHealth = (float)SavingSystem.Instance.LoadData("Health");
        _currentLives = (int)SavingSystem.Instance.LoadData("Lives");
        _collectedChips = (int) SavingSystem.Instance.LoadData("Chips");

        GameManager.Instance.onDataSave += OnSaveData;
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

    private void OnDisable()
    {
        GameManager.Instance.onDataSave -= OnSaveData;
    }

    #region Callbacks
    public void OnHurtAnimationFinished()
    {
        _movement.canMove = true;
    }

    public void OnDefeatAnimationFinished()
    {
        SceneManager.LoadScene(0);
    }

    private void OnSaveData()
    {
        SavingSystem.Instance.SaveData("Health", _currentHealth);
        SavingSystem.Instance.SaveData("Chips", _collectedChips);
    }
    #endregion
}