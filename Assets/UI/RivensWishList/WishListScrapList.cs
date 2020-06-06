using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WishListScrapList : MonoBehaviour
{
    public GameObject listItemPrefab;

    public void Unpack(List<WishListItemQuantity> wishListItemQuantities) {
        foreach (var item in wishListItemQuantities) {
            GameObject newGO = Instantiate(listItemPrefab, transform, false);
            WishListScrapPart part = newGO.GetComponent<WishListScrapPart>();
            part.Unpack(item);
        }
    }
}