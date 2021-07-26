using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StolenItemPrompt : MonoBehaviour
{
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI value;

    public void Unpack(InventoryItem inventoryItem) {
        itemName.text = inventoryItem.item.name;
        value.text = UI.ValueFormat(inventoryItem.item.value * inventoryItem.quantity);
        float _scale = UI.GetUIScale();
        transform.localScale = new Vector2(_scale, _scale);
    }
}