using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public LappyMenu lappyMenu;
	public MenuHub menuHub;
	public ButtonGeneric closeButton;
    public ClickToClose clickToClose;
    [Header("Video Tab")]
	public ListControllerDropDown screenMode;
	public ListControllerDropDown resolution;
	public ListControllerDropDown quality;
    [Header("Misc Tab")]
    public ListControllerDropDown uiScale;
    public ListControllerDropDown fontChoice;
    public ListControllerDropDown textSize;
    public ListControllerDropDown printSpeed;
    public ListControllerDropDown lappyBG;
    public Slider mouseSensitivity;

    //[Header("Controls Tab")]SetActiveIndex

	private void OnEnable() {
        menuHub.OnMenuClose += MenuHub_OnMenuClose;
        clickToClose.OnClick += Close;
		closeButton.OnClick += Close;
	//Set Screen Mode drop-down
		screenMode.SetActiveIndex(Screen.fullScreen ? 1 : 0); //Fullscreen / Windowed
		screenMode.OnSelect += ScreenMode_OnChoiceMade;
	//Screen Resolution
		switch (Screen.width) {
			case 1920: resolution.SetActiveIndex(0); break;
			case 1600: resolution.SetActiveIndex(1); break;
			case 1366: case 1360: resolution.SetActiveIndex(2); break;
			case 1280: resolution.SetActiveIndex(3); break;
			default: resolution.SetActiveIndex(4); break;
		}
		resolution.OnSelect += Resolution_OnChoiceMade;
	//Quality
		quality.SetActiveIndex(UI.Instance.quality.value);
		quality.OnSelect += Quality_OnChoiceMade;
//Misc Tab
    //UI Scale
		uiScale.SetActiveIndex(UI.Instance.uiScale.value);
        uiScale.OnSelect += UI.SetUIScale;
    //fontChoice
		fontChoice.SetActiveIndex(UI.Instance.fontChoice.value);
        fontChoice.OnSelect += UI.SetFontChoice;
    //textSize
		textSize.SetActiveIndex(UI.Instance.textSize.value);
        textSize.OnSelect += UI.SetTextSize;
    //printSpeed
		printSpeed.SetActiveIndex(UI.Instance.textPrintSpeed.value);
        printSpeed.OnSelect += UI.SetPrintSpeed;
	//Lappy BG
		lappyBG.SetActiveIndex(lappyMenu.chosenBGIndex);
		lappyBG.OnSelect += LappyBG_OnChoiceMade;
	//mouseSensitivity
        mouseSensitivity.value = UI.Instance.mouseSensitivity.value;
        mouseSensitivity.onValueChanged.AddListener (delegate {mouseSensitivity_onValueChanged();});
	}

    private void OnDisable() {
        menuHub.OnMenuClose -= MenuHub_OnMenuClose;
        clickToClose.OnClick -= Close;
		closeButton.OnClick -= Close;
		screenMode.OnSelect -= ScreenMode_OnChoiceMade;
		resolution.OnSelect -= Resolution_OnChoiceMade;
		quality.OnSelect -= Quality_OnChoiceMade;
		uiScale.OnSelect -= UI.SetUIScale;
        fontChoice.OnSelect -= UI.SetFontChoice;
		textSize.OnSelect -= UI.SetTextSize;
		printSpeed.OnSelect -= UI.SetPrintSpeed;
		lappyBG.OnSelect -= LappyBG_OnChoiceMade;
        mouseSensitivity.onValueChanged.RemoveListener (delegate {mouseSensitivity_onValueChanged();});
	}

//MISC TAB
    public void mouseSensitivity_onValueChanged() {
        UI.SetMouseSensitivity(mouseSensitivity.value);
    }

	private void LappyBG_OnChoiceMade(int choiceMade) {
		lappyMenu.SetBackground(choiceMade);
	}
//VIDEO Tab
	private void Quality_OnChoiceMade(int choiceMade) {
		UI.SetQuality(choiceMade);
	}

	private void Resolution_OnChoiceMade(int choiceMade) {
		UI.SetResolution(choiceMade);
		/*
		switch (choiceMade) {
			case 0: Screen.SetResolution(1920, 1080, Screen.fullScreen); break;
			case 1: Screen.SetResolution(1600, 900, Screen.fullScreen); break;
			case 2: Screen.SetResolution(1366, 768, Screen.fullScreen); break;
			case 3: Screen.SetResolution(1280, 720, Screen.fullScreen); break;
			case 4: Screen.SetResolution(1176, 664, Screen.fullScreen); break;
		}
		//*/
	}

	private void ScreenMode_OnChoiceMade(int choiceMade) {
		UI.SetWindowMode(choiceMade);
	}

    private void MenuHub_OnMenuClose() {
        Close();
    }

	private void Close() {
		gameObject.SetActive(false);
	}
}