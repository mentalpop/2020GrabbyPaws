using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonDataUnpacker : MonoBehaviour
{
    public NavButton navButton;

    private void OnEnable() {
        navButton.OnUnpack += Unpack;
    }

    private void OnDisable() {
        navButton.OnUnpack -= Unpack;
    }

    protected virtual void Unpack(ButtonStateData _buttonStateData) {
        
    }
}