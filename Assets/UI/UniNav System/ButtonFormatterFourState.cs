using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFormatterFourState : ButtonStateFormatter
{
    public BState buttonState;
    public enum BState
    {
        Neutral,
        Focus,
        Selected,
        Unavailable
    }

    protected override void ButtonFormat(ButtonStateData _buttonStateData) {
        if (_buttonStateData.available) {
            if (!_buttonStateData.stateActive || (_buttonStateData.stateActive && allowSelectionWhileActive)) {
                if (_buttonStateData.inputPressed) {
                    buttonState = BState.Selected; //Selected
                } else {
                    if (_buttonStateData.hasFocus) { // || permanentFocus
                        buttonState = BState.Focus; //Focus
                    } else {
                        buttonState = BState.Neutral; //Neutral
                    }
                }
            } else {
                if (_buttonStateData.hasFocus) { // || permanentFocus
                    buttonState = BState.Focus; //Focus
                } else {
                    buttonState = BState.Neutral; //Neutral
                }
            }
        } else {
            buttonState = BState.Unavailable;
        }
        /*
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