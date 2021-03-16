using UnityEngine;

public class InventoryTab : ListElement
{
    public InventoryScrollRect inventoryScrollRect;

    private void Awake() {
//Move Scroll Rect Into Sibling Heirarchy
        //inventoryScrollRect.transform.SetAsLastSibling(); //Do this first to put it to the end of the list
        inventoryScrollRect.transform.parent = transform.parent;
        inventoryScrollRect.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
    //Reset z position on scrollRect
        //inventoryScrollRect.transform.position = new Vector3(inventoryScrollRect.transform.position.x, inventoryScrollRect.transform.position.y, 0f);
    }
}