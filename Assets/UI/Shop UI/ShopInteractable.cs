using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class ShopInteractable : Interactable
{

    public Usable usable;
    private bool subbedEvent = false;
    private ShopUI activeShop;
    public ShopUIData shopUIData;

    private void Awake() {
        gameObject.AddComponent<cakeslice.Outline>();
    }

    private void OnValidate() {
        if (usable == null) {
            usable = GetComponent<Usable>();
        }
    }

    private void OnDisable() {
        if (subbedEvent) {
            activeShop.OnShopClose -= ActiveShop_OnShopClose;
        }
    }

    public override void Interact() {
        if (UI.LockControls) {
            return;
        }
        if (subbedEvent) {
            activeShop.OnShopClose -= ActiveShop_OnShopClose;
        }
        activeShop = UI.Instance.DisplayShop(shopUIData);
        //Subscribe to the Close event on the Readable
        if (activeShop != null) {
            activeShop.OnShopClose += ActiveShop_OnShopClose;
            subbedEvent = true;
        }
    }

    //Hook into a Usable and trigger its Deselect
    private void ActiveShop_OnShopClose() {
        if (usable != null) {
            usable.OnDeselectUsable();
        }
    }
}