using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScrollRect : MonoBehaviour
{
    public ListController listController;
    public GameObject slotPrefab;
    public GameObject keyItemBlockerPrefab;
    public Transform contentTransform;
    public ScrollResize scrollResize;
    //public InventoryDisplay inventoryDisplay;

    private List<GameObject> slots = new List<GameObject>();
    private CategoryItem categoryItem;

    private void OnEnable() {
        scrollResize.OnClose += SlotsClear;
    }

    private void OnDisable() {
        scrollResize.OnClose -= SlotsClear;
    }

    public void Unpack(CategoryItem _categoryItem) {//inventoryDisplay.inventoryDisplayType
        categoryItem = _categoryItem;
        //Key item Blocker
        if (categoryItem == CategoryItem.Key) {
            GameObject newGO = Instantiate(keyItemBlockerPrefab, transform, false);
            newGO.transform.position = new Vector3(newGO.transform.position.x, newGO.transform.position.y, -12);
        }
    }

    public void UpdateItems(List<InventoryItem> items) {
        //Clear the slots first
        SlotsClear();
        List<ListElement> _elements = new List<ListElement>();
        foreach (var iItem in items) {
            //Instantiate each item
            if (iItem.item.category == categoryItem) {
                GameObject buttonClone = Instantiate(slotPrefab, contentTransform, false);
                slots.Add(buttonClone);
                InventorySlot slot = buttonClone.GetComponent<InventorySlot>();
                slot.Unpack(iItem);
                ListElement liEl = buttonClone.GetComponent<ListElement>();
                _elements.Add(liEl);
            }
        }
        if (slots.Count == 0) {
            //Add a Null Item
            GameObject buttonClone = Instantiate(slotPrefab, contentTransform, false);
            slots.Add(buttonClone);
            InventorySlot slot = buttonClone.GetComponent<InventorySlot>();
            slot.Unpack(Inventory.instance.nullItem);
            ListElement liEl = buttonClone.GetComponent<ListElement>();
            _elements.Add(liEl);
        }
        scrollResize.RectResize(slots.Count);
        listController.Elements = _elements;
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