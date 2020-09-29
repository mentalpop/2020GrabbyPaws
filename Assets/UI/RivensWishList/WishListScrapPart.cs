using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;

public class WishListScrapPart : MonoBehaviour
{
    public NavButton navButton;
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
        navButton.OnFocusGain += NavButton_OnFocusGain;
        navButton.OnFocusLost += NavButton_OnFocusLost;
    }

    private void OnDisable() {
        navButton.OnFocusGain -= NavButton_OnFocusGain;
        navButton.OnFocusLost -= NavButton_OnFocusLost;
        CloseTooltip();
    }

    public void UpdateQuantity() {
        int quantityHas = (int)Inventory.instance.InventoryCount(myItem.item.name);
        quantity.text = quantityHas + "/" + myItem.quantity;
        quantity.color = (quantityHas >= myItem.quantity) ? colorHasEnough : colorNeedsMore;
    }

    public void NavButton_OnFocusGain(ButtonStateData _buttonStateData) {
        if (wishlistTooltip == null) { //Don't create a Tooltip if one already exists
            GameObject newGO = Instantiate(tooltipPrefab, UI.Instance.lappy.transform, false);
            newGO.transform.position = new Vector3(transform.position.x + tooltipOffset.x, transform.position.y + tooltipOffset.y, transform.position.z);
            wishlistTooltip = newGO.GetComponent<WishListTooltip>();
            wishlistTooltip.inventorySlot.Unpack(new InventoryItem(myItem.item, (int)Inventory.instance.InventoryCount(myItem.item.name)));
        }
    }

    public void NavButton_OnFocusLost(ButtonStateData _buttonStateData) {
        CloseTooltip();
    }

    public void CloseTooltip() {
        if (wishlistTooltip != null) {
            Destroy(wishlistTooltip.gameObject);
        }
    }
}