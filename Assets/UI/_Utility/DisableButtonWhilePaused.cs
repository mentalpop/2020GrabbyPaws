using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableButtonWhilePaused : MonoBehaviour
{
    public UnityEngine.UI.Button disableThisButton;
    
    private void OnEnable() {
        UI.Instance.OnPauseStateChange += Instance_OnPauseStateChange;
    }

    private void OnDisable() {
        UI.Instance.OnPauseStateChange -= Instance_OnPauseStateChange;
    }

    private void Instance_OnPauseStateChange(bool controlsLocked) {
        disableThisButton.interactable = !controlsLocked;
    }
}