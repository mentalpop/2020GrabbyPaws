using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropDownHeader : MonoBehaviour, IPointerClickHandler//, IPointerDownHandler, IPointerUpHandler
{
	public DropDownMenu dropDownMenu;
    /*
	public ThreeStateButton buttonState;
		
	public void OnPointerDown(PointerEventData evd) {
		buttonState.OnPress();
	}

	public void OnPointerUp (PointerEventData evd) {
		buttonState.OnRelease();
	}
	//*/

	public void OnPointerClick(PointerEventData eventData) {
		dropDownMenu.Toggle(-1);
		//buttonState.SetActiveState(true);
	}
}