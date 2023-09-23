using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class DroneShootingSystem : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] private Boss.BulletPool _bulletsPool;
    [SerializeField] private Transform _bulletsSpawnL;
    [SerializeField] private Transform _bulletsSpawnR;
    [SerializeField] private Camera _cam;
    [SerializeField] private float _fireRate = 0.5f;

    [Header("Focus")]
    [SerializeField] private Transform _targetImg;
    [SerializeField] private float _distanceToCameraCenter = 300f;
    [SerializeField] private float _radius = 20f;

    private StarterAssetsInputs _input;

    private float _timeSinceLastShot = 0f;
    private Transform _target;

    private void Fire()
    {

        if (_input.shoot && _timeSinceLastShot >= _fireRate)
        {
            Quaternion camRot = _cam.transform.rotation;

            DroneBullet newBulletL = _bulletsPool.Pool.Get();
            Quaternion targetRotationL = _target != null ? Quaternion.LookRotation((_target.position - _bulletsSpawnL.position).normalized) : camRot;
            newBulletL.transform.SetPositionAndRotation(_bulletsSpawnL.position, targetRotationL);
            Quaternion targetRotationR = _target != null ? Quaternion.LookRotation((_target.position - _bulletsSpawnR.position).normalized) : camRot;
            DroneBullet newBulletR = _bulletsPool.Pool.Get();
            newBulletR.transform.SetPositionAndRotation(_bulletsSpawnR.position, targetRotationR);

            _timeSinceLastShot = 0f;
        }
        else
        {
            _timeSinceLastShot += Time.deltaTime;
        }
    }

    private void LookForATargetToLockOn()
    {
        _target = null;

        foreach (GameObject enemy in BossLevelSceneData.Instance.Enemies)
        {
            if (enemy == null) continue;

            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            float distToCamera = Vector2.Distance(_cam.WorldToScreenPoint(enemy.transform.position), new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));

            Vector3 unit = (enemy.transform.position - transform.position);
            unit.Normalize();

            Vector3 fwd = transform.forward;
            fwd.Normalize();

            float dot = Vector3.Dot(fwd, unit);
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            // Si aplica
            if (dist <= _radius && distToCamera <= _distanceToCameraCenter && angle <= 60f)
            {
                // Revisar si ya existe uno, y si no, o está mas cerca entonces lo incluimos
                if (_target == null || dist < Vector3.Distance(transform.position, _target.position))
                {
                    _target = enemy.transform;
                }
            }
        }
    }

    private void DisplayTargetHUD()
    {
        if (_target != null)
        {
            _targetImg.gameObject.SetActive(true);
            _targetImg.position = _cam.WorldToScreenPoint(_target.position);
        }
        else
        {
            _targetImg.gameObject.SetActive(false);
        }
    }

    private void Awake()
    {
        _input = GetComponent<StarterAssetsInputs>();
    }

    private void Update()
    {
        LookForATargetToLockOn();
        DisplayTargetHUD();
        Fire();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}