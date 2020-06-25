using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

public enum QuestNames {
    q001TwilightCottonCandy,
    q001TwilightCottonCandyEndFlag
    /*,
    q002TwilightCCandy,
    q010TwilightCCandy,
    q004Twilight,
    //*/
}

public enum Secrets {
    s001Test,
    s002Test
}

public enum NPCLocations {
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
    //public FlagRepository flagRepository;
[Header("Readables")]
    public Readable book;
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
    public ConstrainedIntPref uiScale;
    public ConstrainedIntPref textSize;
    public ConstrainedIntPref fontChoice;
    public ConstrainedIntPref textPrintSpeed;

    [HideInInspector] public PlayerBehaviour player;

    public delegate void FileIOEvent(int fileNum);
    public event FileIOEvent OnSave = delegate { };
    public event FileIOEvent OnLoad = delegate { };

    public delegate void TextScaledEvent(float fontScale);
    public event TextScaledEvent OnTextScaled = delegate { };

    public delegate void FontSelectEvent(int fontChoice);
    public event FontSelectEvent OnFontChoice = delegate { };

    public delegate void PrintSpeedEvent(float speed);
    public event PrintSpeedEvent OnPrintSpeedSet = delegate { };

    //private bool doShowCurrencyDisplay = false;
    private GameObject lockUI = null;
    private float cameraVelocityX;
    private float cameraVelocityY;

    private List<GameObject> mouseCursorUsers = new List<GameObject>();

    public static UI Instance { get; private set; }
    #region Unity Messages
    private void OnEnable() {
        currency.OnCashChanged += OnCurrencyChanged;
    }

    private void OnDisable() {
        currency.OnCashChanged -= OnCurrencyChanged;
    }

    private void Awake() {
    //Singleton Pattern
        if (Instance != null && Instance != this) { 
            Destroy(gameObject);
            return;
        }
        Instance = this;
        LoadOptionsData();
        DontDestroyOnLoad(gameObject);
    }

    void Update() {
        if (lockUI == null) { //No GameObject is currently locking the UI
    //Open / Close menus
            if (Input.GetKeyDown(KeyCode.Tab)) { //"Inventory"
                ShowInventoryDisplay();
            }
            if (Input.GetKeyDown(KeyCode.Escape)) { //"Kwit"
                ShowLappyMenu(false);
            }
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
        Debug.Log("quality: "+choiceMade);
    }

//MISC OPTIONS
    public static void SetMouseSensitivity(float _val) {
        Instance.mouseSensitivity.Write(_val);
    //Set Mouse Sensitivity
        //TODO
    }
    public static void SetUIScale(int choiceMade) {
        Instance.uiScale.Write(choiceMade); //Set and Save
        float _uiScale = GetUIScale();
        Instance.lappy.transform.localScale = new Vector2(_uiScale, _uiScale);
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
    #endregion
    
    #region UI State
    public void DisplayBook(ReadableData rData) {
        book.gameObject.SetActive(true);
        SetControlState(true, book.gameObject);
        book.Unpack(rData);
    }

    public void DisplayPC(ReadablePCData pcData) {
        pc.gameObject.SetActive(true);
        SetControlState(true, pc.gameObject);
        pc.Unpack(pcData);
    }

    public void ShowLappyMenu(bool _override) { //TODO: Make private
    //Show / Hide the HUD
        bool menuIsActive = _override || !lappy.gameObject.activeSelf;//InHierarchy;
        if (menuIsActive) {
            SetControlState(true, lappy.gameObject);
            lappy.gameObject.SetActive(true);
        } else {
            lappy.Close();
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

    public static void SetControlState(bool lockMouse, GameObject gameObject) {
        if (lockMouse) {
            Instance.mouseCursorUsers.Add(gameObject);
        } else {
            Instance.mouseCursorUsers.Remove(gameObject);
        }
        bool lockControls = false;
        //Debug.Log("Instance.mouseCursorUsers.Count: "+Instance.mouseCursorUsers.Count);
        if (Instance.mouseCursorUsers.Count > 0) {
            lockControls = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        } else {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    //Player
        if (Instance.player != null) {
            Instance.player.SetLockState(lockControls);
        }
        /*
        Debug.Log("Instance.thirdPersonCamera: "+Instance.thirdPersonCamera);
        Debug.Log("Instance.cBrain: "+Instance.cBrain);
        //*/
        //*
        //if (Instance.thirdPersonCamera == null) {
//Camera
    //Using Cinemachine Freelook?
        if (Instance.cFreeLook != null) {
            //Debug.Log("suppressCamera: "+suppressCamera);
            //CinemachineFreeLook currentCamera = Instance.cFreeLook;//Instance.cFreeLook.ActiveVirtualCamera as CinemachineFreeLook;
            //Debug.Log("currentCamera: "+currentCamera);
            if (lockControls) {
            //X Axis
                Instance.cFreeLook.m_XAxis.m_InputAxisName = "";
                Instance.cameraVelocityX = Instance.cFreeLook.m_XAxis.m_MaxSpeed;
                Instance.cFreeLook.m_XAxis.m_MaxSpeed = 0;
            //Y Axis
                Instance.cFreeLook.m_YAxis.m_InputAxisName = "";
                Instance.cameraVelocityY = Instance.cFreeLook.m_YAxis.m_MaxSpeed;
                Instance.cFreeLook.m_YAxis.m_MaxSpeed = 0;
            } else {
                Instance.cFreeLook.m_XAxis.m_InputAxisName = "Mouse X";
                Instance.cFreeLook.m_XAxis.m_MaxSpeed = Instance.cameraVelocityX;
                Instance.cFreeLook.m_YAxis.m_InputAxisName = "Mouse Y";
                Instance.cFreeLook.m_YAxis.m_MaxSpeed = Instance.cameraVelocityY;
            }
        }
        /*
        } else {
    //Using thirdPersonCamera
            Instance.thirdPersonCamera.enabled = !suppressCamera;
        }
        //*/
        //*/
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
        SetMouseSensitivity(mouseSensitivity.Read());
        SetUIScale(uiScale.Read());
        SetTextSize(textSize.Read());
        SetFontChoice(fontChoice.Read());
        SetPrintSpeed(textPrintSpeed.Read());
    }

    public void SaveGameData(int fileNum) {
        Debug.Log("Game Saved: "+Application.persistentDataPath);
        OnSave?.Invoke(fileNum);
    }

    public void LoadGameData(int fileNum) {
        Debug.Log("Game Loaded");
        OnLoad?.Invoke(fileNum);
    }
    #endregion
    
    public static float Direction(Vector2 _a, Vector2 _b) {
       return Mathf.Rad2Deg*Mathf.Atan2(_a.y - _b.y, _a.x - _b.x);
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