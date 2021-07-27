using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StolenItemContainer : MonoBehaviour
{
    public GameObject sipPrefab;
    public Inventory inventory;

    private void OnEnable() {
        inventory.OnPickUp += PickUp;
    }

    private void OnDisable() {
        inventory.OnPickUp -= PickUp;
    }

    private void PickUp(Item item) {
        GameObject newGO = Instantiate(sipPrefab, transform, false);
        StolenItemPrompt prompt = newGO.GetComponent<StolenItemPrompt>();
        prompt.Unpack(Inventory.GetInventoryItem(item));
    }
}