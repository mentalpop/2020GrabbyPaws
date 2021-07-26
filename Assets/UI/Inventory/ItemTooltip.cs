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
    public float offset = 120;

    public void Unpack(InventoryItem inventoryItem) {
        itemName.text = inventoryItem.item.name;
        description.text = inventoryItem.item.description;
        weight.text = (inventoryItem.item.weight * inventoryItem.quantity).ToString();
        //float _value = inventoryItem.item.value * inventoryItem.quantity;
        value.text = UI.ValueFormat(inventoryItem.item.value * inventoryItem.quantity);
        //Keep on screen
        float _scale = UI.GetUIScale();
        transform.localScale = new Vector2(_scale, _scale); //Set scale first!
    }

    public void Unpack(InventoryItem inventoryItem, Transform _transform) {
        itemName.text = inventoryItem.item.name;
        description.text = inventoryItem.item.description;
        weight.text = (inventoryItem.item.weight * inventoryItem.quantity).ToString();
        value.text = UI.ValueFormat(inventoryItem.item.value * inventoryItem.quantity);
        //Snap to position of transform plus offset
        Transform previousParent = transform.parent;
        transform.parent = _transform;
        myRect.anchoredPosition = tooltipOffset;
        transform.parent = previousParent;
        /*
        float _scale = UI.GetUIScale();
        transform.localScale = new Vector2(_scale, _scale); //Set scale first!
        tooltipOffset = new Vector2(tooltipOffset.x, tooltipOffset.y * _scale);
        Vector2 correctedPosition = Camera.main.WorldToScreenPoint(_position);
        correctedPosition = new Vector2(ScreenSpace.Inverse(correctedPosition.x), ScreenSpace.Inverse(correctedPosition.y));
        float yMax = (canvasSize.y - myRect.rect.height - offset * _scale) / 2f;
        //Debug.Log("yMax: "+yMax);
        myRect.anchoredPosition = new Vector3(Mathf.Clamp(correctedPosition.x + tooltipOffset.x, 0f, canvasSize.x - myRect.rect.width - offset * _scale),
            Mathf.Clamp(correctedPosition.y + tooltipOffset.y - canvasSize.y / 2f, -yMax, yMax), transform.position.z);
        //*/
    }
}