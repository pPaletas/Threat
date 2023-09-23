using System.Collections;
using System.Collections.Generic;
using CartoonFX;
using UnityEngine;

public class DroneBullet : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 5f;
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private float _bulletForce = 100f;
    [SerializeField] private GameObject _particles;
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private GameObject _trail;

    [HideInInspector] public Boss.BulletPool pool;

    private bool _isActive = false;

    private Rigidbody _rb;
    private bool _hasBeenFired = false;

    private float _currentLifeTime = 0f;

    public void NotifyFinished()
    {
        ClearEverything();
        _particles.transform.SetParent(transform);
        _particles.transform.localPosition = Vector3.zero;
        pool.Pool.Release(this);
    }

    private void Rotate()
    {
        transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime, Space.Self);
    }

    private void TickLifeTime()
    {
        if (_isActive)
        {
            _currentLifeTime += Time.deltaTime;

            if (_currentLifeTime >= _lifeTime && gameObject.activeInHierarchy)
            {
                ClearEverything();
                _isActive = false;
                pool.Pool.Release(this);
            }
        }
    }

    private void ClearEverything()
    {
        _currentLifeTime = 0f;
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _trail.GetComponent<TrailRenderer>().Clear();
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _particles = GetComponentInChildren<ParticleSystem>(true).gameObject;
        _renderer = GetComponent<MeshRenderer>();
        _trail = GetComponentInChildren<TrailRenderer>().gameObject;
    }

    private void Update()
    {
        Rotate();
        TickLifeTime();
    }

    private void OnEnable()
    {
        _particles.SetActive(false);

        _renderer.enabled = true;
        _trail.SetActive(true);
        _isActive = true;
    }

    private void FixedUpdate()
    {
        _rb.AddForce(transform.forward * _bulletForce, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (_isActive)
        {
            _isActive = false;
            _particles.transform.SetParent(null);
            _particles.SetActive(true);

            _renderer.enabled = false;
            _trail.SetActive(false);
        }
    }
}