﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavButton : MonoBehaviour
{
    public ButtonStateData buttonStateData;
    public NavButtonData navButtonData;

    public bool Available {
        get => buttonStateData.available;
        //set { }
    }
    
    public delegate void ButtonEvent (ButtonStateData _buttonStateData);
	public event ButtonEvent OnSelect = delegate { };
	public event ButtonEvent OnStateUpdate = delegate { };
	public event ButtonEvent OnUnpack = delegate { };
	public event ButtonEvent OnFocusGain = delegate { };
	public event ButtonEvent OnFocusLost = delegate { };

    public delegate void ButtonEventExt(ButtonStateData _buttonStateData, object _data);
    public event ButtonEventExt OnSelectExt = delegate { };

    public virtual void Unpack(NavButtonData _navButtonData) {
        navButtonData = _navButtonData;
        OnUnpack(buttonStateData);
    }

    public void Select() {
        OnSelect(buttonStateData);
        StateUpdate();
    }

    public void Select(object _data) {
        OnSelectExt(buttonStateData, _data);
        StateUpdate();
    }

    public void StateUpdate() {
        //Debug.Log("StateUpdate: "+gameObject.name);
        OnStateUpdate(buttonStateData);
    }

    public void SetPressed(bool _pressed) {
        buttonStateData.inputPressed = _pressed;
        StateUpdate();
    }

    public void SetFocus(bool _hasFocus) {
        //Debug.Log(gameObject.name+"; "+_hasFocus);
        buttonStateData.hasFocus = _hasFocus;
        if (_hasFocus) {
            OnFocusGain(buttonStateData);
        } else {
            OnFocusLost(buttonStateData);
            buttonStateData.inputPressed = false;
        }
        //buttonStateData.inputPressed = false;
        StateUpdate();
    }

    public void SetActive(bool _isActive) {
        buttonStateData.stateActive = _isActive;
        buttonStateData.inputPressed = false;
        StateUpdate();
    }

    public void SetAvailable(bool _isAvailable) {
        buttonStateData.available = _isAvailable;
        buttonStateData.stateActive = false;
        buttonStateData.inputPressed = false;
        StateUpdate();
    }
}

[System.Serializable]
public class ButtonStateData
{
    public bool available = true;
	public bool hasToggleState = false;
    public bool stateActive = false; //If this button has a toggled state, should it use that sprite set?
    public bool hasFocus = false;
    [HideInInspector] public bool inputPressed = false;

    public ButtonStateData() {
        this.available = true;
        this.hasToggleState = false;
        this.stateActive = false;
        this.hasFocus = false;
        this.inputPressed = false;
    }

    public ButtonStateData(bool available, bool hasToggleState, bool stateActive, bool hasFocus, bool inputPressed) {
        this.available = available;
        this.hasToggleState = hasToggleState;
        this.stateActive = stateActive;
        this.hasFocus = hasFocus;
        this.inputPressed = inputPressed;
    }
}