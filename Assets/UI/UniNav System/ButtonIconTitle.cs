using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class ButtonIconTitle : NavButton
{
    public TextMeshProUGUI myTitle;
    public Image icon;

    public override void Unpack(NavButtonData _navButtonData) {
        base.Unpack(_navButtonData);
        NavButtonDataIconTitle navButtonDataPCTitles = (NavButtonDataIconTitle)_navButtonData;
        myTitle.text = navButtonDataPCTitles.title;
        icon.sprite = navButtonDataPCTitles.icon;
        icon.SetNativeSize();
    }
}