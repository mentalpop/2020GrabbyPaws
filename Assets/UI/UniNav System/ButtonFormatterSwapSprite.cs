using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFormatterSwapSprite : ButtonFormatterEightState
{
    public Image image;
    public Sprite spriteNeutral;
    public Sprite spriteFocus;
    public Sprite spriteSelected;
    public Sprite spriteActive;
    public Sprite spriteActiveFocus;
    public Sprite spriteActiveSelected;
    public Sprite spriteUnavailable;

    protected override void ButtonFormat(ButtonStateData _buttonStateData) {
        base.ButtonFormat(_buttonStateData);
        //Debug.Log("buttonState: "+buttonState);
        switch (buttonState) {
            case BState.Neutral: image.sprite = spriteNeutral; break;
            case BState.Focus: image.sprite = spriteFocus; break;
            case BState.Selected: image.sprite = spriteSelected; break;
            case BState.Active: image.sprite = spriteActive; break;
            case BState.ActiveFocus: image.sprite = spriteActiveFocus; break;
            case BState.ActiveSelected: image.sprite = spriteActiveSelected; break;
            case BState.Unavailable: image.sprite = spriteUnavailable; break;
        }
    }
}