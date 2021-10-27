using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractEvent : Interactable
{
    public UnityEvent OnInteract;
    public override void Interact() {
        //base.Interact();
        if (UI.LockControls) {
            return;
        }
        OnInteract.Invoke();
    }
}