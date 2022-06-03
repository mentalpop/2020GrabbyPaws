using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    public AnimatedUIContainer container;
    public ClickToClose clickToClose;

    private ShopUIData shopUIData;

    public delegate void ShopEvent();
    public event ShopEvent OnShopClose = delegate { };

    private void OnEnable() {
        clickToClose.OnClick += Close;
        container.OnEffectComplete += Container_OnEffectComplete;
        float _scale = UI.GetUIScale();
        transform.localScale = new Vector2(_scale, _scale); //Set scale first!
    }

    private void OnDisable() {
        clickToClose.OnClick -= Close;
        container.OnEffectComplete -= Container_OnEffectComplete;
    }

    public void Unpack(ShopUIData _shopUIData) {
        shopUIData = _shopUIData;
        foreach (Transform child in container.transform) {
            Destroy(child.gameObject);
        }
        GameObject newGO = Instantiate(_shopUIData.prefabShop, container.transform, false);
        newGO.GetComponent<Shop>().Unpack(_shopUIData);
    }

    private void Container_OnEffectComplete(bool reverse) {
        if (reverse) {
            UI.SetControlState(false, gameObject); //De-register from UI
            gameObject.SetActive(false); //For now, just close instantly
            OnShopClose();
        } else {

        }
    }

    public void Close() {
        if (!container.gTween.doReverse)
            container.gTween.Reverse();
    }
}