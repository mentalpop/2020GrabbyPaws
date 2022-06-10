using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public RectTransform itemContainer;
    public GameObject prefabShopItem;

    [HideInInspector] public List<ShopItem> items = new List<ShopItem>();
    //[HideInInspector] public List<bool> itemPurchased = new List<bool>();
    [HideInInspector] public ShopItemInventory shopItemInventory;

    public List<ListElement> Unpack(ShopItemInventory _shopItemInventory) {
        List<ListElement> _elements = new List<ListElement>();
        shopItemInventory = _shopItemInventory;
        foreach (var item in shopItemInventory.items) {
            GameObject newGO = Instantiate(prefabShopItem, itemContainer, false);
            ShopItem shopItem = newGO.GetComponent<ShopItem>();
            if (item.Value) {
                shopItem.Purchase();
            } else {
                shopItem.Unpack(item.Key);
            }
            items.Add(shopItem);
            _elements.Add(shopItem.itemSlot);
            //itemPurchased.Add(false); //No items have been purchased
        }
        return _elements;
    }

    public void Purchase(Item item, int _index) {
        //itemPurchased[_index] = true;
        shopItemInventory.items[item] = true;
        items[_index].Purchase();
        UI.RegisterChange(shopItemInventory.shopUIData.GetSaveID(_index), true);
    }

    public void CanNotAfford(int _index) {
        items[_index].CanNotAfford();
    }
}