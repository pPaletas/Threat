using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletImpact : MonoBehaviour
{
    public HitPool pool;
    private ParticleSystem _particles;

    private void OnParticleSystemStopped()
    {
        pool.Pool.Release(_particles);
    }

    private void Awake()
    {
        _particles = GetComponent<ParticleSystem>();
    }
}
