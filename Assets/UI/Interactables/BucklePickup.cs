﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucklePickup : SaveLoadInteractable
{
    public int value;
    public GameObject itemToPickup;

    public override void Interact() {
        if (UI.LockControls) {
            return;
        }
        Currency.instance.Cash += value;
        if (itemToPickup == null) {
            Destroy(gameObject);
        } else {
            Destroy(itemToPickup);
        }
        hasInteracted = true;
        base.Interact(); //Write the pending change to the save file
    }
}