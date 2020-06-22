using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFormatterGO : ButtonFormatter
{
    public GameObject goDefaultNeutral;
    public GameObject goDefaultHover;
    public GameObject goDefaultSelected;
    public GameObject goToggleNeutral;
    public GameObject goToggleHover;
    public GameObject goToggleSelected;
    public GameObject goUnavailable;
    //[HideInInspector] public bool permanentFocus = false;

    protected override void ButtonFormat(ButtonStateData _buttonStateData) {
		GameObject setThisActive;
        if (_buttonStateData.available) {
            if (!_buttonStateData.stateActive || (_buttonStateData.stateActive && allowSelectionWhileActive)) {
                if (_buttonStateData.inputPressed) {
                    setThisActive = _buttonStateData.stateActive ? goToggleSelected : goDefaultSelected; //Selected
                } else {
                    if (_buttonStateData.hasFocus) { // || permanentFocus
                        setThisActive = _buttonStateData.stateActive ? goToggleHover : goDefaultHover; //Focus
                    } else {
                        setThisActive = _buttonStateData.stateActive ? goToggleNeutral : goDefaultNeutral; //Neutral
                    }
                }
            } else {
                if (_buttonStateData.hasFocus) { // || permanentFocus
                    setThisActive = _buttonStateData.stateActive ? goToggleHover : goDefaultHover; //Focus
                } else {
                    setThisActive = _buttonStateData.stateActive ? goToggleNeutral : goDefaultNeutral; //Neutral
                }
            }
        } else {
            setThisActive = goUnavailable;
        }
    //Disable all GameObjects except the right one
        if (goDefaultNeutral != null) goDefaultNeutral.SetActive(false);
        if (goDefaultHover != null) goDefaultHover.SetActive(false);
        if (goDefaultSelected != null) goDefaultSelected.SetActive(false);
        if (goToggleNeutral != null) goToggleNeutral.SetActive(false);
        if (goToggleHover != null) goToggleHover.SetActive(false);
        if (goToggleSelected != null) goToggleSelected.SetActive(false);
        if (goUnavailable != null) goUnavailable.SetActive(false);
        if (setThisActive != null) setThisActive.SetActive(true);
	}
}