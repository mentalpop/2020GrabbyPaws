using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMenuHandler : MonoBehaviour
{
    public NavButton buttonNewGame;
    public NavButton buttonLoad;
    public NavButton buttonOptions;
    public NavButton buttonQuit;
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
        buttonNewGame.OnSelect += OnClickNewGame;
        buttonLoad.OnSelect += OnClickLoad;
        buttonQuit.OnSelect += OnClickQuit;
        buttonOptions.OnSelect += ButtonOptions_OnSelect;
    }

    private void OnDisable() {
        buttonNewGame.OnSelect -= OnClickNewGame;
        buttonLoad.OnSelect -= OnClickLoad;
        buttonQuit.OnSelect -= OnClickQuit;
        buttonOptions.OnSelect -= ButtonOptions_OnSelect;
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

    private void OnClickNewGame(ButtonStateData _buttonStateData) {
        if (saveFileExists) {
            confirmationWindow = UI.RequestConfirmation(promptNewGame, null);
            confirmationWindow.OnChoiceMade += OnConfirm;
			awaitingConfirmation = true;
        } else {
            NewGame();
        }
    }

    private void OnClickLoad(ButtonStateData _buttonStateData) {
        LoadGame();
    }

    private void ButtonOptions_OnSelect(ButtonStateData _buttonStateData) {
        //Open Lappy Menu to Options Screen
        optionsMenu.gameObject.SetActive(true);
    }

    private void OnClickQuit(ButtonStateData _buttonStateData) {
        confirmationWindow = UI.RequestConfirmation(promptQuit, null);
        confirmationWindow.OnChoiceMade += OnConfirm;
		awaitingConfirmation = true;
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