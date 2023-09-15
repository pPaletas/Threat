using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingSystem : MonoBehaviour
{
    [SerializeField] private Gun _leftGun, _rightGun;

    public void Shoot()
    {
        _leftGun.Shoot();
        _rightGun.Shoot();
    }
}