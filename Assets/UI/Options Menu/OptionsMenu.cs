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
    [Header("Controls Tab")]
	public GameObject controlOptionsTab; //Controls Tab must be Active when Scene starts for events to fire
	public OptionsTabHandler optionsTabHandler;
    public ListControllerDropDown cameraInversion;
    public ListControllerDropDown inputPreference;
    public Slider mouseSensitivity;

    //[Header("Controls Tab")]SetActiveIndex

	private bool stateOfOptionsTab = false;

	private void OnEnable() {
        optionsTabHandler.OnTabSelected += OptionsTabHandler_OnTabSelected;
        menuHub.OnMenuClose += MenuHub_OnMenuClose;
        clickToClose.OnClick += Close;
		closeButton.OnClick += Close;
	//Set Screen Mode drop-down
		screenMode.SetActiveIndex(Screen.fullScreen ? 1 : 0); //Fullscreen / Windowed
		screenMode.OnSelect += UI.SetWindowMode;
	//Screen Resolution
		switch (Screen.width) {
			case 1920: resolution.SetActiveIndex(0); break;
			case 1600: resolution.SetActiveIndex(1); break;
			case 1366: case 1360: resolution.SetActiveIndex(2); break;
			case 1280: resolution.SetActiveIndex(3); break;
			default: resolution.SetActiveIndex(4); break;
		}
		resolution.OnSelect += UI.SetResolution;
	//Quality
		quality.SetActiveIndex(UI.Instance.quality.value);
		quality.OnSelect += UI.SetQuality;
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
//Controls Tab
	//Camera Inversion
		cameraInversion.SetActiveIndex(UI.Instance.cameraInversion.value);
        cameraInversion.OnSelect += UI.SetCameraInversion;
	//mouseSensitivity
        mouseSensitivity.value = UI.Instance.mouseSensitivity.value;
        mouseSensitivity.onValueChanged.AddListener (delegate {mouseSensitivity_onValueChanged();});
	//inputPreference
		inputPreference.SetActiveIndex(UI.Instance.inputPreference.value);
		inputPreference.OnSelect += UI.SetInputPreference;
	//Hide the Options Tab
		controlOptionsTab.SetActive(stateOfOptionsTab);
	}

    private void OnDisable() {
        menuHub.OnMenuClose -= MenuHub_OnMenuClose;
        clickToClose.OnClick -= Close;
		closeButton.OnClick -= Close;
		screenMode.OnSelect -= UI.SetWindowMode;
		resolution.OnSelect -= UI.SetResolution;
		quality.OnSelect -= UI.SetQuality;
	//Misc Tab
		uiScale.OnSelect -= UI.SetUIScale;
        fontChoice.OnSelect -= UI.SetFontChoice;
		textSize.OnSelect -= UI.SetTextSize;
		printSpeed.OnSelect -= UI.SetPrintSpeed;
		lappyBG.OnSelect -= LappyBG_OnChoiceMade;
	//Controls Tab
		cameraInversion.OnSelect -= UI.SetCameraInversion;
        mouseSensitivity.onValueChanged.RemoveListener (delegate {mouseSensitivity_onValueChanged();});
		inputPreference.OnSelect -= UI.SetInputPreference;
	}

	private void MenuHub_OnMenuClose() {
        Close();
    }

    private void OptionsTabHandler_OnTabSelected(bool controlsTabActive) {
        stateOfOptionsTab = controlsTabActive;
    }

	private void Close() {
		gameObject.SetActive(false);
	}

//MISC TAB
    public void mouseSensitivity_onValueChanged() {
        UI.SetCameraSensitivity(mouseSensitivity.value);
    }

	private void LappyBG_OnChoiceMade(int choiceMade) {
		lappyMenu.SetBackground(choiceMade);
	}
}