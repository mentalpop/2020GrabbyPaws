using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsTabHandler : MonoBehaviour
{
    public ButtonGeneric tabMisc;
    public ButtonGeneric tabControls;

    public GameObject tabMiscContents;
    public GameObject tabControlsContents;

    private void OnEnable() {
        tabMisc.OnClick += tabMisc_OnClick;
        tabControls.OnClick += tabControls_OnClick;
    }

    private void OnDisable() {
        tabMisc.OnClick -= tabMisc_OnClick;
        tabControls.OnClick -= tabControls_OnClick;
    }

    public void tabMisc_OnClick() {
        tabMiscContents.SetActive(true);
        tabControlsContents.SetActive(false);
    }
    
    public void tabControls_OnClick() {
        tabMiscContents.SetActive(false);
        tabControlsContents.SetActive(true);
    }
}