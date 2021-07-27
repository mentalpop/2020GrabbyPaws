using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using Invector.vCharacterController;
using System;

#region Enums
public enum NightPhases
{
    p1Twilight,
    p2MoonRise,
    p3Midnight,
    p4MoonFall,
    p5Dawn
}

public enum QuestNames
{
    q001TwilightCottonCandy,
    q001TwilightCottonCandyEndFlag
    /*,
    q002TwilightCCandy,
    q010TwilightCCandy,
    q004Twilight,
    //*/
}

public enum Secrets
{
    s001Test,
    s002Test,
    s003Test,
    s004Test
}

public enum NPCLocations
{
    /*
    npc001Twilight01Riven,
    npc001Twilight02Riven,
    npc001Twilight03Riven,
    npc001Twilight04Riven,
    npc001Twilight05Riven,
    npc002,
    q010TwilightCCandy,
    q004Twilight,
    //*/
}
#endregion

[DefaultExecutionOrder(-100)]
public class UI : MonoBehaviour
{
    public static string saveFileName = "grabby.paws";

    public MenuNavigator menuNavigator;
    public Sonos sonosAudio;
    public GameObject HUD;
    public LappyMenu lappy;
    public ConfirmationWindow confirmationWindow;
    public CinemachineBlendDefinition firstPersonBlend;
    //public FlagRepository flagRepository;
    [Header("Readables")]
    public Readable book;
    public Readable sign;
    public ReadablePC pc;
    [Header("Currency")]
    public Currency currency;
    public CurrencyDisplay currencyDisplay;
    [Header("Inventory")]
    public InventoryDisplay inventoryDisplay;
    public Inventory inventory;
    [HideInInspector] public CinemachineFreeLook cFreeLook;
    //[HideInInspector] public vThirdPersonCamera thirdPersonCamera;
    [Header("VIDEO Options")]
    public ConstrainedIntPref screenMode;
    public ConstrainedIntPref resolution;
    public ConstrainedIntPref quality;
    [Header("MISC Options")]
    public ConstrainedFloatPref mouseSensitivity;
    public Vector2 cameraSpeedMin;
    public Vector2 cameraSpeedMax;
    public ConstrainedIntPref uiScale;
    public ConstrainedIntPref textSize;
    public ConstrainedIntPref fontChoice;
    public ConstrainedIntPref textPrintSpeed;
    public ConstrainedIntPref cameraInversion;
    public ConstrainedIntPref inputPreference;

    [HideInInspector] public PlayerBehaviour player;

    private int currentFile = 0;

    public delegate void FileIOEvent(int fileNum);
    public event FileIOEvent OnSave = delegate { };
    public event FileIOEvent OnLoad = delegate { };

    public delegate void TextScaledEvent(float fontScale);
    public event TextScaledEvent OnTextScaled = delegate { };

    public delegate void FontSelectEvent(int fontChoice);
    public event FontSelectEvent OnFontChoice = delegate { };

    public delegate void PrintSpeedEvent(float speed);
    public event PrintSpeedEvent OnPrintSpeedSet = delegate { };

    public delegate void UIScaleEvent(float scale);
    public event UIScaleEvent OnUIScaled = delegate { };

    //private bool doShowCurrencyDisplay = false;
    private MenuNode inventoryMenuNode;
    private MenuNode pauseMenuNode;
    private GameObject lockUI = null;
    private CinemachineBrain cBrain;
    private Camera gameCamera;
    private CinemachineBlendDefinition defaultBlend;
    private List<GameObject> mouseCursorUsers = new List<GameObject>();
    //public bool lockControls = false;
    public bool lockControls { get; private set; }
    ICinemachineCamera previousCamera = null;
    private bool firstPersonCamera = false;

    public static UI Instance { get; private set; }

    #region Unity Messages
    private void OnEnable() {
        currency.OnCashChanged += OnCurrencyChanged;
        MenuNavigator.Instance.OnInputMethodSet += Instance_OnInputMethodSet;
    }

    private void OnDisable() {
        currency.OnCashChanged -= OnCurrencyChanged;
        MenuNavigator.Instance.OnInputMethodSet -= Instance_OnInputMethodSet;
        if (cBrain != null) {
            cBrain.m_CameraActivatedEvent.RemoveListener(delegate { OnCameraActivated(); });
        }
    }

