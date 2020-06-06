using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DropDownOption : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
	public Image myIcon;
	public TextMeshProUGUI myTitle;
    public ThreeStateButton buttonState;
    [HideInInspector] public GameObject mySeparator;
    [HideInInspector] public DropDownMenu dropDownMenu;
    [HideInInspector] public int optionID = 0;

    private void OnEnable() {
        if (mySeparator != null) {
//Enable my Separator if I am not the last in the list
            bool lastInList = optionID == dropDownMenu.optionDataList.Count - 1;
        //If I am the 2nd last element in the list AND the chosen index IS the last element in the list; 
            bool secondLastInListAndLastIsSelected = optionID == dropDownMenu.optionDataList.Count - 2 && dropDownMenu.chosenIndex == dropDownMenu.optionDataList.Count - 1;
            if (!(lastInList || secondLastInListAndLastIsSelected))
                mySeparator.SetActive(true);
        }
    }

    private void OnDisable() {
        if (mySeparator != null)
            mySeparator.SetActive(false);
    }

    public void Unpack(LineItemData _optionData) {
		myTitle.text = _optionData.text;
        myIcon.sprite = null; //REMOVE work-around Unity 2019.3
        myIcon.sprite = _optionData.icon;
    }
		
	public void OnPointerDown(PointerEventData evd) {
		buttonState.OnPress();
	}

	public void OnPointerUp (PointerEventData evd) {
		buttonState.OnRelease();
	}

	public void OnPointerClick (PointerEventData evd) {
        dropDownMenu.Toggle(optionID);
	}
}