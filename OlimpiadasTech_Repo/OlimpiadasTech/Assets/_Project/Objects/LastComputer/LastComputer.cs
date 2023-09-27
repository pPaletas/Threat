using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastComputer : HackableObject
{
    protected override void OnLoaded_E()
    {
        Debug.Log("activated");
        base.OnLoaded_E();
    }
}