    private void Awake() {
        //Singleton Pattern
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        //Debug.Log("Instance: "+Instance);
        LoadOptionsData();
        DontDestroyOnLoad(gameObject);
    }

    void Update() {
        if (lockUI == null) { //No GameObject is currently locking the UI
            if (MenuNavigator.MouseIsUsing()) {
                //Open / Close menus
                if (!lappy.gameObject.activeSelf && Input.GetKeyDown(KeyCode.Tab)) { //"Inventory"
                    ShowInventoryDisplay();
                }
                if (Input.GetKeyDown(KeyCode.Escape)) { //"Kwit"
                    LappyMenuToggle(false);
                }
            } else {
                if (!lappy.gameObject.activeSelf && Input.GetButtonDown("Back")) {
                    ShowInventoryDisplay();
                }
                if (Input.GetButtonDown("Start")) {
                    LappyMenuToggle(false);
                }
                //FirstPerson Camera
                if (!lappy.gameObject.activeSelf && !inventoryDisplay.gameObject.activeSelf) { //Can't change state of First-Person Cam while paused
                    if (firstPersonCamera) {
                        //Disable on Release
                        if (Mathf.Round(Input.GetAxisRaw("Triggers")) < 1) {
                            if (cBrain != null) {
                                //cBrain.transform.position = player.firstPersonCam.gameObject.transform.position;
                                //cBrain.transform.rotation = player.firstPersonCam.gameObject.transform.rotation;
                                gameCamera.cullingMask |= 1 << LayerMask.NameToLayer("Player"); // Show Player Layer
                                previousCamera.VirtualCameraGameObject.SetActive(true);
                                player.firstPersonCam.gameObject.SetActive(false);
                                player.SetLockState(false);
                                //CameraSetInputLabels(); Can't call this here, cFreeLook is still null
                                Invoke(nameof(ResetCBrainDefaultBlend), firstPersonBlend.BlendTime); //After the Camera transition is over, reset the blend back to the default
                                previousCamera = null;
                                firstPersonCamera = false;
                            }
                        }
                    } else {
                        //Activate on Press
                        if (Mathf.Round(Input.GetAxisRaw("Triggers")) > 0) {
                            if (cBrain != null) {
                                previousCamera = cBrain.ActiveVirtualCamera;
                                player.SetLockState(true);
                                CameraSetInputLabels();
                                previousCamera.VirtualCameraGameObject.SetActive(false);
                                player.firstPersonCam.gameObject.SetActive(true);
                                //player.firstPersonCam.MoveToTopOfPrioritySubqueue();
                                firstPersonCamera = true;
                                cBrain.m_DefaultBlend = firstPersonBlend;
                                gameCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Player")); // Hide Player Layer
                                //previousCamera.VirtualCameraGameObject.transform.rotation = player.firstPersonCam.gameObject.transform.rotation;
                                //previousCamera.VirtualCameraGameObject.transform.rotation = player.transform.rotation;
                            }
                        }
                    }
                }
            }
        }
    }

    private void ResetCBrainDefaultBlend() {
        if (cBrain.m_DefaultBlend.BlendCurve == firstPersonBlend.BlendCurve && cBrain.m_DefaultBlend.BlendTime == firstPersonBlend.BlendTime) { //No direct overload of ==, so check both these properties
            cBrain.m_DefaultBlend = defaultBlend;
        }
    }
    #endregion

    #region Options, Get / Set Methods
    //VIDEO OPTIONS
    public static void SetWindowMode(int choiceMade) {
        Instance.screenMode.Write(choiceMade);
        switch (choiceMade) {
            case 0: Screen.fullScreen = false; break;
            case 1: Screen.fullScreen = true; break;
        }
    }

    public static void SetResolution(int choiceMade) {
        Instance.resolution.Write(choiceMade);
        switch (choiceMade) {
            case 0: Screen.SetResolution(1920, 1080, Screen.fullScreen); break;
            case 1: Screen.SetResolution(1600, 900, Screen.fullScreen); break;
            case 2: Screen.SetResolution(1366, 768, Screen.fullScreen); break;
            case 3: Screen.SetResolution(1280, 720, Screen.fullScreen); break;
            case 4: Screen.SetResolution(1176, 664, Screen.fullScreen); break;
        }
    }

