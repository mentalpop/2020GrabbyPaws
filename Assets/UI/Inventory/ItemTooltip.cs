using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemTooltip : MonoBehaviour
{
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI description;
    public TextMeshProUGUI weight;
    public TextMeshProUGUI value;
    public RectTransform myRect;
    public Vector2 canvasSize;
    public Vector2 tooltipOffset;
    //public float offset = 120;

    private Transform myInventorySlotTransform;
    private RectTransform rect;

    private void Awake() {
        rect = (RectTransform)transform;
    }

    public void Unpack(InventoryItem inventoryItem) {
        itemName.text = inventoryItem.item.name;
        description.text = inventoryItem.item.description;
        weight.text = (inventoryItem.item.weight * inventoryItem.quantity).ToString();
        //float _value = inventoryItem.item.value * inventoryItem.quantity;
        value.text = StaticMethods.ValueFormat(inventoryItem.item.value * inventoryItem.quantity);
    }

    public void Unpack(InventoryItem inventoryItem, Transform _transform) {
        itemName.text = inventoryItem.item.name;
        description.text = inventoryItem.item.description;
        weight.text = (inventoryItem.item.weight * inventoryItem.quantity).ToString();
        value.text = StaticMethods.ValueFormat(inventoryItem.item.value * inventoryItem.quantity);
        myInventorySlotTransform = _transform;
        BindRect();
    }

    private void LateUpdate() {
        if (myInventorySlotTransform != null) {
        //Snap to position of transform plus offset
            Transform previousParent = transform.parent;
            transform.parent = myInventorySlotTransform;
            myRect.anchoredPosition = tooltipOffset;
            transform.parent = previousParent;
            BindRect();
        }
    }

    public void BindRect() {
        StaticMethods.BindRect(rect, UI.Instance.UICanvasRect.sizeDelta - rect.sizeDelta);
    }
}