using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Readable : MonoBehaviour
{
    public AnimatedUIContainer container;
    public ClickToClose clickToClose;
    public TextMeshProUGUI bookTitle;
    public TextMeshProUGUI bookText;

    private ReadableData readableData;

    public delegate void ReadableEvent(ReadableData data);
    public event ReadableEvent OnReadableClose = delegate { };

    private void OnEnable() {
        clickToClose.OnClick += Close;
        container.OnEffectComplete += Container_OnEffectComplete;
        Instance_OnUIScaled(UI.GetUIScale());
        UI.Instance.OnUIScaled += Instance_OnUIScaled;
    }

    private void OnDisable() {
        clickToClose.OnClick -= Close;
        container.OnEffectComplete -= Container_OnEffectComplete;
        UI.Instance.OnUIScaled -= Instance_OnUIScaled;
    }

    public void Unpack(ReadableData rData) {
        readableData = rData;
        bookTitle.text = rData.title;
        bookText.text = rData.contents;
    }

    private void Container_OnEffectComplete(bool reverse) {
        if (reverse) {
            UI.SetControlState(false, gameObject); //De-register from UI
            gameObject.SetActive(false); //For now, just close instantly
            OnReadableClose(readableData);
        } else {

        }
    }

    public void Close() {
        if (!container.gTween.doReverse)
            container.gTween.Reverse();
    }

    private void Instance_OnUIScaled(float scale) {
        transform.localScale = new Vector3(scale, scale, 1f); //Z Scale MUST be 1f for 3D models to render!
    }
}