    public static void SetQuality(int choiceMade) {
        Instance.quality.Write(choiceMade);
        //Debug.Log("quality: "+choiceMade);
    }

    //MISC OPTIONS
    public void RestoreAllDefaults() {
        SetWindowMode(screenMode.defaultValue);
        SetResolution(resolution.defaultValue);
        SetQuality(quality.defaultValue);
        SetCameraSensitivity(mouseSensitivity.defaultValue);
        SetCameraInversion(cameraInversion.defaultValue);
        SetUIScale(uiScale.defaultValue);
        SetFontChoice(fontChoice.defaultValue);
        SetTextSize(textSize.defaultValue);
        SetPrintSpeed(textPrintSpeed.defaultValue);
        //SetInputPreference(inputPreference.defaultValue);
        //method(variable.defaultValue);
        lappy.SetBackground(lappy.lappySelectedBG.defaultValue);
        //Audio
        Sonos.SetVolume(AudioType.Effect, 1f);
        Sonos.SetVolume(AudioType.Music, 1f);
        Sonos.SetVolume(AudioType.Voice, 1f);
        Sonos.VolumeMaster = 1f;
    }

    public static void SetCameraSensitivity(float _cameraSensitivity) {
        Instance.mouseSensitivity.Write(_cameraSensitivity);
        Instance.UpdateCameraSettings();
    }

    public static void SetCameraInversion(int _cameraInversion) {
        Instance.cameraInversion.Write(_cameraInversion);
        Instance.UpdateCameraSettings();
    }

    private void UpdateCameraSettings() {
        if (cBrain != null) {
            switch (cBrain.ActiveVirtualCamera) {
                /*
                case CinemachineVirtualCamera cVc:
                    cVc.m_
                    break;
                //*/
                case CinemachineFreeLook cFc:
                    //Sensitivity
                    cFc.m_XAxis.m_MaxSpeed = cameraSpeedMin.x + (cameraSpeedMax.x - cameraSpeedMin.x) * mouseSensitivity.value;
                    cFc.m_YAxis.m_MaxSpeed = cameraSpeedMin.y + (cameraSpeedMax.y - cameraSpeedMin.y) * mouseSensitivity.value;
                    //Axis Inversion
                    cFc.m_XAxis.m_InvertInput = Instance.cameraInversion.value == 2 || Instance.cameraInversion.value == 3; //Normal = 0, Inverted = 1
                    cFc.m_YAxis.m_InvertInput = Instance.cameraInversion.value == 1 || Instance.cameraInversion.value == 3; //Normal = 0, Inverted = 1
                    break;
            }
            /*
            cFreeLook.m_XAxis.m_MaxSpeed = cameraSpeedMin.x + (cameraSpeedMax.x - cameraSpeedMin.x) * mouseSensitivity.value;
            cFreeLook.m_YAxis.m_MaxSpeed = cameraSpeedMin.y + (cameraSpeedMax.y - cameraSpeedMin.y) * mouseSensitivity.value;
            //*/
        } else {
            Debug.Log("Tried to update cFreeLook, but cBrain is null");
        }
    }

    public static void SetUIScale(int choiceMade) {
        Instance.uiScale.Write(choiceMade); //Set and Save
        float _uiScale = GetUIScale();
        Instance.lappy.transform.localScale = new Vector2(_uiScale, _uiScale);
        Instance.OnUIScaled(_uiScale);
    }

    public static float GetUIScale() {
        float _uiScale = 1f;
        switch (Instance.uiScale.value) {
            case 0: _uiScale = 1f; break;
            case 1: _uiScale = 1.125f; break;
            case 2: _uiScale = 1.25f; break;
        }
        return _uiScale;
    }

    public static void SetFontChoice(int choiceMade) {
        Instance.fontChoice.Write(choiceMade);
        Instance.OnFontChoice(choiceMade);
    }

    public static void SetTextSize(int choiceMade) {
        Instance.textSize.Write(choiceMade);
        Instance.OnTextScaled(GetTextSize());
    }
    public static float GetTextSize() {
        float _textSize = 1f;
        switch (Instance.textSize.value) {
            case 0: _textSize = 1f; break;
            case 1: _textSize = 1.5f; break;
            case 2: _textSize = 2f; break;
        }
        return _textSize;
    }

