using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInputLegacy : MonoBehaviour
{
    public MenuNavigator menuNavigator;
    public string inputActivate;
    public string inputHorizontal;
    public string inputVertical;
    public float axisThreshold = 0.025f;
    public string inputForward;
    public string inputBackward;
    public string inputCancel;

    //private List<string> inputs = new List<string>();
    private bool axisDown = false;
    /*
    private bool hAxisDown = false;
    private bool vAxisDown = false;
    //*/

    private void Start() {
        if (menuNavigator == null) {
            Debug.LogWarning("menuNavigator is null, attempting to use FindObjectOfType<MenuNavigator> but this should be fixed");
            menuNavigator = UI.Instance.menuNavigator;//FindObjectOfType<MenuNavigator>();
        }
    }

    private void Update() {
        if (MenuNavigator.MouseIsUsing() || !MenuNavigator.IsActive) //Ignore Input if user is using Mouse or if the Menu is not active
            return;
        Vector2 inputVector = new Vector2(Input.GetAxis(inputHorizontal), Input.GetAxis(inputVertical));
        if (axisDown) {
            if (inputVector.magnitude < axisThreshold) {
                axisDown = false;
            }
        } else {
            if (inputVector.magnitude > axisThreshold) {
                float inputDirection = UI.Direction(Vector2.zero, inputVector) + 180f;
                axisDown = true;
                //Debug.Log("inputDirection: "+inputDirection);
                if (inputDirection >= 45 && inputDirection < 135) {
                    menuNavigator.MenuNavigate(MenuNode.NavDir.Up);
                    //Debug.Log("MenuNode.NavDir.Up");
                } else if (inputDirection >= 135 && inputDirection < 225) {
                    menuNavigator.MenuNavigate(MenuNode.NavDir.Left);
                    //Debug.Log("MenuNode.NavDir.Left");
                } else if (inputDirection >= 225 && inputDirection < 315) {
                    menuNavigator.MenuNavigate(MenuNode.NavDir.Down);
                    //Debug.Log("MenuNode.NavDir.Down");
                } else {
                    menuNavigator.MenuNavigate(MenuNode.NavDir.Right);
                    //Debug.Log("MenuNode.NavDir.Right");
                }
            }
        }
    /*
    //Horizontal
        if (inputHorizontal != "") {
            float _hAxis = Input.GetAxis(inputHorizontal);
            if (hAxisDown) {
                if (Mathf.Abs(_hAxis) < axisThreshold) {
                    hAxisDown = false;
                    Debug.Log("hAxisDown: "+hAxisDown);
                }
            } else {
                if (_hAxis > axisThreshold) {
                    menuNavigator.MenuNavigate(MenuNode.NavDir.Right);
                    Debug.Log("MenuNode.NavDir.Right: "+MenuNode.NavDir.Right);
                    hAxisDown = true;
                }
                if (_hAxis < -axisThreshold) {
                    menuNavigator.MenuNavigate(MenuNode.NavDir.Left);
                    Debug.Log("MenuNode.NavDir.Left: "+MenuNode.NavDir.Left);
                    hAxisDown = true;
                }
            }
        }
    //Vertical
        if (inputVertical != "") {
            float _vAxis = Input.GetAxis(inputVertical);
            //Debug.Log("_vAxis: "+_vAxis);
            if (vAxisDown) {
                if (Mathf.Abs(_vAxis) < axisThreshold) {
                    vAxisDown = false;
                    Debug.Log("vAxisDown: "+vAxisDown);
                }
            } else {
                if (_vAxis > axisThreshold) {
                    menuNavigator.MenuNavigate(MenuNode.NavDir.Up);
                    Debug.Log("MenuNode.NavDir.Up: "+MenuNode.NavDir.Up);
                    vAxisDown = true;
                }
                if (_vAxis < -axisThreshold) {
                    menuNavigator.MenuNavigate(MenuNode.NavDir.Down);
                    Debug.Log("MenuNode.NavDir.Down: "+MenuNode.NavDir.Down);
                    vAxisDown = true;
                }
            }
        }
    //*/
    //Other
        if (inputForward != "" && Input.GetButtonUp(inputForward)) {
            menuNavigator.MenuNavigate(MenuNode.NavDir.Forward);
            //Debug.Log("MenuNode.NavDir.Forward: "+MenuNode.NavDir.Forward);
        }
        if (inputBackward != "" && Input.GetButtonUp(inputBackward)) {
            menuNavigator.MenuNavigate(MenuNode.NavDir.Backward);
            //Debug.Log("MenuNode.NavDir.Backward: "+MenuNode.NavDir.Backward);
        }
        if (inputCancel != "" && Input.GetButtonUp(inputCancel)) {
            menuNavigator.MenuNavigate(MenuNode.NavDir.Cancel);
            //Debug.Log("MenuNode.NavDir.Cancel: "+MenuNode.NavDir.Cancel);
        }
    //inputActivate
        if (inputActivate != "") {
            //menuNavigator.MenuActivate();
            if (Input.GetButtonDown(inputActivate)) {
                //Debug.Log("MenuPress");
                menuNavigator.MenuPress();
                //Debug.Log("MenuNode.NavDir.MenuActivate");
            }
            if (Input.GetButtonUp(inputActivate)) {
                //Debug.Log("MenuRelease");
                menuNavigator.MenuRelease();
            }
        }
            
        /*
        for (int i = 0; i < inputs.Count; i++) {
            if (inputs[i] != "" && Input.GetButtonUp(inputs[i])) {
                menuNavigator.MenuNavigate((MenuNode.NavDir)i);
            }
        }
        //*/
    }
    
        /*
    private void Awake() {
        inputs.Add(inputLeft);
        inputs.Add(inputRight);
        inputs.Add(inputUp);
        inputs.Add(inputDown);
        inputs.Add(inputForward);
        inputs.Add(inputBackward);
        inputs.Add(inputCancel);
    }
        //*/
}