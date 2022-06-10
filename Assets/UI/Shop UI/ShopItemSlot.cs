using UnityEngine;
using TMPro;

public class ShopItemSlot : ListElement
{
    public float itemSpinSpeed = 72f;
    public Transform cube;
    public Vector2 tooltipOffset;

    private bool showTooltip = true;
    private GameObject model;
    private Quaternion initialRotation;
    private InventoryItem iItem;
    private bool mouseOver = false;
    private ItemTooltip iTooltip;

    protected override void OnEnable() {
        base.OnEnable();
        //navButton.OnSelectExt += NavButton_OnSelectExt; //Dropping items
        navButton.OnFocusGain += NavButton_OnFocusGain;
        navButton.OnFocusLost += NavButton_OnFocusLost;
    }

    private void NavButton_OnFocusGain(ButtonStateData _buttonStateData) {
        //Tooltip Handling
        if (showTooltip) {
            iTooltip.gameObject.SetActive(true);
            iTooltip.Unpack(iItem, transform);
            mouseOver = true;
        }
    }

    private void NavButton_OnFocusLost(ButtonStateData _buttonStateData) {
        CloseTooltip();
    }

    protected override void OnDisable() {
        base.OnDisable();
        //navButton.OnSelectExt -= NavButton_OnSelectExt;
        navButton.OnFocusGain -= NavButton_OnFocusGain;
        navButton.OnFocusLost -= NavButton_OnFocusLost;
        CloseTooltip();
    }

    void Update() {
        if (mouseOver && model != null)
            model.transform.Rotate(0, Time.deltaTime * itemSpinSpeed, 0);
    }

    public void Unpack(InventoryItem _item) {
        iTooltip = Inventory.GetItemTooltip();
        iItem = _item;
        if (iItem.item.model != null) {
            model = Instantiate(iItem.item.model, cube);
            model.transform.localPosition = iItem.item.positionOffset;
            initialRotation = Quaternion.Euler(iItem.item.rotationOffset);
            model.transform.rotation = initialRotation;
            model.transform.localScale = new Vector3(iItem.item.itemScale, iItem.item.itemScale, iItem.item.itemScale);
            //model.layer = 5; //UI - Does not apply to children
            StaticMethods.SetLayerRecursively(model.transform, 5); //UI
        }
    }

    private void CloseTooltip() {
        //Tooltip Handling
        if (showTooltip) {
            if (mouseOver) {
                iTooltip.gameObject.SetActive(false);
                mouseOver = false;
            }
            if (model != null) {
                model.transform.rotation = initialRotation;
            }
        }
    }

    public void SetItemNull() {
        foreach (Transform child in cube.transform) {
            Destroy(child.gameObject);
        }
        showTooltip = false;
    }
}