using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LappyMenu : MonoBehaviour
{
    public AnimatedUIContainer container;
    public SneakDiary sneakDiary;
    public NotSecrets notSecrets;
    public OptionsMenu optionsMenu;
    public HellaHockster hellaHuckster;
    public WishListWindow wishList;
    public GameObject rivenChatWindow;
    public Image lappyBG;
    public ClickToClose clickToClose;
    public ConstrainedIntPref lappySelectedBG;
    public List<Sprite> lappyBGs = new List<Sprite>();
    [HideInInspector] public int chosenBGIndex = 0;
    
    public ListController startMenuList;
    public MenuNavigator menuNavigator;
    //public List<TabData> tabs = new List<TabData>();
    public ButtonGeneric startButton;

	public ConfirmationPromptData promptSave;
	public ConfirmationPromptData promptQuitTitle;
	public ConfirmationPromptData promptQuitGame;
	private ConfirmationWindow confirmationWindow;
	private bool awaitingConfirmation = false;

	private void OnClickStart() {
		startMenuList.gameObject.SetActive(!startMenuList.gameObject.activeInHierarchy);
	}

    private void OnEnable() {
        startMenuList.OnSelect += SelectStartMenuItem;
        clickToClose.OnClick += Close;
        startButton.OnClick += OnClickStart;
        container.OnEffectComplete += Container_OnEffectComplete;
        menuNavigator.OnClose += MenuNavigator_OnClose;
        SetBackground(lappySelectedBG.Read());
    }

    private void OnDisable() {
        startMenuList.OnSelect -= SelectStartMenuItem;
        clickToClose.OnClick -= Close;
		startButton.OnClick -= OnClickStart;
        container.OnEffectComplete -= Container_OnEffectComplete;
        menuNavigator.OnClose -= MenuNavigator_OnClose;
		if (awaitingConfirmation) {
			awaitingConfirmation = false;
			confirmationWindow.OnChoiceMade -= OnConfirm;
		}
    }

    private void Container_OnEffectComplete(bool reverse) {
        if (reverse) {
            UI.SetControlState(false, gameObject); //De-register from UI
            gameObject.SetActive(false); //For now, just close instantly
        } else {

        }
    }
    
    private void MenuNavigator_OnClose(MenuNode menuNode) {
        Close();
    }

    public void Close() {
        if (!container.gTween.doReverse)
            container.gTween.Reverse();
    }
    
	private void OnConfirm(bool _choice) {
		awaitingConfirmation = false;
		confirmationWindow.OnChoiceMade -= OnConfirm;
		if (_choice) {
            if (confirmationWindow.promptData.promptID == ConfirmationPromptID.QuitToTitle) {
        //Quit to Title
                Close();
                SceneManager.LoadScene("TitleScreen");
            }
            if (confirmationWindow.promptData.promptID == ConfirmationPromptID.QuitGame) {
                Application.Quit();
            }
            if (confirmationWindow.promptData.promptID == ConfirmationPromptID.Save) {
                UI.Instance.SaveGameData(0);
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
        switch(_activeTab) {
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
                rivenChatWindow.SetActive(true);
                break;
            case 5: //Options
                optionsMenu.gameObject.SetActive(true);
                break;
            case 6: //Save Game
                confirmationWindow = UI.RequestConfirmation(promptSave);
                confirmationWindow.OnChoiceMade += OnConfirm;
			    awaitingConfirmation = true;
                break;
            case 7: //Quit to Title
                confirmationWindow = UI.RequestConfirmation(promptQuitTitle);
                confirmationWindow.OnChoiceMade += OnConfirm;
			    awaitingConfirmation = true;
                break;
            case 8: //Quit Game
                confirmationWindow = UI.RequestConfirmation(promptQuitGame);
                confirmationWindow.OnChoiceMade += OnConfirm;
			    awaitingConfirmation = true;
                break;
        }
    }

    public void SetBackground(int _bgIndex) {
        lappySelectedBG.Write(_bgIndex);
        chosenBGIndex = _bgIndex;
        lappyBG.sprite = lappyBGs[_bgIndex];
    }
}