using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldableInteractable : SaveLoadInteractable
{
    public HoldableData holdableData;
    public GameObject itemToPickup;

    public override void Interact() {
        if (UI.LockControls) {
            return;
        }
        bool wasPickedUp = Inventory.instance.HoldablePickUp(holdableData);
        if (wasPickedUp) {
            if (itemToPickup == null) {
                Destroy(gameObject);
            } else {
                Destroy(itemToPickup);
            }
            hasInteracted = true;
        }
        base.Interact(); //Write the pending change to the save file
    }
}
