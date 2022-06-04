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
        Instance_OnUIScaled(UI.GetUIScale());
        UI.Instance.OnUIScaled += Instance_OnUIScaled;
    }

    private void OnDisable() {
        clickToClose.OnClick -= Close;
        container.OnEffectComplete -= Container_OnEffectComplete;
        UI.Instance.OnUIScaled -= Instance_OnUIScaled;
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

    private void Instance_OnUIScaled(float scale) {
        transform.localScale = new Vector3(scale, scale, 1f); //Z Scale MUST be 1f for 3D models to render!
    }
}