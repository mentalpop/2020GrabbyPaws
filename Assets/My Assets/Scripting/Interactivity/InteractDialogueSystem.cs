using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class InteractDialogueSystem : Interactable
{
    public DialogueSystemTrigger DSTrigger;
    public override void Interact() {
        //base.Interact();
        if (UI.LockControls) {
            return;
        }
        DSTrigger.OnUse();
    }
}