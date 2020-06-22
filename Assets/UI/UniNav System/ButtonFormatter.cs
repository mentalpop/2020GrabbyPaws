using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFormatter : MonoBehaviour
{
    public NavButton navButton;
    public bool allowSelectionWhileActive = false;

    private void OnEnable() {
        navButton.OnStateUpdate += ButtonFormat;
        navButton.OnUnpack += Unpack;
    }

    private void OnDisable() {
        navButton.OnStateUpdate -= ButtonFormat;
        navButton.OnUnpack -= Unpack;
    }

    protected virtual void Unpack(ButtonStateData _buttonStateData) {
        
    }

    protected virtual void ButtonFormat(ButtonStateData _buttonStateData) {
        
    }
}