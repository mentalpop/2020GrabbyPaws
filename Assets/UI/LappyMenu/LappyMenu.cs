﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LappyMenu : MonoBehaviour
{
    public MenuHub menuHub;
    public AnimatedUIContainer container;
    public SneakDiary sneakDiary;
    public NotSecrets notSecrets;
    public OptionsMenu optionsMenu;
    public HellaHockster hellaHuckster;
    public WishListWindow wishList;
    public RivenChatWindow rivenChatWindow;
    public string defaultConversation;
    public Image lappyBG;
    public ClickToClose clickToClose;
    public ConstrainedIntPref lappySelectedBG;
    public List<Sprite> lappyBGs = new List<Sprite>();
    [HideInInspector] public int chosenBGIndex = 0;

    public ListController startMenuList;
    //public List<TabData> tabs = new List<TabData>();
    public ButtonGeneric startButton;

[Header("Confirmation Prompts")]
    public ConfirmationPromptData promptSave;
    public ConfirmationPromptData promptQuitTitle;
    public ConfirmationPromptData promptQuitGame;
    public MenuNode mNodeStartMenu;
    //public MenuNode mNodeSave;
    //public MenuNode mNodeQuitTitle;
    //public MenuNode mNodeQuitGame;
    private ConfirmationWindow confirmationWindow;
    private bool awaitingConfirmation = false;

    private void OnClickStart() {
        startMenuList.gameObject.SetActive(!startMenuList.gameObject.activeInHierarchy);
    }

    private void OnEnable() {
        startMenuList.OnSelect += SelectStartMenuItem;
        startButton.OnClick += OnClickStart;
        clickToClose.OnClick += clickToClose_OnClick;
        container.OnEffectComplete += Container_OnEffectComplete;
        menuHub.OnMenuClose += MenuNavigator_OnClose;
        SetBackground(lappySelectedBG.Read());
    }

    private void OnDisable() {
        startMenuList.OnSelect -= SelectStartMenuItem;
        clickToClose.OnClick -= clickToClose_OnClick;
        startButton.OnClick -= OnClickStart;
        container.OnEffectComplete -= Container_OnEffectComplete;
        menuHub.OnMenuClose -= MenuNavigator_OnClose;
        if (awaitingConfirmation) {
            awaitingConfirmation = false;
            confirmationWindow.OnChoiceMade -= OnConfirm;
        }
        UI.Instance.RefocusInventory(); //If the Inventory was open, refocus on it
    }

    private void Container_OnEffectComplete(bool reverse) {
        if (reverse) {
            UI.SetControlState(false, gameObject); //De-register from UI
            gameObject.SetActive(false); //For now, just close instantly
        }
    }

    private void clickToClose_OnClick() {
        Close();
    }

    private void MenuNavigator_OnClose() {
        UI.Instance.LappyMenuToggle(false);
        //Close();
    }

    public bool Close() {
        if (!rivenChatWindow.gameObject.activeSelf && !container.gTween.doReverse && !awaitingConfirmation) {
            container.gTween.Reverse();
            return true;
        }
        return false;
    }

    private void OnConfirm(bool _choice) {
        awaitingConfirmation = false;
        confirmationWindow.OnChoiceMade -= OnConfirm;
        if (_choice) {
            if (confirmationWindow.promptData.promptID == ConfirmationPromptID.QuitToTitle) {
                //Quit to Title
                Close();
                SceneTransitionHandler.SceneGoto("Title_Screen", SpawnPoints.UITestRoomA);
            }
            if (confirmationWindow.promptData.promptID == ConfirmationPromptID.QuitGame) {
                Application.Quit();
            }
            if (confirmationWindow.promptData.promptID == ConfirmationPromptID.Save) {
                UI.Instance.SaveGameData(UI.GetCurrentFile());
            }
        } else {
            Debug.Log("User selected NOPE");
        }
    }

    /*
    void Start() {
        startTabsSortMenu.InstantiateTabs(tabs);
    }
    //*/

    public void SelectStartMenuItem(int _activeTab) {
        switch (_activeTab) {
            case 0: //Rewind Time
                hellaHuckster.gameObject.SetActive(true);
                break;
            case 1: //Sneak Diary
                sneakDiary.gameObject.SetActive(true);
                break;
            case 2: //Not Secrets
                notSecrets.gameObject.SetActive(true);
                break;
            case 3: //Wish List
                wishList.gameObject.SetActive(true);
                break;
            case 4: //Chat
                StartConversation(defaultConversation);
                break;
            case 5: //Options
                optionsMenu.gameObject.SetActive(true);
                break;
            case 6: //Save Game
                confirmationWindow = UI.RequestConfirmation(promptSave, mNodeStartMenu); //mNodeSave
                confirmationWindow.OnChoiceMade += OnConfirm;
                awaitingConfirmation = true;
                break;
            case 7: //Quit to Title
                confirmationWindow = UI.RequestConfirmation(promptQuitTitle, mNodeStartMenu); //mNodeQuitTitle
                confirmationWindow.OnChoiceMade += OnConfirm;
                awaitingConfirmation = true;
                break;
            case 8: //Quit Game
                confirmationWindow = UI.RequestConfirmation(promptQuitGame, mNodeStartMenu); //mNodeQuitGame
                confirmationWindow.OnChoiceMade += OnConfirm;
                awaitingConfirmation = true;
                break;
        }
    }

    public void StartConversation(string conversationID) {
        rivenChatWindow.gameObject.SetActive(true);
        rivenChatWindow.TriggerConversation(conversationID);
    }

    public void SetBackground(int _bgIndex) {
        lappySelectedBG.Write(_bgIndex);
        chosenBGIndex = _bgIndex;
        lappyBG.sprite = lappyBGs[_bgIndex];
    }
}