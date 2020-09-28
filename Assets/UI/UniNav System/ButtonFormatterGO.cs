using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFormatterGO : ButtonFormatterEightState
{
    public GameObject goNeutral;
    public GameObject goFocus;
    public GameObject goSelected;
    public GameObject goActive;
    public GameObject goActiveFocus;
    public GameObject goActiveSelected;
    public GameObject goUnavailable;
    public GameObject goUnavailableFocus;
    //[HideInInspector] public bool permanentFocus = false;

    protected override void ButtonFormat(ButtonStateData _buttonStateData) {
        base.ButtonFormat(_buttonStateData);
    //Disable all GameObjects except the right one
        TrySetActive(goNeutral, false);
        TrySetActive(goFocus, false);
        TrySetActive(goSelected, false);
        TrySetActive(goActive, false);
        TrySetActive(goActiveFocus, false);
        TrySetActive(goActiveSelected, false);
        TrySetActive(goUnavailable, false);
        TrySetActive(goUnavailableFocus, false);
        switch (buttonState) {
            case BState.Neutral: TrySetActive(goNeutral, true); break;
            case BState.Focus: TrySetActive(goFocus, true); break;
            case BState.Selected: TrySetActive(goSelected, true); break;
            case BState.Active: TrySetActive(goActive, true); break;
            case BState.ActiveFocus: TrySetActive(goActiveFocus, true); break;
            case BState.ActiveSelected: TrySetActive(goActiveSelected, true); break;
            case BState.Unavailable: TrySetActive(goUnavailable, true); break;
            case BState.UnavailableFocus: TrySetActive(goUnavailableFocus, true); break;
        }
    }

    private void TrySetActive(GameObject button, bool _active) {
        if (button != null) {
            button.SetActive(_active);
        }
    }
}