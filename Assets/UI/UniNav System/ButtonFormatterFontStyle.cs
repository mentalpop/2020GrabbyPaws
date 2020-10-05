using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ButtonFormatterFontStyle : ButtonFormatterEightState
{
    public TextMeshProUGUI text;
    public FontStyles fontStyleNeutral = FontStyles.Normal;
    public FontStyles fontStyleFocus = FontStyles.Bold;
    public FontStyles fontStyleSelected = FontStyles.Normal;
    public FontStyles fontStyleActive = FontStyles.Normal;
    public FontStyles fontStyleActiveFocus = FontStyles.Bold;
    public FontStyles fontStyleActiveSelected = FontStyles.Normal;
    public FontStyles fontStyleUnavailable = FontStyles.Italic;

    protected override void ButtonFormat(ButtonStateData _buttonStateData) {
        base.ButtonFormat(_buttonStateData);
        //Debug.Log("buttonState: "+buttonState);
        switch (buttonState) {
            case BState.Neutral: text.fontStyle = fontStyleNeutral; break;
            case BState.Focus: text.fontStyle = fontStyleFocus; break;
            case BState.Selected: text.fontStyle = fontStyleSelected; break;
            case BState.Active: text.fontStyle = fontStyleActive; break;
            case BState.ActiveFocus: text.fontStyle = fontStyleActiveFocus; break;
            case BState.ActiveSelected: text.fontStyle = fontStyleActiveSelected; break;
            case BState.Unavailable: text.fontStyle = fontStyleUnavailable; break;
        }
    }
}
