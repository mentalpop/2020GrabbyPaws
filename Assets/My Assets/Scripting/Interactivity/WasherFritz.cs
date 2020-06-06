using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WasherFritz : Interactable
{
    public override void Interact()
    {
        base.Interact();
        Debug.Log("Zzzt");
    }

}
