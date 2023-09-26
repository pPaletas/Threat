using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveEntity : MonoBehaviour
{
    [SerializeField] private Vector2 _randomLifeTime = new Vector2(5f, 7f);

    private GameObject _shiningParticles, _explosionParticles;

    private float _lifeTime;
    private float _currentLifeTime = 0f;
    private bool _lifeTimeReached = false;

    private void TickLifeTime()
    {
        _currentLifeTime += Time.deltaTime;
    }

    private void Awake()
    {
        _lifeTime = Random.Range(_randomLifeTime.x, _randomLifeTime.y);

        _shiningParticles = transform.Find("Shining").gameObject;
        _explosionParticles = transform.Find("Explosion").gameObject;
    }

    private void Update()
    {
        if (!_lifeTimeReached)
        {
            TickLifeTime();

            if (_currentLifeTime >= _lifeTime)
            {
                _lifeTimeReached = true;

                _shiningParticles.SetActive(false);

                _explosionParticles.transform.SetParent(null);
                _explosionParticles.SetActive(true);

                Vector3 plrPos = BossLevelSceneData.Instance.Player.transform.position;

                float plrDist = Vector3.Distance(plrPos, transform.position);

                if (plrDist <= 50f)
                {
                    BossLevelSceneData.Instance.Player.GetComponent<BossLevelHealthSystem>().TakeDamage(50f);
                }

                Destroy(gameObject);
            }
        }
    }
}