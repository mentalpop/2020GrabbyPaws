using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsTabHandler : MonoBehaviour
{
    public NavButton tabMisc;
    public NavButton tabControls;

    public GameObject tabMiscContents;
    public GameObject tabControlsContents;

    public delegate void TabSelectEvent(bool controlsTabActive);
    public event TabSelectEvent OnTabSelected = delegate { }; //Whether or not this tab is active

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
        OnTabSelected(true);
    }

    private void TabMisc_OnSelect(ButtonStateData _buttonStateData) {
        SetActiveMisc();
        OnTabSelected(false);
    }

    private void TabControls_OnFocusGain(ButtonStateData _buttonStateData) {
        if (!MenuNavigator.MouseIsUsing()) {
            SetActiveControls();
            OnTabSelected(true);
        }
    }

    private void TabMisc_OnFocusGain(ButtonStateData _buttonStateData) {
        if (!MenuNavigator.MouseIsUsing()) {
            SetActiveMisc();
            OnTabSelected(false);
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