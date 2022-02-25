using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldableColliderHandler : MonoBehaviour
{
    public Rigidbody rBody;
    public Collider myCollider;

    private bool isKinematic = true;
    private bool isTouchingPlayer = false;

    private void OnValidate() {
        if (rBody == null) {
            rBody = GetComponent<Rigidbody>();
        }
        if (myCollider == null) {
            myCollider = GetComponent<Collider>();
        }
    }

    private void Start() {
        StartCoroutine(NextFrame());
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == 8) { //Player Layer
            isTouchingPlayer = true; //If we are touching the player, don't automatically turn collision off
        }
    }

    private void OnTriggerExit(Collider other) {
        if (isKinematic && other.gameObject.layer == 8) { //Player Layer
            isTouchingPlayer = false;
            rBody.isKinematic = false;
            myCollider.isTrigger = false;
        }
    }

    private IEnumerator NextFrame() {
        yield return null;
        if (!isTouchingPlayer) {
            rBody.isKinematic = false;
            myCollider.isTrigger = false;
        }
    }
}