using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleMenuHandler : MonoBehaviour
{
    public Canvas tsCanvas;
    public MenuNode mNodeMenu;
    public OptionsMenu optionsMenu;
    public Image artLayer;
    public List<Sprite> artSprites = new List<Sprite>();
[Header("ConfirmationPromptData")]
    public ConfirmationPromptData promptNewGame;
    public ConfirmationPromptData promptEraseData;
    public ConfirmationPromptData promptQuit;
[Header("New Game Data")]
    public string startScene;
    public SpawnPoints initialSpawnPoint;
    public bool loadDebugScene = false;
    public SpawnPoints debugSpawnPoint;
    public string debugScene;
[Header("Menu Handling")]
    public ListController menuList;
    public ListUnpacker listUnpacker;
    public List<NavButtonData> dataNoSave = new List<NavButtonData>();
    public List<NavButtonData> dataHasSaveFile = new List<NavButtonData>();

    private ConfirmationWindow confirmationWindow;
    private bool awaitingConfirmation = false;
    private bool saveFileExists = false;
    private int indexOfNewGameButton;

    private void OnEnable() {
        menuList.OnSelect += SelectStartMenuItem;
        UI.Instance.OnUIScaled += Instance_OnUIScaled;
    }

    private void OnDisable() {
        menuList.OnSelect -= SelectStartMenuItem;
        UI.Instance.OnUIScaled -= Instance_OnUIScaled;
        if (awaitingConfirmation) {
			awaitingConfirmation = false;
			confirmationWindow.OnChoiceMade -= OnConfirm;
		}
    }

    private void Start() {
        //Set UI Scale of Options Window
        Instance_OnUIScaled(UI.GetUIScale()); //Load default value
                                              //These are important for when the user returns to the title screen in order to reset references
        tsCanvas.worldCamera = UI.Instance.uiCamera;
        optionsMenu.lappyMenu = UI.Instance.lappy;

        //Randomize Art Layer
        artLayer.sprite = artSprites[Random.Range(0, artSprites.Count)];
        CheckSaveFileExistsCreateMenuButtons();
    }

    private void CheckSaveFileExistsCreateMenuButtons() {
        saveFileExists = UI.Instance.GameDataExists(UI.GetCurrentFile());
        //Depending on whether the save file exists, instantiate a different list
        if (saveFileExists) {
            listUnpacker.Unpack(dataHasSaveFile);
        } else {
            listUnpacker.Unpack(dataNoSave);
        }
    }

    private void Instance_OnUIScaled(float scale) {
        optionsMenu.transform.localScale = new Vector2(scale, scale);
    }

    public void SelectStartMenuItem(int _activeTab) {
        if (saveFileExists) {
            switch (_activeTab) {
                case 0: OptionContinue(); break;
                case 1: OptionNewGame(); break;
                case 2: OptionOptions(); break;
                case 3: OptionEraseSaveData(); break;
                case 4: OptionQuitGame(); break;
            }
        } else {
            switch (_activeTab) {
                case 0: OptionNewGame(); break;
                case 1: OptionOptions(); break;
                case 2: OptionQuitGame(); break;
            }
        }
    }

    private void OptionQuitGame() {
        confirmationWindow = UI.RequestConfirmation(promptQuit, mNodeMenu);
        confirmationWindow.OnChoiceMade += OnConfirm;
        awaitingConfirmation = true;
    }

    private void OptionOptions() {
        optionsMenu.gameObject.SetActive(true);
    }

    private void OptionContinue() {
        if (saveFileExists) {
            LoadGame();
        }
    }

    private void OptionNewGame() {
        if (saveFileExists) {
            confirmationWindow = UI.RequestConfirmation(promptNewGame, mNodeMenu);
            confirmationWindow.OnChoiceMade += OnConfirm;
            awaitingConfirmation = true;
        } else {
            NewGame();
        }
    }

    private void OnConfirm(bool _choice) {
		awaitingConfirmation = false;
		confirmationWindow.OnChoiceMade -= OnConfirm;
		if (_choice) {
            switch (confirmationWindow.promptData.promptID) {
                case ConfirmationPromptID.QuitGame:
                    Application.Quit();
                    Debug.Log("Application.Quit");
                    break;
                case ConfirmationPromptID.NewGame:
                    NewGame();
                    break;
                case ConfirmationPromptID.EraseSaveData:
                    UI.Instance.EraseData(UI.GetCurrentFile());
                    CheckSaveFileExistsCreateMenuButtons();
                    break;
            }


        } else {
			Debug.Log("User selected NOPE");
		}
	}

    private void NewGame() {
        UI.Instance.NewGame(UI.GetCurrentFile());
        if (loadDebugScene) {
            SceneTransitionHandler.SceneGoto(debugScene, debugSpawnPoint);
        } else {
            SceneTransitionHandler.SceneGoto(startScene, initialSpawnPoint);
        }
    }

    private void OptionEraseSaveData() {
        confirmationWindow = UI.RequestConfirmation(promptEraseData, mNodeMenu);
        confirmationWindow.OnChoiceMade += OnConfirm;
        awaitingConfirmation = true;
    }

    private void LoadGame() {
        UI.Instance.LoadGameData(UI.GetCurrentFile());
    //Go to UI Test Room
        //SceneTransitionHandler.SceneGoto("UITestScene", SpawnPoints.UITestRoomA);
    }
}