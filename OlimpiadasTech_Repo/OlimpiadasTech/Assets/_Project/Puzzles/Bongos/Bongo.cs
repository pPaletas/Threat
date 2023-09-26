using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bongo : HackableObject
{
    private BongosPuzzle _puzzle;

    protected override void Awake()
    {
        base.Awake();
        isActive = false;
        _puzzle = GetComponentInParent<BongosPuzzle>(true);

        _puzzle.showingEnded += OnShowingEnded;
        _puzzle.restart += OnRestart;
    }

    private void OnDisable()
    {
        _puzzle.showingEnded -= OnShowingEnded;
    }

    protected override void OnLoaded_E()
    {
        _puzzle.TriggerBongo(gameObject.name);

        base.OnLoaded_E();
    }


    private void OnRestart()
    {
        isActive = false;
    }

    private void OnShowingEnded()
    {
        isActive = true;
    }
}