using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReadablePC : MonoBehaviour
{
    public AnimatedUIContainer container;
    public ClickToClose clickToClose;
    public TextMeshProUGUI header;
    public TextMeshProUGUI windowText;

    private void OnEnable() {
        clickToClose.OnClick += Close;
        container.OnEffectComplete += Container_OnEffectComplete;
    }

    private void OnDisable() {
        clickToClose.OnClick -= Close;
        container.OnEffectComplete -= Container_OnEffectComplete;
    }

    public void Unpack(ReadablePCData rData) {
        header.text = rData.title;
        windowText.text = rData.contents;
    }

    private void Container_OnEffectComplete(bool reverse) {
        if (reverse) {
            UI.SetControlState(false, gameObject); //De-register from UI
            gameObject.SetActive(false); //For now, just close instantly
        } else {

        }
    }

    public void Close() {
        if (!container.gTween.doReverse)
            container.gTween.Reverse();
    }
}