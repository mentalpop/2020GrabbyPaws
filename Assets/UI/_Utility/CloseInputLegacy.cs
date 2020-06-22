using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseInputLegacy : MonoBehaviour
{
    public ClickToClose clickToClose;
    public string inputClose;

    private void Update() {
        if (Input.GetButtonUp(inputClose)) {
            clickToClose.ManualClose();
        }
    }
}