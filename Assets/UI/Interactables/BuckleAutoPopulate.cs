using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuckleAutoPopulate : MonoBehaviour
{
    public BucklePickup pickup;

    private void OnValidate() {
        if (pickup != null && pickup.itemToPickup == null && pickup.transform.parent != null) {
            pickup.itemToPickup = pickup.transform.parent.gameObject;
        }
    }
}