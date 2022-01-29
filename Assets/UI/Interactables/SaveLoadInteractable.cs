using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class SaveLoadInteractable : Interactable
{
    public bool doSaveLoad = true;
    public UnityEvent onLoadTrue;
    public UnityEvent onLoadFalse;

    protected bool hasInteracted = false;

    private void Start() {
        if (doSaveLoad) {
            var result = UI.CompareChange(GetSaveID());
            //Debug.Log("SaveLoadInteractable Start" + gameObject.name + ": " + result);
            if (result) { //Load the item
                OnLoadTrue();
            } else {
                OnLoadFalse();
            }
        }
    }

    public override void Interact() {
        if (doSaveLoad) {
            UI.RegisterChange(GetSaveID(), hasInteracted); //Save whether the instance still exists (if it hasn't, it has been picked up)
        }
        onInteract.Invoke();
    }

    public virtual string GetSaveID() {
        return string.Format("{0}_{1}_{2}_{3}_{4}_{5}", UI.GetCurrentFile(), SceneTransitionHandler.CurrentScene(), gameObject.name, transform.position.x, transform.position.y, transform.position.z);
    }

    protected virtual void OnLoadTrue() {
        onLoadTrue.Invoke();
    }

    protected virtual void OnLoadFalse() {
        onLoadFalse.Invoke();
    }
}
