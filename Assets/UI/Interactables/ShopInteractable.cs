using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.Events;

public class ShopInteractable : Interactable
{

    public Usable usable;
    private bool subbedEvent = false;
    private ShopUI activeShop;
    public ShopUIData shopUIData;

    public UnityEvent OnLoadAllItemsPurchased;

    private Dictionary<Item, bool> items = new Dictionary<Item, bool>();
    private bool allPurchased = true;

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
    private void Start() {
        for (int i = 0; i < shopUIData.items.Count; i++) {
            var purchased = UI.CompareChange(shopUIData.GetSaveID(i));
            items.Add(shopUIData.items[i], purchased);
            if (!purchased) {
                allPurchased = false;
            }
        }
        Debug.Log("allPurchased: " + shopUIData.name);
    }

    public override void Interact() {
        if (UI.LockControls) {
            return;
        }
        if (subbedEvent) {
            activeShop.OnShopClose -= ActiveShop_OnShopClose;
        }
    //All items purchased
        if (allPurchased) {
            OnLoadAllItemsPurchased.Invoke();
        } else {
    //Open the Shop
            activeShop = UI.Instance.DisplayShop(new ShopItemInventory(shopUIData, items));
        //Subscribe to the Close event on the Readable
            if (activeShop != null) {
                activeShop.OnShopClose += ActiveShop_OnShopClose;
                subbedEvent = true;
            }
        }
    }

    //Hook into a Usable and trigger its Deselect
    private void ActiveShop_OnShopClose() {
        if (usable != null) {
            usable.OnDeselectUsable();
        }
    }
}