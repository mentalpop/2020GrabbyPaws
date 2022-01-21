using UnityEngine;
using TMPro;

public class InventorySlot : ListElement {
    
    public float itemSpinSpeed = 72f;
    public Transform cube;
    public GameObject quantityDisplay;
    public TextMeshProUGUI quantity;
    public Vector2 tooltipOffset;
    //public Image icon;
    //public Button removeButton;

    //public ToolTip tip;
    //public GameObject modelTransform;
    private GameObject model;
    private Quaternion initialRotation;
    private InventoryItem iItem;
    private bool mouseOver = false;
    private ItemTooltip iTooltip;

    private void OnEnable() {
        navButton.OnSelectExt += NavButton_OnSelectExt;
        navButton.OnFocusGain += NavButton_OnFocusGain;
        navButton.OnFocusLost += NavButton_OnFocusLost;
    }

    private void NavButton_OnSelectExt(ButtonStateData _buttonStateData, object _data) {
        int choice = (int)_data;
        if (choice == 1) { //eventData.button == PointerEventData.InputButton.Right
            if (iItem.item.category == CategoryItem.Trash) {
                Inventory.instance.Drop(iItem.item);
                Debug.Log("Dropping: " + iItem.item.name);
            } else {
                Debug.Log("Can't drop item: " + iItem.item.name);
            }
        }
    }

    private void NavButton_OnFocusGain(ButtonStateData _buttonStateData) {
        //Tooltip Handling
        iTooltip.gameObject.SetActive(true);
        iTooltip.Unpack(iItem, transform);
        mouseOver = true;
    }

    private void NavButton_OnFocusLost(ButtonStateData _buttonStateData) {
        CloseTooltip();
    }

    private void OnDisable() {
        navButton.OnSelectExt -= NavButton_OnSelectExt;
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
        if (iItem.quantity > 1) {
            quantity.text = iItem.quantity.ToString();
        } else {
            quantityDisplay.SetActive(false);
        }
        if (iItem.item.model != null) {
            model = Instantiate(iItem.item.model, cube);
            //Debug.Log("cube transform: " + cube);
            model.transform.localPosition = iItem.item.positionOffset;
            initialRotation = Quaternion.Euler(iItem.item.rotationOffset);
            model.transform.rotation = initialRotation;
            model.transform.localScale = new Vector3(iItem.item.itemScale, iItem.item.itemScale, iItem.item.itemScale);
            //model.layer = 5; //UI - Does not apply to children
            StaticMethods.SetLayerRecursively(model.transform, 5); //UI
        }
        /*
        if (model.GetComponent<Rigidbody>() != null) {
            model.GetComponent<Rigidbody>().useGravity = false;
            //*/
        //*/
    }

    private void CloseTooltip() {
        //Tooltip Handling
        if (mouseOver) {
            iTooltip.gameObject.SetActive(false);
            mouseOver = false;
        }
        if (model != null) {
            model.transform.rotation = initialRotation;
        }
    }


    /*
    public void OnPointerEnter(PointerEventData eventData) {
        //Tooltip Handling
        iTooltip.gameObject.SetActive(true);
        //iTooltip.transform.position = new Vector3(transform.position.x + tooltipOffset.x, transform.position.y + tooltipOffset.y, transform.position.z);
        iTooltip.Unpack(iItem, transform.position);
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData) {
        //Tooltip Handling
        iTooltip.gameObject.SetActive(false);
        mouseOver = false;
        if (model != null) {
            model.transform.rotation = initialRotation;
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.button == PointerEventData.InputButton.Right) {
            if (iItem.item.category == CategoryItem.Trash) {
                Inventory.instance.Drop(iItem.item);
                Debug.Log("Dropping: " + iItem.item.name);
            } else {
                Debug.Log("Can't drop item: " + iItem.item.name);
            }
        }
    }
    public void Awake()
    {
        tip = FindObjectOfType<ToolTip>();
    }
    //*/

    /*
    public void AddItem(Item newItem)
    {
        item = newItem;

        icon.sprite = item.icon;

        if (model == null) {
            model = Instantiate(item.model, modelTransform.transform);
            model.transform.localPosition = new Vector3(0 + item.itemXOffset, 0 + item.itemYOffset, 0);
            model.layer = 5;
            model.transform.localScale = new Vector3(item.itemScale, item.itemScale, item.itemScale);
            if (model.GetComponent<Rigidbody>() != null) {
                model.GetComponent<Rigidbody>().useGravity = false;
            }
        }
        icon.enabled = true;
        //removeButton.interactable = true;
        

    }
    //*/

    /*
    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
        Destroy(model);
        model = null;
        //removeButton.interactable = false;
    }
    //*/
    /*
    
    public void OnRemoveButton()
    {
        //Inventory.instance.Remove(item);
    }
    //*/

    /*
    public void UseItem()
    {
        if(item != null && !tip.gameObject.activeSelf)
        {
            tip.ToggleMe(item, transform, this);
            tip.gameObject.SetActive(true);
            Debug.Log("Selected");
        }
    }
    //*/
}