using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavButton : MonoBehaviour
{
    public ButtonStateData buttonStateData;
    public NavButtonData navButtonData;
    
    public delegate void ButtonEvent (ButtonStateData _buttonStateData);
	public event ButtonEvent OnSelect = delegate { };
	public event ButtonEvent OnStateUpdate = delegate { };
	public event ButtonEvent OnUnpack = delegate { };
	public event ButtonEvent OnFocusGain = delegate { };
	public event ButtonEvent OnFocusLost = delegate { };

    public virtual void Unpack(NavButtonData _navButtonData) {
        navButtonData = _navButtonData;
        OnUnpack(buttonStateData);
    }

    public void Select() {
        OnSelect(buttonStateData);
        StateUpdate();
    }

    public void StateUpdate() {
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
}