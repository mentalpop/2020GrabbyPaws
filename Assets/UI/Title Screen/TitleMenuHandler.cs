using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMenuHandler : MonoBehaviour
{
    public MenuNode mNodeMenu;
    public ListController menuList;
    public OptionsMenu optionsMenu;

    public ConfirmationPromptData promptNewGame;
	public ConfirmationPromptData promptQuit;
	private ConfirmationWindow confirmationWindow;
    public Canvas tsCanvas;

    private bool awaitingConfirmation = false;

[Header("New Game Data")]
    public string startScene;
    public SpawnPoints initialSpawnPoint;
[Header("Debug")]
    public bool saveFileExists = true;

    private void OnEnable() {
        menuList.OnSelect += SelectStartMenuItem;
    }

    private void OnDisable() {
        menuList.OnSelect -= SelectStartMenuItem;
        if (awaitingConfirmation) {
			awaitingConfirmation = false;
			confirmationWindow.OnChoiceMade -= OnConfirm;
		}
    }

    private void Start() {
    //These are important for when the user returns to the title screen in order to reset references
        tsCanvas.worldCamera = UI.Instance.uiCamera; 
        optionsMenu.lappyMenu = UI.Instance.lappy;
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
                LoadGame();
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
        UI.Instance.LoadGameData(0);
    //Go to UI Test Room
        //SceneTransitionHandler.SceneGoto("UITestScene", SpawnPoints.UITestRoomA);
    }
}