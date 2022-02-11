using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldableHeld : MonoBehaviour
{
    private int layerPreventDrop = 12; //StopMove

    public HoldableData holdableData { get; protected set; }

    private List<GameObject> collidingWith = new List<GameObject>();

    public void Unpack(HoldableData _holdableData) {
        holdableData = _holdableData;
    }

    public virtual void Use() {

    }

    public bool CanDrop() {
        return collidingWith.Count == 0; //Can only drop if list of colliders is empty
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == layerPreventDrop) {
            collidingWith.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == layerPreventDrop) {
            collidingWith.Remove(other.gameObject);
        }
    }
}