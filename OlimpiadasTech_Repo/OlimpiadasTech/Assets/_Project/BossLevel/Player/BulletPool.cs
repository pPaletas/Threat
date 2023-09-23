using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Boss
{
    public class BulletPool : MonoBehaviour
    {
        public DroneBullet bulletPrefab;
        // Collection checks will throw errors if we try to release an item that is already in the pool.
        public bool collectionChecks = true;
        public int maxPoolSize = 100;

        IObjectPool<DroneBullet> m_Pool;

        public IObjectPool<DroneBullet> Pool
        {
            get
            {
                if (m_Pool == null)
                {
                    m_Pool = new ObjectPool<DroneBullet>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, collectionChecks, 1, maxPoolSize);
                }
                return m_Pool;
            }
        }

        DroneBullet CreatePooledItem()
        {
            var ps = Instantiate<DroneBullet>(bulletPrefab);
            ps.pool = this;

            return ps;
        }

        // Called when an item is returned to the pool using Release
        void OnReturnedToPool(DroneBullet bullet)
        {
            bullet.gameObject.SetActive(false);
        }

        // Called when an item is taken from the pool using Get
        void OnTakeFromPool(DroneBullet particle)
        {
            particle.gameObject.SetActive(true);
        }

        // If the pool capacity is reached then any items returned will be destroyed.
        // We can control what the destroy behavior does, here we destroy the GameObject.
        void OnDestroyPoolObject(DroneBullet particle)
        {
            Destroy(particle);
        }
    }
}
