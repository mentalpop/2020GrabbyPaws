using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WishListScrapList : MonoBehaviour
{
    public GameObject listItemPrefab;
	public ListController listController;

    public void Unpack(List<WishListItemQuantity> wishListItemQuantities) {
        List<ListElement> _elements = new List<ListElement>();
        foreach (var item in wishListItemQuantities) {
            GameObject newGO = Instantiate(listItemPrefab, transform, false);
            WishListScrapPart part = newGO.GetComponent<WishListScrapPart>();
            ListElement liEl = newGO.GetComponent<ListElement>();
            _elements.Add(liEl);
            part.Unpack(item);
        }
		listController.Elements = _elements;
    }
}