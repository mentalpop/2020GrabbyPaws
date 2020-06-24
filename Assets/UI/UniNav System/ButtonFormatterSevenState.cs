using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFormatterSevenState : ButtonStateFormatter
{
    public BState buttonState;
    /*
    public GameObject Neutral;
    public GameObject Focus;
    public GameObject Selected;
    public GameObject Active;
    public GameObject ActiveFocus;
    public GameObject ActiveSelected;
    public GameObject Unavailable;
    //*/

    public enum BState
    {
        Neutral,
        Focus,
        Selected,
        Active,
        ActiveFocus,
        ActiveSelected,
        Unavailable
    }

    protected override void ButtonFormat(ButtonStateData _buttonStateData) {
        if (_buttonStateData.available) {
            if (!_buttonStateData.stateActive || (_buttonStateData.stateActive && allowSelectionWhileActive)) {
                if (_buttonStateData.inputPressed) {
                    buttonState = _buttonStateData.stateActive ? BState.ActiveSelected : BState.Selected; //Selected
                } else {
                    if (_buttonStateData.hasFocus) { // || permanentFocus
                        buttonState = _buttonStateData.stateActive ? BState.ActiveFocus : BState.Focus; //Focus
                    } else {
                        buttonState = _buttonStateData.stateActive ? BState.Active : BState.Neutral; //Neutral
                    }
                }
            } else {
                if (_buttonStateData.hasFocus) { // || permanentFocus
                    buttonState = _buttonStateData.stateActive ? BState.ActiveFocus : BState.Focus; //Focus
                } else {
                    buttonState = _buttonStateData.stateActive ? BState.Active : BState.Neutral; //Neutral
                }
            }
        } else {
            buttonState = BState.Unavailable;
        }
        //Debug.Log("buttonState: "+buttonState);
        /*
        base.ButtonFormat(_buttonStateData);
        switch (buttonState) {
            case BState.Neutral: ; break;
            case BState.Focus: ; break;
            case BState.Selected: ; break;
            case BState.Active: ; break;
            case BState.ActiveFocus: ; break;
            case BState.ActiveSelected: ; break;
            case BState.Unavailable: ; break;
        }
        //*/
    }
}