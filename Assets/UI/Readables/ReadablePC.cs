using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class ReadablePC : MonoBehaviour
{
    public AnimatedUIContainer container;
    public ClickToClose clickToClose;
    public TextMeshProUGUI header;
    public TextMeshProUGUI windowText;
    public RectTransform windowContent;
    public ListUnpacker listUnpacker;
    public Scrollbar contentScrollbar;
    public MenuNavigator menuNavigator;

    private ReadablePCData rData;

    private void OnEnable() {
        clickToClose.OnClick += Close;
        container.OnEffectComplete += Container_OnEffectComplete;
        listUnpacker.listController.OnSelect += ListController_OnSelect;
        menuNavigator.OnClose += MenuNavigator_OnClose;
        float _scale = UI.GetUIScale();
        transform.localScale = new Vector2(_scale, _scale); //Set scale first!
    }

    private void OnDisable() {
        clickToClose.OnClick -= Close;
        container.OnEffectComplete -= Container_OnEffectComplete;
        listUnpacker.listController.OnSelect -= ListController_OnSelect;
    }

    public void Unpack(ReadablePCData _rData) {
        rData = _rData;
        header.text = _rData.header;
        //windowText.text = _rData.data[0].contents;
        List<NavButtonData> parentList = _rData.data.Cast<NavButtonData>().ToList();
        listUnpacker.Unpack(parentList);
    }

    private void Container_OnEffectComplete(bool reverse) {
        if (reverse) {
            UI.SetControlState(false, gameObject); //De-register from UI
            gameObject.SetActive(false); //For now, just close instantly
        } else {

        }
    }

    private void MenuNavigator_OnClose(MenuNode menuNode) {
        Close();
    }

    public void Close() {
        if (!container.gTween.doReverse)
            container.gTween.Reverse();
    }

    private void ListController_OnSelect(int index) {
        windowText.text = rData.data[index].contents;
        windowText.rectTransform.sizeDelta = new Vector2(windowText.rectTransform.sizeDelta.x, windowText.preferredHeight);
        windowContent.sizeDelta = new Vector2(windowContent.sizeDelta.x, windowText.preferredHeight);
        contentScrollbar.value = 0f;
    }
}