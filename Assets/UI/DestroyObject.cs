using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    public GameObject objectToDestroy;

    private void OnValidate() {
        if (objectToDestroy == null) {
            objectToDestroy = gameObject; 
        }
    }

    public void Destroy() {
        Destroy(objectToDestroy); //Most cases, simply destroy the item
    }
}