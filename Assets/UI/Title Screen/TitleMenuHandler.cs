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
	public ConfirmationPromptData promptQuit;
[Header("New Game Data")]
    public string startScene;
    public SpawnPoints initialSpawnPoint;
[Header("Menu Handling")]
    public ListController menuList;
    public ListUnpacker listUnpacker;
    public List<NavButtonData> buttonData = new List<NavButtonData>();

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

        saveFileExists = UI.Instance.GameDataExists(UI.GetCurrentFile());
        //If Save file does not exist, remove Continue button from the list
        //if (!saveFileExists) {
        //    var _button = menuList.Elements[1]; //Magic number to "get" the Continue Button
        //    _button.navButton.SetAvailable(false);
        //}
        if (saveFileExists) {
            listUnpacker.Unpack(buttonData);
        } else {
            List<NavButtonData> _buttonData = new List<NavButtonData>();
            for (int i = 1; i < buttonData.Count; i++) { //Build a new list, ignore the first element (Continue Button)
                _buttonData.Add(buttonData[i]);
            }
            listUnpacker.Unpack(_buttonData);
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
                case 3: OptionQuitGame(); break;
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
            if (confirmationWindow.promptData.promptID == ConfirmationPromptID.NewGame) {
                NewGame();
            }
            if (confirmationWindow.promptData.promptID == ConfirmationPromptID.QuitGame) {
                Application.Quit();
                Debug.Log("Application.Quit");
            }
		} else {
			Debug.Log("User selected NOPE");
		}
	}

    private void NewGame() {
        SceneTransitionHandler.SceneGoto(startScene, initialSpawnPoint);
    }

    private void LoadGame() {
        UI.Instance.LoadGameData(UI.GetCurrentFile());
    //Go to UI Test Room
        //SceneTransitionHandler.SceneGoto("UITestScene", SpawnPoints.UITestRoomA);
    }
}