    public static void SetPrintSpeed(int choiceMade) {
        Instance.textPrintSpeed.Write(choiceMade);
        Instance.OnPrintSpeedSet(GetPrintSpeed());
    }

    public static float GetPrintSpeed() {
        float _speed = 1f;
        switch (Instance.uiScale.value) {
            case 0: _speed = 1f; break;
            case 1: _speed = 2f; break;
            case 2: _speed = 4f; break;
        }
        return _speed;
    }

    public static void SetInputPreference(int choiceMade) {
        Instance.inputPreference.Write(choiceMade);
        MenuNavigator.SetControlPreferences(choiceMade == 1); //0 - Use Gamepad (if present), 1 - Use Mouse
    }
    #endregion

    #region UI State
    public void DisplayBook(ReadableData rData) {
        book.gameObject.SetActive(true);
        SetControlState(true, book.gameObject);
        book.Unpack(rData);
    }

    public void DisplaySign(ReadableData rData) {
        sign.gameObject.SetActive(true);
        SetControlState(true, sign.gameObject);
        sign.Unpack(rData);
    }

    public void DisplayPC(ReadablePCData pcData) {
        pc.gameObject.SetActive(true);
        SetControlState(true, pc.gameObject);
        pc.Unpack(pcData);
    }

    public void OpenConversationInLappy(string conversationID) {
        if (!lappy.gameObject.activeSelf) {
            ActivateLappy();
        }
        lappy.StartConversation(conversationID);
    }

    public void LappyMenuToggle(bool _override) {
        bool doActivateLappy = _override || !lappy.gameObject.activeSelf;
        if (doActivateLappy) {
            //Show / Hide the HUD
            ActivateLappy();
        } else {
            //Close Lappy
            if (lappy.Close()) {
                pauseMenuNode = MenuNavigator.Instance.activeMenuNode;
                MenuNavigator.Instance.MenuClose();
            }
        }
    }

    public void RefocusInventory() {
        if (inventoryDisplay.gameObject.activeSelf) {
            //If the Inventory was open, refocus it
            MenuNavigator.Instance.MenuFocus(inventoryMenuNode);
        }
    }

    private void ActivateLappy() {
        if (inventoryDisplay.gameObject.activeSelf) {
            //If the Inventory was open, make note of which node was active to 
            inventoryMenuNode = MenuNavigator.Instance.activeMenuNode;
        }
        SetControlState(true, lappy.gameObject);
        lappy.gameObject.SetActive(true);
        if (pauseMenuNode != null) {
            MenuNavigator.Instance.MenuFocus(pauseMenuNode);
        }
    }

    public void ShowInventoryDisplay() { //TODO: Make private
                                         //Show / Hide the HUD
        bool menuIsActive = !inventoryDisplay.gameObject.activeSelf;
        if (menuIsActive) {
            inventoryDisplay.gameObject.SetActive(true);
            if (currencyDisplay.gameObject.activeSelf) {
                currencyDisplay.Open();
            } else {
                currencyDisplay.gameObject.SetActive(true);
            }
        } else {
            inventoryDisplay.Close();
            currencyDisplay.Close();
        }
        SetControlState(menuIsActive, inventoryDisplay.gameObject);
    }

    public static void AssignPlayerAndCamera(PlayerBehaviour playerBehaviour, CinemachineBrain cinemachineBrain) {
        Instance.AssignPlayer_AndCamera(playerBehaviour, cinemachineBrain);
    }

    private void AssignPlayer_AndCamera(PlayerBehaviour playerBehaviour, CinemachineBrain cinemachineBrain) {
        player = playerBehaviour;
        cBrain = cinemachineBrain;
        defaultBlend = cBrain.m_DefaultBlend;
        gameCamera = cBrain.GetComponent<Camera>();
        cBrain.m_CameraActivatedEvent.AddListener(delegate { OnCameraActivated(); });
    }

