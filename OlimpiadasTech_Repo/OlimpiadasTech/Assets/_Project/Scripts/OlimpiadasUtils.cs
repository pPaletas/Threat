using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class OlimpiadasUtils
{
    public static bool HasReachedDestination(this NavMeshAgent agent)
    {
        float dist = agent.remainingDistance;
        return dist != Mathf.Infinity && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0 && !agent.pathPending;
    }

    public static float Modulus(float a, float b)
    {
        return a - b * Mathf.Floor(a / b);
    }

    public static int ModulusInt(int a, int b)
    {
        float res = a - b * Mathf.Floor((float)a / (float)b);
        return Mathf.RoundToInt(res);
    }
}