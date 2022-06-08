using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public RectTransform itemContainer;
    public GameObject prefabShopItem;

    [HideInInspector] public List<ShopItem> items = new List<ShopItem>();
    [HideInInspector] public List<bool> itemPurchased = new List<bool>();

    public List<ListElement> Unpack(ShopUIData shopUIData) {
        List<ListElement> _elements = new List<ListElement>();
        foreach (var item in shopUIData.items) {
            GameObject newGO = Instantiate(prefabShopItem, itemContainer, false);
            ShopItem shopItem = newGO.GetComponent<ShopItem>();
            shopItem.Unpack(item);
            items.Add(shopItem);
            _elements.Add(shopItem.inventorySlot);
            itemPurchased.Add(false); //No items have been purchased
        }
        return _elements;
    }

    public void Purchase(int _index) {
        itemPurchased[_index] = true;
        items[_index].Purchase();
    }

    public void CanNotAfford(int _index) {
        items[_index].CanNotAfford();
    }
}