    private void OnCameraActivated() {
        cFreeLook = cBrain.ActiveVirtualCamera as CinemachineFreeLook;
        switch (cBrain.ActiveVirtualCamera) {
            case CinemachineVirtualCamera cVc:
                //Debug.Log("CinemachineVirtualCamera");
                if (cVc.m_LookAt == null) {
                    cVc.m_LookAt = player.cameraTarget;
                    //cVc.m_Follow = player.cameraTarget; //Comment this out; it's the problem
                }
                break;
            case CinemachineFreeLook cFc:
                CameraSetInputLabels();
                if (cFc.gameObject.CompareTag("SetPlayerFocus")) {
                    Debug.Log("CinemachineFreeLook Activated with SetPlayerFocus tag; assigning Target to Player");
                    cFc.m_LookAt = player.cameraTarget;
                    cFc.m_Follow = player.cameraTarget;
                }
                break;
        }
        UpdateCameraSettings();
    }

    public static void SetControlState(bool lockMouse, GameObject gameObject) {
        if (lockMouse) {
            bool _alreadyIn = false;
            //Prevent redundant addition of locking objects
            foreach (var item in Instance.mouseCursorUsers) {
                if (item == gameObject) {
                    _alreadyIn = true;
                    break;
                }
            }
            if (!_alreadyIn) {
                Instance.mouseCursorUsers.Add(gameObject);
            }
        } else {
            Instance.mouseCursorUsers.Remove(gameObject);
        }
        Instance.lockControls = false;
        //Debug.Log("Instance.mouseCursorUsers.Count: "+Instance.mouseCursorUsers.Count);
        if (Instance.mouseCursorUsers.Count > 0) {
            Instance.lockControls = true;
            //if (!MenuNavigator.MouseIsUsing()) {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            //}
        } else {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        //Player
        if (Instance.player != null) {
            Instance.player.SetLockState(Instance.lockControls);
        }
        //Camera
        Instance.CameraSetInputLabels();
        /*
        Debug.Log("Instance.thirdPersonCamera: "+Instance.thirdPersonCamera);
        Debug.Log("Instance.cBrain: "+Instance.cBrain);
        //*/
        //*
        //if (Instance.thirdPersonCamera == null) {
        /*
        } else {
    //Using thirdPersonCamera
            Instance.thirdPersonCamera.enabled = !suppressCamera;
        }
        //*/
    }

    private void Instance_OnInputMethodSet(bool isUsingMouse) {
        CameraSetInputLabels();
    }

    private void CameraSetInputLabels() {
        //Using Cinemachine Freelook?
        if (Instance.cFreeLook != null) {
            //Debug.Log("suppressCamera: "+suppressCamera);
            //CinemachineFreeLook currentCamera = Instance.cFreeLook;//Instance.cFreeLook.ActiveVirtualCamera as CinemachineFreeLook;
            //Debug.Log("currentCamera: "+currentCamera);
            if (Instance.player.controlsLocked) { //Instance.lockControls
                //Debug.Log("lockControls: "+lockControls);
            //X Axis
                Instance.cFreeLook.m_XAxis.m_InputAxisName = "";
                Instance.cFreeLook.m_XAxis.m_MaxSpeed = 0;
            //Y Axis
                Instance.cFreeLook.m_YAxis.m_InputAxisName = "";
                Instance.cFreeLook.m_YAxis.m_MaxSpeed = 0;
            } else {
                //Control Preferences
                Debug.Log("CameraSetInputLabels: " + MenuNavigator.MouseIsUsing());
                if (MenuNavigator.MouseIsUsing()) {
                    cFreeLook.m_XAxis.m_InputAxisName = "Mouse X";
                    cFreeLook.m_YAxis.m_InputAxisName = "Mouse Y";
                } else {
                    cFreeLook.m_XAxis.m_InputAxisName = "RightAnalogHorizontal";
                    cFreeLook.m_YAxis.m_InputAxisName = "RightAnalogVertical";
                }
                Instance.UpdateCameraSettings();
            }
        }
    }

    public static ConfirmationWindow RequestConfirmation(ConfirmationPromptData _data, MenuNode _menuOnDisable) {
        Instance.confirmationWindow.gameObject.SetActive(true);
        Instance.confirmationWindow.Unpack(_data, _menuOnDisable);
        return Instance.confirmationWindow; //Allow calling object to subscribe to the result
    }

    private void OnCurrencyChanged() {
        //doShowCurrencyDisplay = true;
        currencyDisplay.UpdateCashDisplay();
        currencyDisplay.gameObject.SetActive(true);
        /*
        bool showDisplay = doShowCurrencyDisplay || InventoryDisplay.gameObject.activeSelf;
        if (showDisplay) {
            currencyDisplay.gameObject.SetActive(true);
        } else {
            currencyDisplay.Close();
        }
        //*/
    }

    public static void LockUI(GameObject _gameObject) {
        Instance.lockUI = _gameObject;
    }

    public static void UnlockUI() {
        Instance.lockUI = null;
    }
    #endregion

    #region Save / Load

    /*
    public static void SaveOptionsData() {
//Sonos Settings
        
//VIDEO Options

    }
    //*/

    private void LoadOptionsData() {
        SetQuality(quality.Read());
        SetWindowMode(screenMode.Read());
        SetResolution(resolution.Read());
        SetCameraSensitivity(mouseSensitivity.Read());
        SetCameraInversion(cameraInversion.Read());
        SetUIScale(uiScale.Read());
        SetTextSize(textSize.Read());
        SetFontChoice(fontChoice.Read());
        SetPrintSpeed(textPrintSpeed.Read());
        Debug.Log("Load inputPreference: " + inputPreference.Read());
        SetInputPreference(inputPreference.Read());
    }

    private string ssScene = "level";
    private string ssSpawnPoint = "spawnPoint";

    public void SaveGameData(int fileNum) {
        Debug.Log("Game Saved: " + Application.persistentDataPath);
        OnSave?.Invoke(fileNum);
        ES3.Save(ssScene, SceneTransitionHandler.instance.currentScene);
        ES3.Save(ssSpawnPoint, SceneTransitionHandler.instance.spawnPoint);
    }

    public void LoadGameData(int fileNum) {
        Debug.Log("Game Loaded");
        OnLoad?.Invoke(fileNum);
        string _scene = (string)ES3.Load(ssScene);
        SpawnPoints _spawnPoint = (SpawnPoints)ES3.Load(ssSpawnPoint);
        SceneTransitionHandler.SceneGoto(_scene, _spawnPoint);
    }

    public static int GetCurrentFile() {
        return Instance.currentFile;
    }
    #endregion

    public static float Direction(Vector2 _a, Vector2 _b) {
        return Mathf.Rad2Deg * Mathf.Atan2(_a.y - _b.y, _a.x - _b.x);
    }

    public static string ValueFormat(float _value) {
        if (_value < 100f) {
            //Prepend zeroes in front of small numbers
            if (_value < 10f) {
                return "00" + _value.ToString();
            } else {
                return "0" + _value.ToString();
            }
        } else {
            return string.Format("{0:n0}", _value);
        }
    }
}

public static class ScreenSpace
{
    public static float Width = 3840f;
    public static float Height = 2160f;
    public static float Convert(float variable) {
        return variable * (Screen.height / Height);
    }

