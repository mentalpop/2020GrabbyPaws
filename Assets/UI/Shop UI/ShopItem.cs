using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopItem : MonoBehaviour
{
    public ShopItemSlot itemSlot;
    public TextMeshProUGUI tmpCost;
    public Color colorHasEnough = Color.white;
    public Color colorNeedsMore = Color.red;
    //public STweenRectAnchor sTweenRectAnchor;
    public BTweenSystem bTweenSystem;
    public Item itemNull;

    private Item item;

    private void OnEnable() {
        Currency.instance.OnCashChanged += Currency_OnCashChanged;
    }

    private void OnDisable() {
        Currency.instance.OnCashChanged -= Currency_OnCashChanged;
    }

    public void Unpack(Item _item) {
        item = _item;
        itemSlot.Unpack(new InventoryItem(item, 1));
        tmpCost.text = item.value.ToString();
        RectTransform tRect = (RectTransform)tmpCost.transform;
        tRect.sizeDelta = new Vector2(tmpCost.preferredWidth, tRect.sizeDelta.y);
        Currency_OnCashChanged(Currency.instance.Cash, Currency.instance.Cash);
        //sTweenRectAnchor.Calibrate();
    }

    private void Currency_OnCashChanged(int cashOld, int cash) {
        tmpCost.color = (cash >= item.value) ? colorHasEnough : colorNeedsMore;
    }

    public void CanNotAfford() {
        bTweenSystem.PlayFromZero();
    }

    public void Purchase() {
        tmpCost.gameObject.SetActive(false);
        itemSlot.SetItemNull();
        InventoryItem _item = new InventoryItem(itemNull, 1);
        itemSlot.Unpack(_item);
    //Fallback; register "null" item as my item
        item = _item.item;
    }
}