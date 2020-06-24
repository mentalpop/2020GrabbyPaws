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

    public void SetFocus(bool _hasFocus) {
        //Debug.Log("_hasFocus: "+gameObject.name);
        buttonStateData.hasFocus = _hasFocus;
        buttonStateData.inputPressed = false;
        StateUpdate();
    }

    public void SetActive(bool _isActive) {
        buttonStateData.stateActive = _isActive;
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