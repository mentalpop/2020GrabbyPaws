using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ButtonInputPointer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public NavButton navButton;
		
	public void OnPointerEnter(PointerEventData evd) {
        navButton.buttonStateData.hasFocus = true;
        navButton.StateUpdate();
    }

	public void OnPointerExit (PointerEventData evd) {
        navButton.buttonStateData.hasFocus = false;
        navButton.StateUpdate();
    }
		
	public void OnPointerClick (PointerEventData evd) {
        if (navButton.buttonStateData.hasToggleState)
            navButton.buttonStateData.stateActive = !navButton.buttonStateData.stateActive;
        navButton.Select();
	}

	public void OnPointerDown(PointerEventData eventData) {
        navButton.buttonStateData.inputPressed = true;
        navButton.StateUpdate();
    }

    public void OnPointerUp(PointerEventData eventData) {
        navButton.buttonStateData.inputPressed = false;
        navButton.StateUpdate();
    }
}