using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsTabHandler : MonoBehaviour
{
    public NavButton tabMisc;
    public NavButton tabControls;

    public GameObject tabMiscContents;
    public GameObject tabControlsContents;

    private void OnEnable() {
        tabMisc.OnSelect += TabMisc_OnSelect;
        tabMisc.OnFocusGain += TabMisc_OnFocusGain;
        tabControls.OnSelect += TabControls_OnSelect;
        tabControls.OnFocusGain += TabControls_OnFocusGain;
    }

    private void OnDisable() {
        tabMisc.OnSelect -= TabMisc_OnSelect;
        tabMisc.OnFocusGain -= TabMisc_OnFocusGain;
        tabControls.OnSelect -= TabControls_OnSelect;
        tabControls.OnFocusGain -= TabControls_OnFocusGain;
    }

    private void TabControls_OnSelect(ButtonStateData _buttonStateData) {
        SetActiveControls();
    }

    private void TabMisc_OnSelect(ButtonStateData _buttonStateData) {
        SetActiveMisc();
    }

    private void TabControls_OnFocusGain(ButtonStateData _buttonStateData) {
        if (!MenuNavigator.MouseIsUsing()) {
            SetActiveControls();
        }
    }

    private void TabMisc_OnFocusGain(ButtonStateData _buttonStateData) {
        if (!MenuNavigator.MouseIsUsing()) {
            SetActiveMisc();
        }
    }

    private void SetActiveMisc() {
        tabMiscContents.SetActive(true);
        tabControlsContents.SetActive(false);
    }

    private void SetActiveControls() {
        tabMiscContents.SetActive(false);
        tabControlsContents.SetActive(true);
    }
}