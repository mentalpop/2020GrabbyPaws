using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PixelCrushers;
using UnityEngine.SceneManagement;
using PixelCrushers.DialogueSystem;

[DefaultExecutionOrder(-500)]
public class MenuNavigator : MonoBehaviour
{
    public GameObject dialogueManagerPrefab; //Debug - Create this if it doesn't exist

    public StandaloneInputModule standaloneInputModule;
    public string gamepadHorizontalInput;
    public string gamepadVerticalInput;
    public string horizontalInput;
    public string verticalInput;
    public MenuNode activeMenuNode;
    public bool useMouse = true;
    //public MenuNode activeCancelNode;
    //public MenuNode defaultMenuNode;

    public delegate void MenuEvent(MenuNode menuNode);
    public event MenuEvent OnClose = delegate { };
    public event MenuEvent OnMenuFocus = delegate { };


    public delegate void InputMethodEvent(bool isUsingMouse);
    public event InputMethodEvent OnInputMethodSet = delegate { };
    public event InputMethodEvent OnInputMethodIgnoreChange = delegate { };

    protected NavButton activeButton;
    protected NavButton heldButton;

    public static MenuNavigator Instance { get; private set; }
    public static bool IsActive { get => Instance.activeMenuNode != null; set { } }
    private bool preferMouse = false;
    //private bool gamepadDetected = false;
    //private Coroutine delayedGamepadCheckRoutine;
    //private WaitForSeconds shortWait = new WaitForSeconds(2f);
    private InputDeviceManager inputDeviceManager;
    private int mostRecentInputDetected = -1;
    private int buttonID = -1;

