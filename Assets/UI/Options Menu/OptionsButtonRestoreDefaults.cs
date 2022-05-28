using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsButtonRestoreDefaults : MonoBehaviour
{
    public NavButton button;

//Confirmation Window
	public MenuNode menuOnDisable;
	public ConfirmationPromptData promptData;
	private ConfirmationWindow confirmationWindow;
	private bool awaitingConfirmation = false;

    private void OnEnable() {
        button.OnSelect += button_OnSelect;
        button.OnSelectExt += NavButton_OnSelectExt;
    }

    private void OnDisable() {
        button.OnSelect -= button_OnSelect;
        button.OnSelectExt -= NavButton_OnSelectExt;
        if (awaitingConfirmation) {
			awaitingConfirmation = false;
			confirmationWindow.OnChoiceMade -= OnConfirm;
		}
    }

    private void button_OnSelect(ButtonStateData _buttonStateData) {
        if (!awaitingConfirmation) {
            confirmationWindow = UI.RequestConfirmation(promptData, menuOnDisable);
		    confirmationWindow.OnChoiceMade += OnConfirm;
		    awaitingConfirmation = true;
        }
    }

    private void NavButton_OnSelectExt(ButtonStateData _buttonStateData, object _data) {
        button_OnSelect(_buttonStateData);
    }

    private void OnConfirm(bool result) {
        awaitingConfirmation = false;
		confirmationWindow.OnChoiceMade -= OnConfirm;
		if (result) {
            UI.Instance.RestoreAllDefaults();
        }
    }
}