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

    private void OnEnable() {
        clickToClose.OnClick += Close;
        container.OnEffectComplete += Container_OnEffectComplete;
    }

    private void OnDisable() {
        clickToClose.OnClick -= Close;
        container.OnEffectComplete -= Container_OnEffectComplete;
    }

    public void Unpack(ReadableData rData) {
        bookTitle.text = rData.title;
        bookText.text = rData.contents;
    }

    private void Container_OnEffectComplete(bool reverse) {
        if (reverse) {
            UI.MouseSetState(false, gameObject); //De-register from UI
            gameObject.SetActive(false); //For now, just close instantly
        } else {

        }
    }

    public void Close() {
        if (!container.gTween.doReverse)
            container.gTween.Reverse();
    }

    /*
    public void Close() {
        gameObject.SetActive(false); //For now, just close instantly
    }
    //*/
}