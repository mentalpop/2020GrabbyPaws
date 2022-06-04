using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class ReadablePC : MonoBehaviour
{
    public MenuHub menuHub;
    public AnimatedUIContainer container;
    public ClickToClose clickToClose;
    public TextMeshProUGUI header;
    public TextMeshProUGUI windowText;
    public RectTransform windowContent;
    public ListUnpacker listUnpacker;
    public Scrollbar contentScrollbar;

    private ReadablePCData rData;

    private void OnEnable() {
        clickToClose.OnClick += Close;
        container.OnEffectComplete += Container_OnEffectComplete;
        listUnpacker.listController.OnSelect += ListController_OnSelect;
        menuHub.OnMenuClose += MenuNavigator_OnClose;
        Instance_OnUIScaled(UI.GetUIScale());
        UI.Instance.OnUIScaled += Instance_OnUIScaled;
    }

    private void OnDisable() {
        clickToClose.OnClick -= Close;
        container.OnEffectComplete -= Container_OnEffectComplete;
        listUnpacker.listController.OnSelect -= ListController_OnSelect;
        menuHub.OnMenuClose -= MenuNavigator_OnClose;
        UI.Instance.OnUIScaled -= Instance_OnUIScaled;
    }

    public void Unpack(ReadablePCData _rData) {
        rData = _rData;
        header.text = _rData.title;
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

    private void MenuNavigator_OnClose() {
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

    private void Instance_OnUIScaled(float scale) {
        transform.localScale = new Vector3(scale, scale, 1f); //Z Scale MUST be 1f for 3D models to render!
    }
}