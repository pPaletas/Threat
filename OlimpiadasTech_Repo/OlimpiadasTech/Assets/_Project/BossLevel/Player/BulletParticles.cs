using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletParticles : MonoBehaviour
{
    public DroneBullet bullet;

    private void OnParticleSystemStopped()
    {
        bullet.NotifyFinished();
    }
}