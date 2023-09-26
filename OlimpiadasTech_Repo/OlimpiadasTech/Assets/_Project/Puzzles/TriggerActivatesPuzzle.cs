using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerActivatesPuzzle : MonoBehaviour
{
    [SerializeField] private PuzzlesBase _puzzle;
    private bool _triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!_triggered)
        {
            _triggered = true;
            _puzzle.Enable();
        }
    }
}
