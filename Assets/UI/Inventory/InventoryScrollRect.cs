using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScrollRect : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform contentTransform;
    public ScrollResize scrollResize;
    //public InventoryDisplay inventoryDisplay;

    private List<GameObject> slots = new List<GameObject>();

    private void OnEnable() {
        scrollResize.OnClose += SlotsClear;
    }

    private void OnDisable() {
        scrollResize.OnClose -= SlotsClear;
    }

    public void Unpack(List<InventoryItem> items, CategoryItem categoryItem) {//inventoryDisplay.inventoryDisplayType
    //Clear the slots first
        SlotsClear();
        foreach (var iItem in items) {
    //Instantiate each item
            if (iItem.item.category == categoryItem) {
                GameObject gameObject = Instantiate(slotPrefab, contentTransform, false);
                slots.Add(gameObject);
                InventorySlot slot = gameObject.GetComponent<InventorySlot>();
                slot.Unpack(iItem);
            }
        }
        scrollResize.RectResize(slots.Count);
    }

    public void SlotsClear() {
        if (slots.Count > 0) {
            foreach (var slot in slots) {
                Destroy(slot);
            }
            slots.Clear();
        }
    }
}