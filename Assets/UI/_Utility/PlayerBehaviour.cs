using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public vThirdPersonInput vThirdPersonInput;
    public Transform cameraTarget;
    public Transform dropTarget;

    private void Update() {
        Inventory.instance.dropPosition = dropTarget.position;
    }

    public void SetLockState(bool doLock) {
        vThirdPersonInput.lockInput = doLock;
    }
}