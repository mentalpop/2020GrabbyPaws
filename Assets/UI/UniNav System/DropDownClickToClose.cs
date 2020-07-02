using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropDownClickToClose : MonoBehaviour
{
    public ListControllerDropDown ddm;
    public ClickToClose clickToClose;

    private bool isActive = false;

    private void OnEnable() {
        ddm.OnSelect += Ddm_OnSelect;
        ddm.OnOpen += Ddm_OnOpen;
    }

    private void Ddm_OnOpen() {
        Activate();
    }

    private void OnDisable() {
        Deactivate();
        ddm.OnSelect -= Ddm_OnSelect;
        ddm.OnOpen -= Ddm_OnOpen;
    }
    private void Activate() {
        if (!isActive && MenuNavigator.Instance.useMouse) {
            clickToClose.gameObject.SetActive(true);
            clickToClose.OnClick += Close;
            isActive = true;
        }
    }

    private void Deactivate() {
        if (isActive) {
            clickToClose.gameObject.SetActive(false);
            clickToClose.OnClick -= Close;
            isActive = false;
        }
    }

    private void Close() {
        ddm.Unfocus();
        Deactivate();
    }

    private void Ddm_OnSelect(int index) {
        Deactivate();
    }
}