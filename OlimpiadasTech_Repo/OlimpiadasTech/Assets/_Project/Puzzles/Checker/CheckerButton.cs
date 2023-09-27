using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerButton : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        GetComponent<Animator>().SetTrigger("Pushed");
        GetComponentInParent<CheckerPuzzle>().OpenDoor();
    }
}