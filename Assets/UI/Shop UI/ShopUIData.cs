using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopUIData", menuName = "ShopUIData", order = 1)]
public class ShopUIData : ScriptableObject
{
    public List<Item> items = new List<Item>();
    public GameObject prefabShop;
}