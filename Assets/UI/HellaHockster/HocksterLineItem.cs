using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class HocksterLineItem : ButtonFormatterSevenState, IPointerClickHandler
{
    public ItemTooltip iTooltip;
    public RectTransform ttRect;
    public Vector2 ttOffset;
    public GameObject arrow;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemQuantity;
    public Color colorNeutral;
    public Color colorMOver;
    public string singleItemInput;
    public string stackInput;

    private HocksterScrollRect hocksterScrollRect;
    private InventoryItem iItem;
    private List<InventoryItem> myList;
    private List<InventoryItem> otherList;
    private HellaHockster hellaHockster;
    private bool inputStackDown = false;
    private bool inputSingleDown = false;

    private void OnEnable() {
        navButton.OnStateUpdate += ButtonFormat;
    }

    private void OnDisable() {
        navButton.OnStateUpdate -= ButtonFormat;
        Destroy(iTooltip.gameObject);
    }

    protected override void ButtonFormat(ButtonStateData _buttonStateData) {
        base.ButtonFormat(_buttonStateData);
        switch (buttonState) {
            case BState.Neutral:
                HandleTooltip(false);
                arrow.SetActive(false);
                itemName.color = colorNeutral;
                itemQuantity.color = colorNeutral;
                break;
            case BState.Focus:
                arrow.SetActive(true);
                itemName.color = colorMOver;
                itemQuantity.color = colorMOver;
            //Tooltip
                HandleTooltip(true);
                break;
            /*
            case BState.Selected:
                break;
            case BState.Active: ; break;
            case BState.ActiveFocus: ; break;
            case BState.ActiveSelected: ; break;
            case BState.Unavailable: ; break;
            //*/
        }
    }

    public void Unpack(InventoryItem _item, List<InventoryItem> _myList, List<InventoryItem> _otherList, HellaHockster _hellaHockster, HocksterScrollRect _hocksterScrollRect) {
        //iTooltip = Inventory.GetItemTooltip();
        iItem = _item;
        myList = _myList;
        otherList = _otherList;
        hellaHockster = _hellaHockster;
        itemName.text = iItem.item.name;
        itemQuantity.text = iItem.quantity.ToString();
        hocksterScrollRect = _hocksterScrollRect;
    }

    public void HandleTooltip(bool doShow) {
        if (doShow) {
            iTooltip.gameObject.SetActive(true);
            iTooltip.Unpack(iItem);
        } else {
            iTooltip.gameObject.SetActive(false);
        }
    }

    public void MoveItem(bool _moveStack) {
        if (_moveStack) {
    //Move entiire stack
            hellaHockster.MoveItem(myList, otherList, iItem.item, iItem.quantity);
        } else {
    //Move one item at a time
            hellaHockster.MoveItem(myList, otherList, iItem.item, 1);
        }
        hocksterScrollRect.listController.Focus();
        HandleTooltip(false);
        //hocksterScrollRect.listController.DefineIndex();
    }

    private void Update() {
//Listen for Legacy Input
        if (navButton.buttonStateData.hasFocus) {
            if (Input.GetButtonUp(stackInput) && inputStackDown) {
                MoveItem(true);
                inputStackDown = false;
            }
            if (Input.GetButtonDown(stackInput) && !inputStackDown) {
                inputStackDown = true;
            }
            if (Input.GetButtonUp(singleItemInput) && inputSingleDown) {
                MoveItem(false);
                inputSingleDown = false;
            }
            if (Input.GetButtonDown(singleItemInput) && !inputSingleDown) {
                inputSingleDown = true;
            }
        //Reposition the tooltip every frame because it randomly changes position for no reason
            iTooltip.transform.parent = transform;
            ttRect.anchoredPosition = ttOffset;
            iTooltip.transform.parent = hocksterScrollRect.transform.parent;
        }
    }

    public void OnPointerClick (PointerEventData evd) {
        if (evd.button == PointerEventData.InputButton.Right) {
            MoveItem(false);
        } else {
            MoveItem(true);
        }
	}
}