using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganButton : HackableObject
{
    private OrganPuzzle _puzzle;

    protected override void OnLoaded_E()
    {
        _puzzle.TriggerOrgan(gameObject.name);
        base.OnLoaded_E();
    }

    protected override void Awake()
    {
        base.Awake();
        isActive = false;

        _puzzle = GetComponentInParent<OrganPuzzle>();
        _puzzle.showingEnded += OnShowingEnded;
        _puzzle.restart += OnRestart;
    }

    private void OnDisable()
    {
        _puzzle.showingEnded -= OnShowingEnded;
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