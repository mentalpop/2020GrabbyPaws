using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    public AnimatedUIContainer container;
    public ClickToClose clickToClose;
    public ListController listController;
    public MenuHub menuHub;
    public MenuNode listMenuNode;
    public ConfirmationPromptData promptData;

    private ShopUIData shopUIData;
    private ConfirmationWindow confirmationWindow;
    private bool awaitingConfirmation = false;

    public delegate void ShopEvent();
    public event ShopEvent OnShopClose = delegate { };

    private void OnEnable() {
        clickToClose.OnClick += Close;
        container.OnEffectComplete += Container_OnEffectComplete;
        Instance_OnUIScaled(UI.GetUIScale());
        UI.Instance.OnUIScaled += Instance_OnUIScaled;
        menuHub.OnMenuClose += MenuHub_OnMenuClose;
        listController.OnSelect += ListController_OnSelect;
    }

    private void OnDisable() {
        clickToClose.OnClick -= Close;
        container.OnEffectComplete -= Container_OnEffectComplete;
        UI.Instance.OnUIScaled -= Instance_OnUIScaled;
        menuHub.OnMenuClose -= MenuHub_OnMenuClose;
        listController.OnSelect -= ListController_OnSelect;
        if (awaitingConfirmation) {
            awaitingConfirmation = false;
            confirmationWindow.OnChoiceMade -= OnConfirm;
        }
    }

    public void Unpack(ShopUIData _shopUIData) {
        shopUIData = _shopUIData;
        foreach (Transform child in container.transform) {
            Destroy(child.gameObject);
        }
        GameObject newGO = Instantiate(_shopUIData.prefabShop, container.transform, false);
        //Populate listController with Elements
        listController.Elements = newGO.GetComponent<Shop>().Unpack(_shopUIData);
        menuHub.menuOnEnable = listMenuNode;
        MenuNavigator.Instance.MenuFocus(listMenuNode);
    }

    private void Container_OnEffectComplete(bool reverse) {
        if (reverse) {
            UI.SetControlState(false, gameObject); //De-register from UI
            gameObject.SetActive(false); //For now, just close instantly
            OnShopClose();
        } else {

        }
    }

    private void ListController_OnSelect(int index) {
        confirmationWindow = UI.RequestConfirmation(promptData, listMenuNode);
        confirmationWindow.OnChoiceMade += OnConfirm;
        awaitingConfirmation = true;
    }

    private void OnConfirm(bool _choice) {
        awaitingConfirmation = false;
        confirmationWindow.OnChoiceMade -= OnConfirm;
        if (_choice) {

        } else {
            Debug.Log("Did not purchase item");
        }
    }

    private void MenuHub_OnMenuClose() {
        Close();
    }

    public void Close() {
        if (!container.gTween.doReverse)
            container.gTween.Reverse();
    }

    private void Instance_OnUIScaled(float scale) {
        transform.localScale = new Vector3(scale, scale, 1f); //Z Scale MUST be 1f for 3D models to render!
    }
}