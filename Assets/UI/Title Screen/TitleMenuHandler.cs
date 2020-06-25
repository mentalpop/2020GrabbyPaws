using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMenuHandler : MonoBehaviour
{
    public ButtonGeneric buttonNewGame;
    public ButtonGeneric buttonLoad;
    public ButtonGeneric buttonQuit;
    
	public ConfirmationPromptData promptNewGame;
	public ConfirmationPromptData promptQuit;
	private ConfirmationWindow confirmationWindow;
	private bool awaitingConfirmation = false;
    
    [Header("Debug")]
    public bool saveFileExists = true;

    private void OnEnable() {
        buttonNewGame.OnClick += OnClickNewGame;
        buttonLoad.OnClick += OnClickLoad;
        buttonQuit.OnClick += OnClickQuit;
        UI.LockUI(gameObject);
    }

    private void OnDisable() {
        buttonNewGame.OnClick -= OnClickNewGame;
        buttonLoad.OnClick -= OnClickLoad;
        buttonQuit.OnClick -= OnClickQuit;
        UI.UnlockUI();
    //
		if (awaitingConfirmation) {
			awaitingConfirmation = false;
			confirmationWindow.OnChoiceMade -= OnConfirm;
		}
    }

    private void OnClickNewGame() {
        if (saveFileExists) {
            confirmationWindow = UI.RequestConfirmation(promptNewGame, null);
            confirmationWindow.OnChoiceMade += OnConfirm;
			awaitingConfirmation = true;
        } else {
            NewGame();
        }
    }

    private void OnClickLoad() {
        LoadGame();
    }
    
    private void OnClickQuit() {
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
        SceneTransitionHandler.SceneGoto("UITestScene", SpawnPoints.UITestRoomB);
    }

    private void LoadGame() {
        UI.Instance.LoadGameData(0);
    //Go to UI Test Room
        SceneTransitionHandler.SceneGoto("UITestScene", SpawnPoints.UITestRoomA);
    }
}