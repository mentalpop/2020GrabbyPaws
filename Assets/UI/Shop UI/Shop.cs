using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public RectTransform itemContainer;
    public GameObject prefabShopItem;

    public void Unpack(ShopUIData shopUIData) {
        foreach (var item in shopUIData.items) {
            GameObject newGO = Instantiate(prefabShopItem, itemContainer, false);
            InventorySlot slot = newGO.GetComponent<InventorySlot>();
            slot.Unpack(new InventoryItem(item, 1));
        }
    }
}