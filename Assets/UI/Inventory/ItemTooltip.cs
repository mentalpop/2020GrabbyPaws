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
    public float offset = 16f;

    public void Unpack(InventoryItem inventoryItem, Vector2 _position) {
        itemName.text = inventoryItem.item.name;
        description.text = inventoryItem.item.description;
        weight.text = (inventoryItem.item.weight * inventoryItem.quantity).ToString();
        float _value = inventoryItem.item.value * inventoryItem.quantity;
        if (_value < 100f) {
    //Prepend zeroes in front of small numbers
            if (_value < 10f) {
                value.text = "00"+_value.ToString();
            } else {
                value.text = "0"+_value.ToString();
            }
        } else {
            value.text = string.Format("{0:n0}", _value);
        }
    //Keep on screen
        float _scale = UI.Instance.uiScale;
        transform.localScale = new Vector2(_scale, _scale); //Set scale first!
        tooltipOffset = new Vector2(tooltipOffset.x, tooltipOffset.y * _scale);
        offset *= _scale;
        Vector2 correctedPosition = Camera.main.WorldToScreenPoint(_position);
        correctedPosition = new Vector2(ScreenSpace.Inverse(correctedPosition.x), ScreenSpace.Inverse(correctedPosition.y));
        float yMax = (canvasSize.y - myRect.rect.height - offset) / 2f;
        myRect.anchoredPosition = new Vector3(Mathf.Clamp(correctedPosition.x + tooltipOffset.x, 0f, canvasSize.x - myRect.rect.width - offset),
            Mathf.Clamp(correctedPosition.y + tooltipOffset.y - canvasSize.y / 2f, -yMax, yMax), transform.position.z);
    }
}