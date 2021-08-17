using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    public AnimatedUIContainer container;
    public MenuHub menuHub;
    public Inventory inventory;
    
    public ListController inventoryTabMenu;
    public Transform tabSortMenu;
    public Transform menuBGTransform;
    public BottomCapAdjust bottomCapAdjust;

    [HideInInspector] public CategoryItem inventoryDisplayType;
    [HideInInspector] public InventoryScrollRect inventoryScrollRect;

    private List<InventoryTab> iTabs = new List<InventoryTab>();
    private bool subResizeEvent = false;

    private void OnEnable() {
        inventoryTabMenu.OnSelect += SetActiveTab;
        inventory.OnItemChanged += UpdateDisplay;
        container.OnEffectComplete += Container_OnEffectComplete;
        menuHub.OnMenuClose += MenuHub_OnMenuClose;
        if (!subResizeEvent && inventoryScrollRect != null) {
            subResizeEvent = true;
            inventoryScrollRect.scrollResize.OnResize += AdjustBottomCap;
        }
    }

    private void OnDisable() {
        inventoryTabMenu.OnSelect -= SetActiveTab;
        inventory.OnItemChanged -= UpdateDisplay;
        container.OnEffectComplete -= Container_OnEffectComplete;
        menuHub.OnMenuClose -= MenuHub_OnMenuClose;
        if (subResizeEvent) {
            inventoryScrollRect.scrollResize.OnResize -= AdjustBottomCap;
        }
    }

    private void Container_OnEffectComplete(bool reverse) {
        if (reverse) {
            gameObject.SetActive(false);
        }
    }

    private void MenuHub_OnMenuClose() {
        UI.Instance.ShowInventoryDisplay();
        //Close(); //Calling this by itself doesn't actually unpause the game
    }

    public void Close() {
        if (!container.gTween.doReverse)
            container.gTween.Reverse();
    }

    void Start() {
        //Instantiate tabs and create a local list
        float _depthSet = 0f;
        for (int i = 0; i < inventoryTabMenu.Elements.Count; i++) {
            InventoryTab tab = inventoryTabMenu.Elements[i].GetComponent<InventoryTab>();
    //Manually set z depth of Tab Headers and Scroll Rects
            _depthSet = -2f * (i + 1);
            tab.transform.position = new Vector3(tab.transform.position.x, tab.transform.position.y, -3.5f * (i + 1));
            tab.inventoryScrollRect.transform.position = new Vector3(tab.inventoryScrollRect.transform.position.x, tab.inventoryScrollRect.transform.position.y, _depthSet);
            iTabs.Add(tab);
        }
        //Put the menu in front of the last tab
        //menuBGTransform.SetAsLastSibling();
        //menuBGTransform.position = new Vector3(menuBGTransform.position.x, menuBGTransform.position.y, _depthSet * 2f);
        /*
        foreach (var tabObj in inventoryTabMenu.tabs) {
            InventoryTab tab = tabObj.GetComponent<InventoryTab>();
            iTabs.Add(tab);
        }
        //*/
        if (subResizeEvent && inventoryScrollRect != null) {
            subResizeEvent = false;
            inventoryScrollRect.scrollResize.OnResize -= AdjustBottomCap;
        }
        inventoryScrollRect = iTabs[0].inventoryScrollRect;
        if (!subResizeEvent && inventoryScrollRect != null) {
            subResizeEvent = true;
            inventoryScrollRect.scrollResize.OnResize += AdjustBottomCap;
        }
    }

    private void AdjustBottomCap() {
        //bottomCapAdjust.UpdateHeight(inventoryScrollRect.scrollResize.myRect.rect.height);
        bottomCapAdjust.UpdateHeight(StaticMethods.SumHeightOfChildren(tabSortMenu));
    }

    public void SetActiveTab(int _activeTab) {
        inventoryDisplayType = (CategoryItem)_activeTab;
    //Tell the original scrollRect to collapse
        inventoryScrollRect.scrollResize.Collapse();
    //Assign the new ScrollRect and open it
        inventoryScrollRect = iTabs[_activeTab].inventoryScrollRect;
        //Debug.Log("inventoryDisplayType" + ": " + inventoryDisplayType);
        UpdateDisplay(null);
    //Set position in Heirarchy to be one more than the active tab
        /*
        inventoryScrollRect.transform.SetAsLastSibling(); //Do this first to put it to the end of the list
        inventoryScrollRect.transform.SetSiblingIndex(inventoryTabMenu.tabs[_activeTab].transform.GetSiblingIndex() + 1);
        //*/
        /*
        foreach (var scrollRect in inventoryScrollRect) {
            scrollRect.gameObject.SetActive(inventoryScrollRect.IndexOf(scrollRect) == _activeTab);
            
        }
        //*/
    }

    public void UpdateDisplay(Item item) {
        inventoryScrollRect.Unpack(inventory.items, inventoryDisplayType);
        
    }
}