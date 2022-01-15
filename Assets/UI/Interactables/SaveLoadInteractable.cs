using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadInteractable : Interactable
{
    public bool doSaveLoad = true;

    protected bool hasBeenCollected = false;

    private void Start() {
        var result = Inventory.CompareChange(GetSaveID());
        Debug.Log("SaveLoadInteractable Start" + gameObject.name + ": " + result);
        if (result) { //Load the item
            OnLoadTrue();
        } else {
            OnLoadFalse();
        }
    }

    public override void Interact() {
        if (doSaveLoad) {
            Inventory.RegisterChange(GetSaveID(), hasBeenCollected); //Save whether the instance still exists (if it hasn't, it has been picked up)
        }
    }

    public virtual string GetSaveID() {
        return string.Format("{0}_{1}_{2}_{3}_{4}_{5}", UI.GetCurrentFile(), SceneTransitionHandler.CurrentScene(), gameObject.name, transform.position.x, transform.position.y, transform.position.z);
    }

    protected virtual void OnLoadTrue() {
        Destroy(gameObject); //Most cases, simply destroy the item
    }

    protected virtual void OnLoadFalse() {

    }
}
