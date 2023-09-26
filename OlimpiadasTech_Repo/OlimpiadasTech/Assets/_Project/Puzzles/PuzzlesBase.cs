using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlesBase : MonoBehaviour
{
    public virtual void Enable() { }
    protected void PuzzleCompleted()
    {
        GameManager.Instance.PuzzleCompleted(gameObject.name);
    }
}
