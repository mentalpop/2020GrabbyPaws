using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GadgetData", menuName = "GadgetData", order = 7)]
public class GadgetData : ScriptableObject
{
    public string gadgetName;
    [TextArea(3,10)]
    public string description;
    public Sprite blueprintSprite;
    public List<WishListItemQuantity> items = new List<WishListItemQuantity>();
}

[System.Serializable]
public class WishListItemQuantity
{
    public Item item;
    public int quantity;
}