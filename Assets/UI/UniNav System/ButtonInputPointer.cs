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
        ButtonSetFocus(true);
    }

	public void OnPointerExit (PointerEventData evd) {
        ButtonSetFocus(false);
    }
    private void ButtonSetFocus(bool _focus) {
        if (MenuNavigator.Instance.useMouse) {
            navButton.buttonStateData.hasFocus = _focus;
            navButton.StateUpdate();
        }
    }
		
	public void OnPointerClick (PointerEventData evd) {
        if (MenuNavigator.Instance.useMouse) {
            if (navButton.buttonStateData.hasToggleState)
                navButton.buttonStateData.stateActive = !navButton.buttonStateData.stateActive;
            navButton.Select();
        }
	}

	public void OnPointerDown(PointerEventData eventData) {
        ButtonSetPressed(true);
    }

    public void OnPointerUp(PointerEventData eventData) {
        ButtonSetPressed(false);
    }

    private void ButtonSetPressed(bool _pressed) {
        if (MenuNavigator.Instance.useMouse) {
            navButton.buttonStateData.inputPressed = _pressed;
            navButton.StateUpdate();
        }
    }
}