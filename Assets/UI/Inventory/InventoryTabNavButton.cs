using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryTabNavButton : NavButton
{
    public TextMeshProUGUI tmpText;
    public Image icon;
    public ButtonFormatterTintImage ButtonFormatterTintImage;

    public override void Unpack(NavButtonData _navButtonData) {
        base.Unpack(_navButtonData);
        TabData _data = _navButtonData as TabData;
        icon.sprite = _data.icon;
        icon.SetNativeSize();
        tmpText.text = _data.text;
        ButtonFormatterTintImage.colorNeutral = _data.bgColor;
        ButtonFormatterTintImage.colorFocus = _data.bgColorHighlight;
        ButtonFormatterTintImage.colorSelected = _data.bgColor;
        ButtonFormatterTintImage.colorActive = _data.bgColor;
        ButtonFormatterTintImage.colorActiveFocus = _data.bgColorHighlight;
        ButtonFormatterTintImage.colorActiveSelected = _data.bgColor;
    }
}