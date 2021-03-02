using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController;
using UnityEngine;
using Cinemachine;

public class PlayerBehaviour : MonoBehaviour
{
    public vThirdPersonInput vThirdPersonInput;
    public Transform cameraTarget;
    public Transform dropTarget;
    public CinemachineVirtualCamera firstPersonCam;

    [HideInInspector] public bool controlsLocked = false;

    private void Update() {
        Inventory.instance.dropPosition = dropTarget.position;
    }

    public void SetLockState(bool doLock) {
        //vThirdPersonInput.lockInput = doLock;
        vThirdPersonInput.SetLockBasicInput(doLock);
        controlsLocked = doLock;
    }
}