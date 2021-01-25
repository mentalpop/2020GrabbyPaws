using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-500)]
public class MenuNavigator : MonoBehaviour
{
    public MenuNode activeMenuNode;
    public bool useMouse = true;
    //public MenuNode activeCancelNode;
    //public MenuNode defaultMenuNode;

    public delegate void MenuEvent(MenuNode menuNode);
    public event MenuEvent OnClose = delegate { };
    public event MenuEvent OnMenuFocus = delegate { };


    public delegate void InputMethodEvent(bool isUsingMouse);
    public event InputMethodEvent OnInputMethodSet = delegate { };

    protected NavButton activeButton;
    protected NavButton heldButton;

    public static MenuNavigator Instance { get; private set; }
    public static bool IsActive { get => Instance.activeMenuNode != null; set { } }

    private bool gamepadDetected = false;
    private Coroutine delayedGamepadCheckRoutine;
    private WaitForSeconds shortWait = new WaitForSeconds(2f);

    private void Awake() {
    //Singleton Pattern
        if (Instance != null && Instance != this) { 
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /*
    private void Awake() {
        if (defaultMenuNode != null) {
            MenuFocus(defaultMenuNode);
        }
    }
    void Start() {
        GamepadCoroutineUpkeep();
    }
    //*/

    public void MenuCancel(MenuNode _mNode) {
        OnClose(activeMenuNode);
        if (_mNode == null) {
            Debug.Log("MenuCancel; _mNode is null");
        } else {
            //Debug.Log("MenuCancel: "+_mNode.name);
            MenuFocus(_mNode);
        }
    }

    public void MenuClose() {
        //if (activeMenuNode != null) {
        //    OnClose(activeMenuNode);
        //    activeMenuNode.OnSelectionAbort -= ActiveMenuNode_OnSelectionAbort;
        //    if (!MouseIsUsing())
        //        activeMenuNode.MenuUnfocus();
        //    activeMenuNode = null;
        //}
        activeMenuNode = null;
    }

    public void MenuFocus(MenuNode _mNode) {
        if (_mNode != null) {
            if (activeMenuNode != null) {
                activeMenuNode.OnSelectionAbort -= ActiveMenuNode_OnSelectionAbort;
                if (!MouseIsUsing())
                    activeMenuNode.MenuUnfocus();
            }
            activeMenuNode = _mNode;
            if (!MouseIsUsing())
                activeMenuNode.MenuFocus();
            activeMenuNode.OnSelectionAbort += ActiveMenuNode_OnSelectionAbort;
            OnMenuFocus(_mNode);
        }
    }

    private void ActiveMenuNode_OnSelectionAbort(MenuNode _fallbackMenu) {
        MenuFocus(_fallbackMenu);
    }

    public void MenuPress() {
        //activeButton = activeMenuNode.GetButtonInFocus();
        heldButton = activeMenuNode.GetButtonInFocus();//activeButton;//activeMenuNode.listController.focusIndex;
        if (heldButton == null) {
            MenuNavigate(MenuNode.NavDir.Accept);
        } else {
            heldButton.buttonStateData.inputPressed = true;
            heldButton.StateUpdate();
        }
    }

    public void MenuRelease() {
        activeButton = activeMenuNode.GetButtonInFocus();
        //Debug.Log("heldButton: "+heldButton.name+", activeButton: "+activeButton.name);
        if (activeButton != null && activeButton == heldButton) {
            activeButton.buttonStateData.inputPressed = false;
            if (activeButton.buttonStateData.hasToggleState)
                activeButton.buttonStateData.stateActive = !activeButton.buttonStateData.stateActive;
            activeButton.Select();
            /*
            if (activeMenuNode.mAccept == null) {
                MenuNavigate(MenuNode.NavDir.Accept);
                //Debug.Log("MenuNode.NavDir.Accept: "+MenuNode.NavDir.Accept);
                activeButton.StateUpdate();
                //Debug.Log("activeButton.buttonStateData.inputPressed: "+activeButton.buttonStateData.inputPressed);
            } else {
                
            }
            //*/
        }
        heldButton = null;
    }

    public void MenuNavigate(MenuNode.NavDir navDir) {
        //Debug.Log("activeMenuNode: "+activeMenuNode);
        if (activeMenuNode != null) {
            activeMenuNode.MenuNavigate(navDir);
        }
        //Debug.Log("activeMenuNode: "+activeMenuNode.name);
    }

    public static bool MouseIsUsing() {
        return !Instance.gamepadDetected; //Instance.useMouse &&
    }

    public static void SetControlPreferences(bool useMouse) {
        Instance.useMouse = useMouse;
        Debug.Log("SetControlPreferences: "+useMouse);
        Instance.GamepadCoroutineUpkeep();
        /*
        if (Instance.useMouse != useMouse) {
            
        }
        //*/
    }

//Input Control Preference
    IEnumerator DelayedGamepadCheck()
     {
        while (!useMouse)
        {
            CheckForGamepad();
            yield return shortWait;
        }
    }

    private void GamepadCoroutineUpkeep() {
        if (delayedGamepadCheckRoutine != null) {
            if (activeMenuNode != null) {
                activeMenuNode.MenuUnfocus();
            }
            StopCoroutine(delayedGamepadCheckRoutine);
            gamepadDetected = false;
            delayedGamepadCheckRoutine = null;
        }
        if (useMouse) {
            OnInputMethodSet(true);
            Debug.Log("GamepadCoroutineUpkeep - OnInputMethodSet(true);");
        } else {
            delayedGamepadCheckRoutine = StartCoroutine(DelayedGamepadCheck());
            Debug.Log("GamepadCoroutineUpkeep - delayedGamepadCheckRoutine: "+delayedGamepadCheckRoutine);
        }
    }

    public void CheckForGamepad() {
        bool _gamepadWasDetected = gamepadDetected;
        gamepadDetected = false;
        string[] temp = Input.GetJoystickNames();
        if (temp.Length > 0) { //Check whether array contains anything
            for (int i =0; i < temp.Length; ++i) { //Iterate over every element
                if (!string.IsNullOrEmpty(temp[i])) { //Check if the string is empty or not
                    gamepadDetected = true;
        //Not empty, controller temp[i] is connected
                    //Debug.Log("Controller " + i + " is connected using: " + temp[i]);
                } else {
        //If it is empty, controller i is disconnected
        //where i indicates the controller number
                    //Debug.Log("Controller: " + i + " is disconnected.");
                }
            }
            if (_gamepadWasDetected != gamepadDetected) {
                Debug.Log("gamepadDetected: "+gamepadDetected);
                OnInputMethodSet(!gamepadDetected);
            }
        }
    }
}