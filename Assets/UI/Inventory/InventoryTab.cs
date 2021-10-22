using UnityEngine;

public class InventoryTab : ListElement
{
    public InventoryScrollRect inventoryScrollRect;

    //private RectTransform myRect;

    private void Awake() {
        //myRect = (RectTransform)transform;
        transform.position = new Vector3(transform.position.x, transform.position.y, -12);
        //Move Scroll Rect Into Sibling Heirarchy
        //inventoryScrollRect.transform.SetAsLastSibling(); //Do this first to put it to the end of the list
        inventoryScrollRect.transform.parent = transform.parent;
        inventoryScrollRect.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
    //Reset z position on scrollRect
        //inventoryScrollRect.transform.position = new Vector3(inventoryScrollRect.transform.position.x, inventoryScrollRect.transform.position.y, 0f);
    }

    public void BringToFront() {
        inventoryScrollRect.transform.position = new Vector3(inventoryScrollRect.transform.position.x, inventoryScrollRect.transform.position.y, -9);
        //transform.position = new Vector3(transform.position.x, transform.position.y, -12);
    }

    public void SendToBack() {
        inventoryScrollRect.transform.position = new Vector3(inventoryScrollRect.transform.position.x, inventoryScrollRect.transform.position.y, -4);
        //transform.position = new Vector3(transform.position.x, transform.position.y, -12);
    }
}