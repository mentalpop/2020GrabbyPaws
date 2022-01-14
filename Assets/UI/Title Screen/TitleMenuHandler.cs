using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleMenuHandler : MonoBehaviour
{
    public Canvas tsCanvas;
    public MenuNode mNodeMenu;
    public ListController menuList;
    public OptionsMenu optionsMenu;
    public Image artLayer;
    public List<Sprite> artSprites = new List<Sprite>();
[Header("ConfirmationPromptData")]
    public ConfirmationPromptData promptNewGame;
	public ConfirmationPromptData promptQuit;
[Header("New Game Data")]
    public string startScene;
    public SpawnPoints initialSpawnPoint;

    private ConfirmationWindow confirmationWindow;
    private bool awaitingConfirmation = false;
    private bool saveFileExists = false;

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
        //If Save file does not exist, disable Continue button
        if (!saveFileExists) {
            var _button = menuList.Elements[1]; //Magic number to "get" the Continue Button
            _button.navButton.SetAvailable(false);
        }
    }

    private void Instance_OnUIScaled(float scale) {
        optionsMenu.transform.localScale = new Vector2(scale, scale);
    }

    public void SelectStartMenuItem(int _activeTab) {
        switch (_activeTab) {
            case 0: //New Game
                if (saveFileExists) {
                    confirmationWindow = UI.RequestConfirmation(promptNewGame, mNodeMenu);
                    confirmationWindow.OnChoiceMade += OnConfirm;
                    awaitingConfirmation = true;
                } else {
                    NewGame();
                }
                break;
            case 1: //Continue
                if (saveFileExists) {
                    LoadGame();
                }
                break;
            case 2: //Options
                optionsMenu.gameObject.SetActive(true);
                break;
            case 3: //Quit Game
                confirmationWindow = UI.RequestConfirmation(promptQuit, mNodeMenu);
                confirmationWindow.OnChoiceMade += OnConfirm;
                awaitingConfirmation = true;
                break;
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