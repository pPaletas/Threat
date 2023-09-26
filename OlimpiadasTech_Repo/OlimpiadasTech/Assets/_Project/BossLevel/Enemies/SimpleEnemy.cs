using System.Collections;
using System.Collections.Generic;
using CartoonFX;
using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
    [SerializeField] private Vector2 _randomSpeed = new Vector2(5f, 15f);
    [SerializeField] private float _initialForce = 200f;
    [SerializeField] private float _timeBeforeChasingPlayer = 2f;
    [SerializeField] private float _chasingDelay = 0.5f;

    private Rigidbody _rb;
    private GameObject _enemyParticles;

    private Vector3 _initialForceDirection;
    private float _speed;
    private float _currentTimeBeforeChasingPlayer = 0f;

    private Vector3 _currentPlayerPos;

    private void ChasePlayer()
    {
        Vector3 unit = (_currentPlayerPos - transform.position).normalized;
        _rb.AddForce(unit * _speed);

        Vector3 vel = _rb.velocity;
        vel = Vector3.ClampMagnitude(vel, _speed);

        _rb.velocity = vel;
    }

    private void Start()
    {
        BossLevelSceneData.Instance.Enemies.Add(gameObject);

        GetComponentInChildren<CFXR_Effect>(true).SetTarget(BossLevelSceneData.Instance.Player.transform);

        _rb = GetComponent<Rigidbody>();
        _enemyParticles = GetComponentInChildren<ParticleSystem>(true).gameObject;

        _speed = Random.Range(_randomSpeed.x, _randomSpeed.y);

        float randomX = Random.Range(-1f, 1f);
        float randomY = Random.Range(-1f, 1f);

        randomX = randomX == 0f ? 1f : randomX;
        randomY = randomY == 0f ? 1f : randomY;

        Vector3 unit = (BossLevelSceneData.Instance.Player.transform.position - transform.position).normalized;
        float randomAngle = Random.Range(15f, 45f);
        _initialForceDirection = Quaternion.AngleAxis(randomAngle, transform.right) * unit;
        _initialForceDirection = transform.TransformDirection(_initialForceDirection);

        _currentPlayerPos = BossLevelSceneData.Instance.Player.transform.position;
    }

    private void FixedUpdate()
    {
        _currentPlayerPos = BossLevelSceneData.Instance.Player.transform.position;

        if (_currentTimeBeforeChasingPlayer < _timeBeforeChasingPlayer)
        {
            _rb.AddForce(_initialForceDirection * _initialForce, ForceMode.Impulse);
            _currentTimeBeforeChasingPlayer += Time.deltaTime;
        }
        else
        {
            ChasePlayer();
        }

        if (_rb.velocity.sqrMagnitude != 0f)
        {
            transform.forward = _rb.velocity.normalized;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (_currentTimeBeforeChasingPlayer >= _timeBeforeChasingPlayer)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<BossLevelHealthSystem>().TakeDamage(10f);
            }

            _enemyParticles.transform.SetParent(null);
            _enemyParticles.SetActive(true);
            BossLevelSceneData.Instance.NotifyDeletion(gameObject);
            Destroy(gameObject);
        }
    }
}