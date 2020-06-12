using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public LappyMenu lappyMenu;
	public ButtonGeneric closeButton;
    public ClickToClose clickToClose;
    [Header("Video Tab")]
	public DropDownMenu screenMode;
	public DropDownMenu resolution;
	public DropDownMenu quality;
    [Header("Misc Tab")]
    public DropDownMenu uiScale;
    public DropDownMenu fontChoice;
    public DropDownMenu textSize;
    public DropDownMenu printSpeed;
    public DropDownMenu lappyBG;
    public Slider mouseSensitivity;

    //[Header("Controls Tab")]

	private void OnEnable() {
        clickToClose.OnClick += Close;
		closeButton.OnClick += Close;
	//Set Screen Mode drop-down
		screenMode.chosenIndex = Screen.fullScreen ? 1 : 0; //Fullscreen / Windowed
		screenMode.SetHeader(screenMode.chosenIndex);
		screenMode.OnChoiceMade += ScreenMode_OnChoiceMade;
	//Screen Resolution
		switch (Screen.width) {
			case 1920: resolution.chosenIndex = 0; break;
			case 1600: resolution.chosenIndex = 1; break;
			case 1366: case 1360: resolution.chosenIndex = 2; break;
			case 1280: resolution.chosenIndex = 3; break;
			default: resolution.chosenIndex = 4; break;
		}
		resolution.SetHeader(resolution.chosenIndex);
		resolution.OnChoiceMade += Resolution_OnChoiceMade;
	//Quality
		quality.chosenIndex = UI.Instance.quality.value;
		quality.SetHeader(quality.chosenIndex);
		quality.OnChoiceMade += Quality_OnChoiceMade;
//Misc Tab
    //UI Scale
		uiScale.chosenIndex = UI.Instance.uiScale.value;
		uiScale.SetHeader(uiScale.chosenIndex);
        uiScale.OnChoiceMade += UI.SetUIScale;
    //fontChoice
		fontChoice.chosenIndex = UI.Instance.fontChoice.value;
		fontChoice.SetHeader(fontChoice.chosenIndex);
        fontChoice.OnChoiceMade += UI.SetFontChoice;
    //textSize
		textSize.chosenIndex = UI.Instance.textSize.value;
		textSize.SetHeader(textSize.chosenIndex);
        textSize.OnChoiceMade += UI.SetTextSize;
    //printSpeed
		printSpeed.chosenIndex = UI.Instance.textPrintSpeed.value;
		printSpeed.SetHeader(printSpeed.chosenIndex);
        printSpeed.OnChoiceMade += UI.SetPrintSpeed;
	//Lappy BG
		lappyBG.chosenIndex = lappyMenu.chosenBGIndex;
		lappyBG.SetHeader(lappyBG.chosenIndex);
		lappyBG.OnChoiceMade += LappyBG_OnChoiceMade;
	//mouseSensitivity
        mouseSensitivity.value = UI.Instance.mouseSensitivity.value;
        mouseSensitivity.onValueChanged.AddListener (delegate {mouseSensitivity_onValueChanged();});
	}

    private void OnDisable() {
        clickToClose.OnClick -= Close;
		closeButton.OnClick -= Close;
		screenMode.OnChoiceMade -= ScreenMode_OnChoiceMade;
		resolution.OnChoiceMade -= Resolution_OnChoiceMade;
		quality.OnChoiceMade -= Quality_OnChoiceMade;
		uiScale.OnChoiceMade -= UI.SetUIScale;
        fontChoice.OnChoiceMade -= UI.SetFontChoice;
		textSize.OnChoiceMade -= UI.SetTextSize;
		printSpeed.OnChoiceMade -= UI.SetPrintSpeed;
		lappyBG.OnChoiceMade -= LappyBG_OnChoiceMade;
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

	private void Close() {
		gameObject.SetActive(false);
	}
}