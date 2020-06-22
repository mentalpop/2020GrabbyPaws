using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonPCTitles : NavButton
{
    public RectTransform myRect;
    public List<TextMeshProUGUI> titles = new List<TextMeshProUGUI>();

    public override void Unpack(NavButtonData _navButtonData) {
        base.Unpack(_navButtonData);
        NavButtonDataPCTitles navButtonDataPCTitles = (NavButtonDataPCTitles)_navButtonData;
        string _text = navButtonDataPCTitles.title; //[navButton.GetComponent<ListElement>().listIndex]
        float _greatestHeight = 0f;
        foreach (var title in titles) {
            title.text = ">" + _text.ToUpper();
            float _check = title.preferredHeight;
            if (_check > _greatestHeight)
                _greatestHeight = _check;
            //title.autoSizeTextContainer = true;
        }
        myRect.sizeDelta = new Vector2(myRect.sizeDelta.x, _greatestHeight);
    }
}