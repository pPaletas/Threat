using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BulletPool : MonoBehaviour
{
    public TrailRenderer bulletPrefab;
    // Collection checks will throw errors if we try to release an item that is already in the pool.
    public bool collectionChecks = true;
    public int maxPoolSize = 100;

    IObjectPool<TrailRenderer> m_Pool;

    public IObjectPool<TrailRenderer> Pool
    {
        get
        {
            if (m_Pool == null)
            {
                m_Pool = new ObjectPool<TrailRenderer>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 1, maxPoolSize);
            }
            return m_Pool;
        }
    }

    TrailRenderer CreatePooledItem()
    {
        var ps = Instantiate<TrailRenderer>(bulletPrefab);

        return ps;
    }

    // Called when an item is returned to the pool using Release
    void OnReturnedToPool(TrailRenderer particle)
    {
        particle.gameObject.SetActive(false);
    }

    // Called when an item is taken from the pool using Get
    void OnTakeFromPool(TrailRenderer particle)
    {
        particle.gameObject.SetActive(true);
    }

    // If the pool capacity is reached then any items returned will be destroyed.
    // We can control what the destroy behavior does, here we destroy the GameObject.
    void OnDestroyPoolObject(TrailRenderer particle)
    {
        Destroy(particle);
    }
}