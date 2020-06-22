using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFormatterTintImage : ButtonFormatterSevenState
{
    public Image image;
    public Color colorNeutral = Color.white;
    public Color colorFocus = Color.white;
    public Color colorSelected = Color.white;
    public Color colorActive = Color.white;
    public Color colorActiveFocus = Color.white;
    public Color colorActiveSelected = Color.white;
    public Color colorUnavailable = Color.white;

    protected override void ButtonFormat(ButtonStateData _buttonStateData) {
        base.ButtonFormat(_buttonStateData);
        switch (buttonState) {
            case BState.Neutral: image.color = colorNeutral; break;
            case BState.Focus: image.color = colorNeutral; break;
            case BState.Selected: image.color = colorNeutral; break;
            case BState.Active: image.color = colorNeutral; break;
            case BState.ActiveFocus: image.color = colorNeutral; break;
            case BState.ActiveSelected: image.color = colorNeutral; break;
            case BState.Unavailable: image.color = colorNeutral; break;
        }
    }
}