using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseSystem : MonoBehaviour
{
    public static NoiseSystem Instance;

    [SerializeField] private Transform _noisePivot;
    [SerializeField] private LayerMask _wallsMask;
    [SerializeField] private float _blockedNoiseDisminution = 0.75f;

    private Transform _robotsContainer;

    public void GenerateNoise(float radius, float noiseValue)
    {
        foreach (Transform robotInfo in _robotsContainer)
        {
            DetectionSystem detectionSystem = robotInfo.GetComponentInChildren<DetectionSystem>();

            float distance = Vector3.Distance(detectionSystem.transform.position, _noisePivot.position);

            if (distance <= radius)
            {
                // Estar bloqueado disminuye el ruido 
                bool isBlocked = detectionSystem.IsPlayerBlocked();

                float noise = isBlocked ? noiseValue * _blockedNoiseDisminution : noiseValue;
                detectionSystem.TriggerEars(noise);
            }
        }
    }

    private void Awake()
    {
        Instance = this;
        _robotsContainer = GameObject.Find("Robots").transform;
    }
}