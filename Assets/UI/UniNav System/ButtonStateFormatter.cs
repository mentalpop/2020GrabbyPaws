﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonStateFormatter : MonoBehaviour
{
    public NavButton navButton;
    public bool allowSelectionWhileActive = false;

    private void OnEnable() {
        navButton.OnStateUpdate += ButtonFormat;
    }

    private void OnDisable() {
        navButton.OnStateUpdate -= ButtonFormat;
    }

    protected virtual void ButtonFormat(ButtonStateData _buttonStateData) {
        
    }
}