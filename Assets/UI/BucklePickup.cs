using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BucklePickup : Interactable
{
    public int value;

    private void Awake() {
        gameObject.AddComponent<cakeslice.Outline>();
    }

    public override void Interact() {
        Currency.instance.Cash += value;
        Destroy(gameObject);
    }
}