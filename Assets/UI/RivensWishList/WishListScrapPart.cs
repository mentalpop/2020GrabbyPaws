using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;

public class WishListScrapPart : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI quantity;
    public Color colorHasEnough = Color.white;
    public Color colorNeedsMore = Color.red;
    public GameObject tooltipPrefab;
    public Vector2 tooltipOffset;

    private WishListItemQuantity myItem;
    private WishListTooltip wishlistTooltip;

    public void Unpack(WishListItemQuantity _myItem) {
        myItem = _myItem;
        itemName.text = _myItem.item.name;
        UpdateQuantity();
    }

    private void OnEnable() {
        if (myItem != null) {
            UpdateQuantity();
        }
    }

    public void UpdateQuantity() {
        int quantityHas = (int)Inventory.instance.InventoryCount(myItem.item.name);
        quantity.text = quantityHas + "/" + myItem.quantity;
        quantity.color = (quantityHas >= myItem.quantity) ? colorHasEnough : colorNeedsMore;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        GameObject newGO = Instantiate(tooltipPrefab, UI.Instance.lappy.transform, false);
        newGO.transform.position = new Vector3(transform.position.x + tooltipOffset.x, transform.position.y + tooltipOffset.y, transform.position.z);
        wishlistTooltip = newGO.GetComponent<WishListTooltip>();
        wishlistTooltip.inventorySlot.Unpack(new InventoryItem(myItem.item, (int)Inventory.instance.InventoryCount(myItem.item.name)));
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (wishlistTooltip != null) {
            Destroy(wishlistTooltip.gameObject);
        }
    }
}