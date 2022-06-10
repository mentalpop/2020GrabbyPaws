using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopUIData", menuName = "ShopUIData", order = 1)]
public class ShopUIData : ScriptableObject
{
    public List<Item> items = new List<Item>();
    public GameObject prefabShop;

    public string GetSaveID(int index) {
        return string.Format("{0}_{1}_{2}", UI.GetCurrentFile(), name, index);
    }
}

public class ShopItemInventory
{
    public ShopUIData shopUIData;
    public Dictionary<Item, bool> items = new Dictionary<Item, bool>();

    public ShopItemInventory(ShopUIData shopUIData, Dictionary<Item, bool> items) {
        this.shopUIData = shopUIData;
        this.items = items;
    }
}