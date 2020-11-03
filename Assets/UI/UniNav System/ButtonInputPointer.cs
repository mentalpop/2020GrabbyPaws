using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ButtonInputPointer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public NavButton navButton;

    /*
    private MenuNavigator menuNavigator;

    private void Awake() {
        menuNavigator = MenuNavigator.Instance;
    }
    //*/

    public void OnPointerEnter(PointerEventData evd) {
        ButtonSetFocus(true);
    }

	public void OnPointerExit (PointerEventData evd) {
        ButtonSetFocus(false);
    }
    private void ButtonSetFocus(bool _focus) {
        if (MenuNavigator.MouseIsUsing()) {
            navButton.SetFocus(_focus);
            /*
            navButton.buttonStateData.hasFocus = _focus;
            navButton.StateUpdate();
            //*/
        }
    }
		
	public void OnPointerClick (PointerEventData evd) {
        if (MenuNavigator.MouseIsUsing()) {
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
        if (MenuNavigator.MouseIsUsing()) {
            navButton.SetPressed(_pressed);
            /*
            navButton.buttonStateData.inputPressed = _pressed;
            navButton.StateUpdate();
            //*/
        }
    }
}