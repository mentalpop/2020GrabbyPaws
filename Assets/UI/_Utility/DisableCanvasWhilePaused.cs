using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableCanvasWhilePaused : MonoBehaviour
{
    public Canvas disableThisCanvas;

    private void OnEnable() {
        UI.Instance.OnPauseStateChange += Instance_OnPauseStateChange;
    }

    private void OnDisable() {
        UI.Instance.OnPauseStateChange -= Instance_OnPauseStateChange;
    }

    private void Instance_OnPauseStateChange(bool controlsLocked) {
        disableThisCanvas.enabled = !controlsLocked;
    }
}