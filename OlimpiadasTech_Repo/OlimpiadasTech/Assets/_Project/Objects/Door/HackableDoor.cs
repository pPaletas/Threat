using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackableDoor : HackableObject
{
    protected override void OnLoaded_E()
    {
        transform.parent.GetComponent<Animator>().SetTrigger("Open");
        base.OnLoaded_E();
    }
}