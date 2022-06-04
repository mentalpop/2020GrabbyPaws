using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public RectTransform itemContainer;
    public GameObject prefabShopItem;

    public List<ListElement> Unpack(ShopUIData shopUIData) {
        List<ListElement> _elements = new List<ListElement>();
        foreach (var item in shopUIData.items) {
            GameObject newGO = Instantiate(prefabShopItem, itemContainer, false);
            ShopItem shopItem = newGO.GetComponent<ShopItem>();
            shopItem.Unpack(item);
            _elements.Add(shopItem.inventorySlot);
        }
        return _elements;
    }
}