    public static float Inverse(float variable) {
        return variable * (Height / Screen.height);
    }
}

[Serializable]
public class ConstrainedFloatPref
{
    public string key;
    public float defaultValue;
    public float minValue;
    public float maxValue;
    [HideInInspector] public float value;

    public float Read() {
        //Load in values or their defaults, clamp each value to prevent external tampering from causing problems
        value = Mathf.Clamp(PlayerPrefs.GetFloat(key, defaultValue), minValue, maxValue);
        return value;
    }

    public void Write(float _value) {
        //Save a value, but Clamp it before even writing to the file
        value = Mathf.Clamp(_value, minValue, maxValue);
        PlayerPrefs.SetFloat(key, value);
    }
}

[Serializable]
public class ConstrainedIntPref
{
    public string key;
    public int defaultValue;
    public int minValue;
    public int maxValue;
    [HideInInspector] public int value;

    public int Read() {
        //Load in values or their defaults, clamp each value to prevent external tampering from causing problems
        value = Mathf.Clamp(PlayerPrefs.GetInt(key, defaultValue), minValue, maxValue);
        return value;
    }

    public void Write(int _value) {
        //Save a value, but Clamp it before even writing to the file
        value = Mathf.Clamp(_value, minValue, maxValue);
        PlayerPrefs.SetInt(key, value);
    }
}