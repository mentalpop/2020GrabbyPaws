using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheftableAutoPopulate : MonoBehaviour
{
    public ItemPickup pickup;

    private void OnValidate() {
        if (pickup != null && pickup.itemToPickup == null && pickup.transform.parent != null) {
            pickup.itemToPickup = pickup.transform.parent.gameObject;
        }
    }
}