using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField]
    private bool AddBulletSpread = true;
    [SerializeField]
    private Vector3 BulletSpreadVariance = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField]
    private ParticleSystem ShootingSystem;
    [SerializeField]
    private Transform BulletSpawnPoint;
    [SerializeField]
    private float ShootDelay = 0.5f;
    [SerializeField]
    private LayerMask Mask;
    [SerializeField]
    private float BulletSpeed = 100;

    [Header("Pools")]
    [SerializeField]
    private BulletPool _bulletPool;
    [SerializeField]
    private HitPool _impactPool;

    private float LastShootTime;

    public void Shoot()
    {
        if (LastShootTime + ShootDelay < Time.time)
        {
            // Use an object pool instead for these! To keep this tutorial focused, we'll skip implementing one.
            // For more details you can see: https://youtu.be/fsDE_mO4RZM or if using Unity 2021+: https://youtu.be/zyzqA_CPz2E

            ShootingSystem.Play();
            Vector3 direction = GetDirection();

            if (Physics.Raycast(BulletSpawnPoint.position, direction, out RaycastHit hit, float.MaxValue, Mask))
            {
                // TrailRenderer trail = Instantiate(BulletTrail, BulletSpawnPoint.position, Quaternion.identity);
                TrailRenderer trail = _bulletPool.Pool.Get();
                trail.transform.position = BulletSpawnPoint.position;
                trail.transform.rotation = Quaternion.identity;

                HealthSystem health = null;

                bool isPlr = hit.collider.TryGetComponent<HealthSystem>(out health);

                StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, true, health));

                LastShootTime = Time.time;
            }
            // this has been updated to fix a commonly reported problem that you cannot fire if you would not hit anything
            else
            {
                TrailRenderer trail = _bulletPool.Pool.Get();
                trail.transform.position = BulletSpawnPoint.position;
                trail.transform.rotation = Quaternion.identity;

                StartCoroutine(SpawnTrail(trail, BulletSpawnPoint.position + GetDirection() * 100, Vector3.zero, false));

                LastShootTime = Time.time;
            }
        }
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = transform.forward;

        if (AddBulletSpread)
        {
            direction += new Vector3(
                Random.Range(-BulletSpreadVariance.x, BulletSpreadVariance.x),
                Random.Range(-BulletSpreadVariance.y, BulletSpreadVariance.y),
                Random.Range(-BulletSpreadVariance.z, BulletSpreadVariance.z)
            );

            direction.Normalize();
        }

        return direction;
    }

    private IEnumerator SpawnTrail(TrailRenderer Trail, Vector3 HitPoint, Vector3 HitNormal, bool MadeImpact, HealthSystem health = null)
    {
        // This has been updated from the video implementation to fix a commonly raised issue about the bullet trails
        // moving slowly when hitting something close, and not
        Vector3 startPosition = Trail.transform.position;
        float distance = Vector3.Distance(Trail.transform.position, HitPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, HitPoint, 1 - (remainingDistance / distance));

            remainingDistance -= BulletSpeed * Time.deltaTime;

            yield return null;
        }
        Trail.transform.position = HitPoint;
        if (MadeImpact)
        {
            // Instantiate(ImpactParticleSystem, HitPoint, Quaternion.LookRotation(HitNormal));
            ParticleSystem clone = _impactPool.Pool.Get();
            clone.transform.position = HitPoint;
            clone.transform.rotation = Quaternion.LookRotation(HitNormal);

            if (health != null)
            {
                health.TakeDamage(2);
            }
        }

        _bulletPool.Pool.Release(Trail);
    }
}
