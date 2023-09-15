using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class HitPool : MonoBehaviour
{
    public ParticleSystem hitSystem;
    // Collection checks will throw errors if we try to release an item that is already in the pool.
    public bool collectionChecks = true;
    public int maxPoolSize = 100;

    IObjectPool<ParticleSystem> m_Pool;

    public IObjectPool<ParticleSystem> Pool
    {
        get
        {
            if (m_Pool == null)
            {
                m_Pool = new ObjectPool<ParticleSystem>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 1, maxPoolSize);
            }
            return m_Pool;
        }
    }

    ParticleSystem CreatePooledItem()
    {
        var ps = Instantiate<ParticleSystem>(hitSystem);
        var impact = ps.gameObject.AddComponent<BulletImpact>();

        impact.pool = this;

        return ps;
    }

    // Called when an item is returned to the pool using Release
    void OnReturnedToPool(ParticleSystem particle)
    {
        particle.gameObject.SetActive(false);
    }

    // Called when an item is taken from the pool using Get
    void OnTakeFromPool(ParticleSystem particle)
    {
        particle.gameObject.SetActive(true);
    }

    // If the pool capacity is reached then any items returned will be destroyed.
    // We can control what the destroy behavior does, here we destroy the GameObject.
    void OnDestroyPoolObject(ParticleSystem particle)
    {
        Destroy(particle);
    }
}