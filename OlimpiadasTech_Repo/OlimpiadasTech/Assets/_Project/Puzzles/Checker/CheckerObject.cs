using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerObject : HackableObject
{
    protected override void OnLoaded_E()
    {
        base.OnLoaded_E();
        isActive = true;
    }
}