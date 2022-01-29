using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent onInteract;

    public virtual void Interact() {
        //This method is meant to be overwritten
    }
}
