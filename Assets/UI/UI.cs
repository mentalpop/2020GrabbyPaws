using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Invector.vCamera;

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

[DefaultExecutionOrder(-100)]
public class UI : MonoBehaviour
{
    public static string saveFileName = "grabby.paws";

    public Sonos sonosAudio;
    public GameObject HUD;
    public LappyMenu lappy;
    public ConfirmationWindow confirmationWindow;
    //public FlagRepository flagRepository;
[Header("Readables")]
    public Readable book;
[Header("Currency")]
    public Currency currency;
    public CurrencyDisplay currencyDisplay;
[Header("Inventory")]
    public InventoryDisplay InventoryDisplay;
    public Inventory inventory;
    public CinemachineBrain cBrain;
    [HideInInspector] public vThirdPersonCamera thirdPersonCamera;
[Header("Options")]
    public float uiScale = 1f;
    [HideInInspector] public float mouseSensitivity;

    public static UI Instance { get; private set; }

    public delegate void FileIOEvent(int fileNum);
    public event FileIOEvent OnSave = delegate { };
    public event FileIOEvent OnLoad = delegate { };

    //private bool doShowCurrencyDisplay = false;
    private GameObject lockUI = null;

    private List<GameObject> mouseCursorUsers = new List<GameObject>();

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
        DontDestroyOnLoad(gameObject);
    }

    public void DisplayReadable(ReadableData rData) {
        if (rData.isBook) {
            book.gameObject.SetActive(true);
            SetMouseState(true, book.gameObject);
            book.Unpack(rData);
        } else {
            Debug.Log("Readable PC not yet implemented!");
        }
    }

    public void ShowLappyMenu(bool _override) { //TODO: Make private
    //Show / Hide the HUD
        bool menuIsActive = _override || !lappy.gameObject.activeSelf;//InHierarchy;
        if (menuIsActive) {
            SetMouseState(true, lappy.gameObject);
            lappy.gameObject.SetActive(true);
        } else {
            lappy.Close();
        }
    }

    private void ShowInventoryDisplay() {
    //Show / Hide the HUD
        bool menuIsActive = !InventoryDisplay.gameObject.activeSelf;
        if (menuIsActive) {
            InventoryDisplay.gameObject.SetActive(true);
            if (currencyDisplay.gameObject.activeSelf) {
                currencyDisplay.Open();
            } else {
                currencyDisplay.gameObject.SetActive(true);
            }
        } else {
            InventoryDisplay.Close();
            currencyDisplay.Close();
        }
        SetMouseState(menuIsActive, InventoryDisplay.gameObject);
    }

    public static void SetUIScale(float scale) {
        Instance.uiScale = scale;
        Instance.lappy.transform.localScale = new Vector2(scale, scale);
    }

    public static void SetMouseState(bool lockMouse, GameObject gameObject) {
        if (lockMouse) {
            Instance.mouseCursorUsers.Add(gameObject);
        } else {
            Instance.mouseCursorUsers.Remove(gameObject);
        }
        bool suppressCamera = false;
        //Debug.Log("Instance.mouseCursorUsers.Count: "+Instance.mouseCursorUsers.Count);
        if (Instance.mouseCursorUsers.Count > 0) {
            suppressCamera = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        } else {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        /*
        Debug.Log("Instance.thirdPersonCamera: "+Instance.thirdPersonCamera);
        Debug.Log("Instance.cBrain: "+Instance.cBrain);
        //*/
        /*
        if (Instance.thirdPersonCamera == null) {
    //Using Cinemachine Freelook?
            if (Instance.cBrain != null) {
                CinemachineFreeLook currentCamera = Instance.cBrain.ActiveVirtualCamera as CinemachineFreeLook;
                //Debug.Log("currentCamera: "+currentCamera);
                if (currentCamera != null) {
                    if (suppressCamera) {
                        currentCamera.m_XAxis.m_InputAxisName = "";
                        currentCamera.m_YAxis.m_InputAxisName = "";
                    } else {
                        currentCamera.m_XAxis.m_InputAxisName = "Mouse X";
                        currentCamera.m_YAxis.m_InputAxisName = "Mouse Y";
                    }
                }
            }
        } else {
    //Using thirdPersonCamera
            Instance.thirdPersonCamera.enabled = !suppressCamera;
        }
        //*/
    }

    public static ConfirmationWindow RequestConfirmation(ConfirmationPromptData _data) {
        Instance.confirmationWindow.gameObject.SetActive(true);
        Instance.confirmationWindow.Unpack(_data);
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
//Save / Load
    public void SaveGameData(int fileNum) {
        Debug.Log("Game Saved: "+Application.persistentDataPath);
        OnSave?.Invoke(fileNum);
    }

    public void LoadGameData(int fileNum) {
        Debug.Log("Game Loaded");
        OnLoad?.Invoke(fileNum);
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