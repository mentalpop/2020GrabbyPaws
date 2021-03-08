using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucklePickup : SaveLoadInteractable
{
    public int value;

    public override void Interact() {
        Currency.instance.Cash += value;
        Destroy(gameObject);
        hasBeenCollected = true;
        base.Interact();
    }
}