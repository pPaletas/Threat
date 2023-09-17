using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymbolsLookTowardsPlayer : MonoBehaviour
{
    void Update()
    {
        Vector3 unit = (SceneData.Instance.Player.transform.position - transform.position);
        unit.y = 0f;
        unit.Normalize();

        transform.forward = unit;
    }
}
