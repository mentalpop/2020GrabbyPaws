using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButtonText : NavButton
{
    public List<TextMeshProUGUI> titles = new List<TextMeshProUGUI>();

    public override void Unpack(NavButtonData _navButtonData) {
        base.Unpack(_navButtonData);
        NavButtonDataTitle nBDTitle = (NavButtonDataTitle)_navButtonData;
        string _text = nBDTitle.title.ToUpper(); //Ensure text is uppercase
        foreach (var title in titles) {
            title.text = _text;
        }
    }
}