using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using cakeslice;

public class OutlineUsableLink : MonoBehaviour
{
    public Usable Usable;
    public Outline outline;
    public bool isChildPrefab = false;
    private ProximitySelector proximitySelector;
    private bool subbedEvents = false;

    private void Start() {
        proximitySelector = UI.Instance.player.proximitySelector;//FindObjectOfType<ProximitySelector>();
        if (proximitySelector == null) {
            Debug.LogWarning("Failed to find proximitySelector on player");
        } else {
            proximitySelector.OnUsableGainFocus += ProximitySelector_OnUsableGainFocus;
            proximitySelector.OnUsableLoseFocus += ProximitySelector_OnUsableLoseFocus;
            subbedEvents = true;
        }
        if (outline == null) { //If the outline is null, try to GetComponent, if it is still null, Add it as a Component
            outline = gameObject.GetComponent<Outline>();
            if (outline == null) {
                GameObject targetGameObject = isChildPrefab ? transform.parent.gameObject : gameObject;
                if (targetGameObject.GetComponent<Renderer>()) { //The Renderer Component is required
                    targetGameObject.AddComponent<Outline>();
                } else {
                    Debug.LogError("Failed to automatically add an Outline to " + targetGameObject.name + "because it doesn't have a Renderer Component. Please add an Outline Component to the correct Child object and then specify a reference to it on the OutlineUsableLink Component");
                }
                //if (isChildPrefab) {
                //    outline = transform.parent.gameObject.AddComponent<Outline>();
                //} else {
                //    outline = gameObject.AddComponent<Outline>();
                //}
            }
        }
        outline.enabled = false;
    }

    void OnEnable() {
        if (!subbedEvents && proximitySelector != null) { //Prevent redundant subscription
            proximitySelector.OnUsableGainFocus += ProximitySelector_OnUsableGainFocus;
            proximitySelector.OnUsableLoseFocus += ProximitySelector_OnUsableLoseFocus;
            subbedEvents = true;
        }
    }
    private void OnDisable() {
        if (subbedEvents && proximitySelector != null) {
            proximitySelector.OnUsableGainFocus -= ProximitySelector_OnUsableGainFocus;
            proximitySelector.OnUsableLoseFocus -= ProximitySelector_OnUsableLoseFocus;
            subbedEvents = false;
        }
    }

    private void ProximitySelector_OnUsableLoseFocus(Usable usable) {
        if (Usable == usable) {
            outline.enabled = false;
        }
    }

    private void ProximitySelector_OnUsableGainFocus(Usable usable) {
        if (Usable == usable) {
            outline.enabled = true;
        }
    }
}