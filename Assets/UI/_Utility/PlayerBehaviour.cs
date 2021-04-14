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

    private MenuNavigator MenuNavigator;
//Define player controls;
    private GenericInput horizontalInput = new GenericInput("Horizontal", "", "");
    private GenericInput verticallInput = new GenericInput("Vertical", "", "");
    private GenericInput jumpInput = new GenericInput("Space", "", "");
    private GenericInput sprintInput = new GenericInput("LeftShift", "", "");

    private GenericInput horizontalInputGamepad = new GenericInput("Horizontal", "LeftAnalogHorizontal", "Horizontal");
    private GenericInput verticallInputGamepad = new GenericInput("Vertical", "LeftAnalogVertical", "Vertical");
    private GenericInput jumpInputGamepad = new GenericInput("Space", "X", "X");
    private GenericInput sprintInputGamepad = new GenericInput("LeftShift", "LeftStickClick", "LeftStickClick");
    //private GenericInput rollInput = new GenericInput("Q", "B", "B");
    //private GenericInput strafeInput = new GenericInput("Tab", "RightStickClick", "RightStickClick");
    //private GenericInput crouchInput = new GenericInput("C", "Y", "Y");

    private void Update() {
        Inventory.instance.dropPosition = dropTarget.position;
    }

    private void Awake() {
        MenuNavigator = FindObjectOfType<MenuNavigator>();
        if (MenuNavigator == null) {
            Debug.LogWarning("MenuNavigator is null");
        } else {
            MenuNavigator_OnInputMethodSet(MenuNavigator.MouseIsUsing()); //Set initial control labels
        }
    }

    private void OnEnable() {
        MenuNavigator.OnInputMethodSet += MenuNavigator_OnInputMethodSet;
    }

    private void OnDisable() {
        MenuNavigator.OnInputMethodSet -= MenuNavigator_OnInputMethodSet;
    }

    public void SetLockState(bool doLock) {
        //vThirdPersonInput.lockInput = doLock;
        vThirdPersonInput.SetLockBasicInput(doLock);
        controlsLocked = doLock;
    }

    private void MenuNavigator_OnInputMethodSet(bool isUsingMouse) { //If the user is not using the gamepad, set the labels to ignore input from the gamepad
        vThirdPersonInput.horizontalInput = isUsingMouse ? horizontalInput : horizontalInputGamepad;
        vThirdPersonInput.verticallInput = isUsingMouse ? verticallInput : verticallInputGamepad;
        vThirdPersonInput.jumpInput = isUsingMouse ? jumpInput : jumpInputGamepad;
        vThirdPersonInput.sprintInput = isUsingMouse ? sprintInput : sprintInputGamepad;
    }
}