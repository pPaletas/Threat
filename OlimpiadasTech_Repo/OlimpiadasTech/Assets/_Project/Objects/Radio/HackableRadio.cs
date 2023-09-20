using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackableRadio : HackableObject
{
    [Header("Radio")]
    [SerializeField] private float _noiseRadius = 10f;
    [SerializeField] private LayerMask _dronesMask;
    [SerializeField] private LayerMask _wallsMask;

    protected override void OnLoaded_E()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _noiseRadius, _dronesMask);

        if (colliders.Length > 0f)
        {

            foreach (Collider hit in colliders)
            {
                if (!Physics.Linecast(transform.position, hit.transform.position, _wallsMask))
                {
                    Drone droneScript = hit.GetComponent<Drone>();
                    droneScript.Alert(gameObject);
                }
            }
        }
        base.OnLoaded_E();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _noiseRadius);
    }
}