    private void Awake() {
    //Singleton Pattern
        if (Instance != null && Instance != this) { 
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update() {
        if (Input.mouseScrollDelta.y != 0f) {
            MouseOrKeyboardDetected();
        }
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= SceneManager_sceneUnloaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        DialogueSystemController dialogueSystemController = FindObjectOfType<DialogueSystemController>();
        if (dialogueSystemController == null) {
            GameObject newGO = Instantiate(dialogueManagerPrefab, transform.parent, false);
            Debug.LogWarning("\"Dialogue Manager New\" prefab not found in: " + scene.name + ", creating one for Debug Purposes");
        }
        inputDeviceManager = FindObjectOfType<InputDeviceManager>();
        if (inputDeviceManager == null) {
            Debug.LogWarning("inputDeviceManager not found in: " + scene.name);
            useMouse = true; //Enable this as a fallback
        } else {
            //useMouse = false; //This should not be set to false - it causes the mouse to not work
            inputDeviceManager.onUseJoystick.AddListener(JoystickDetected);
            inputDeviceManager.onUseKeyboard.AddListener(MouseOrKeyboardDetected);
            inputDeviceManager.onUseMouse.AddListener(MouseOrKeyboardDetected);
        }
    }

    private void SceneManager_sceneUnloaded(Scene arg0) {
        if (inputDeviceManager != null) {
            inputDeviceManager.onUseJoystick.RemoveListener(JoystickDetected);
            inputDeviceManager.onUseKeyboard.RemoveListener(MouseOrKeyboardDetected);
            inputDeviceManager.onUseMouse.RemoveListener(MouseOrKeyboardDetected);
        }
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

    public void MenuPress(int _buttonID) {
        if (buttonID == -1) {
            buttonID = _buttonID;
            //activeButton = activeMenuNode.GetButtonInFocus();
            heldButton = activeMenuNode.GetButtonInFocus();//activeButton;//activeMenuNode.listController.focusIndex;
            if (heldButton == null) {
                MenuNavigate(MenuNode.NavDir.Accept);
            } else {
                heldButton.buttonStateData.inputPressed = true;
                heldButton.StateUpdate();
            }
        }
    }

    public void MenuRelease(int _buttonID) {
        if (buttonID == _buttonID) {
            activeButton = activeMenuNode.GetButtonInFocus();
            //Debug.Log("heldButton: "+heldButton.name+", activeButton: "+activeButton.name);
            if (activeButton != null && activeButton == heldButton) {
                activeButton.buttonStateData.inputPressed = false;
                if (activeButton.buttonStateData.hasToggleState)
                    activeButton.buttonStateData.stateActive = !activeButton.buttonStateData.stateActive;
                activeButton.Select(buttonID);
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
        }
        buttonID = -1;
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
        return Instance.useMouse;// !Instance.gamepadDetected; //Instance.useMouse &&
    }

    public static void SetControlPreferences(bool useMouse) {
        Instance.preferMouse = useMouse;
        Instance.CheckUserPreference();
        if (Instance.preferMouse) { //Create Ignore Gamepad Prompt
            Instance.OnInputMethodIgnoreChange(false);
        }
    }

//Input Control Preference
    private void JoystickDetected() {
        //Debug.Log("UseJoystick");
        mostRecentInputDetected = 0;
        Instance.CheckUserPreference();
    }

    private void MouseOrKeyboardDetected() {
        //Debug.Log("UseMouseOrKeyboard");
        mostRecentInputDetected = 1;
        Instance.CheckUserPreference();
    }

    private void CheckUserPreference() {
        //Debug.Log("CheckUserPreference: " + mostRecentInputDetected + ", preferMouse: " + preferMouse);
        /*
        if (delayedGamepadCheckRoutine != null) { //If the user was using the gamepad and has now swapped //useMouse && 
            if (activeMenuNode != null) {
                activeMenuNode.MenuUnfocus();
            }
            StopCoroutine(delayedGamepadCheckRoutine);
            gamepadDetected = false;
            delayedGamepadCheckRoutine = null;
        }
        //*/
        //Debug.Log("useMouse: " + useMouse + ", mostRecentInputDetected: " + mostRecentInputDetected+", preferMouse: "+preferMouse);
        if (useMouse && mostRecentInputDetected == 0) { //A joystick was detected and the user doesn't prefer the mouse
            if (preferMouse) {
                OnInputMethodIgnoreChange(false);
            } else {
                SwitchToGamepad();
            }
            //if (useMouse) { //And is currently using the gamepad
            //delayedGamepadCheckRoutine = StartCoroutine(DelayedGamepadCheck());
            //}
        }
        if (!useMouse && (preferMouse || mostRecentInputDetected == 1)) { //If the most recent input was the mouse
            //if (!useMouse) { //And is not currently using the mouse
                SwitchToMouse();
            //}
        }

        /*
        if (preferMouse) { //If the user would prefer to use the mouse
            if (!useMouse) { //And is not currently using the mouse
                SwitchToMouse();
            }
        } else { //If the user would prefer to use the gamepad
            if (useMouse) { //And is currently using the gamepad
                delayedGamepadCheckRoutine = StartCoroutine(DelayedGamepadCheck());
            }
        }
        //*/
        //Debug.Log("useMouse: " + useMouse);
    }

    /*
    IEnumerator DelayedGamepadCheck() {
        while (!useMouse) {
            CheckForGamepad();
            yield return shortWait;
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
            Debug.Log("gamepadDetected: " + gamepadDetected);
            if (_gamepadWasDetected != gamepadDetected) {
                SwitchToGamepad();
            }
        }
    }
    //*/

    private void SwitchToMouse() {
        Debug.Log("SwitchToMouse");
        if (activeMenuNode != null) {
            activeMenuNode.MenuUnfocus();
        }
        useMouse = true;
        standaloneInputModule.horizontalAxis = horizontalInput;
        standaloneInputModule.verticalAxis = verticalInput;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        OnInputMethodSet(true);
    }

    private void SwitchToGamepad() {
        Debug.Log("SwitchToGamepad");
        useMouse = false;
        standaloneInputModule.horizontalAxis = gamepadHorizontalInput;
        standaloneInputModule.verticalAxis = gamepadVerticalInput;
        OnInputMethodSet(false); //!gamepadDetected
    }
}