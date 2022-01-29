using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericInteractable : SaveLoadInteractable
{
    public override void Interact() {
        if (UI.LockControls) {
            return;
        }
        hasInteracted = true;
        base.Interact(); //Write the pending change to the save file
    }
}