using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarkPlayerTransformReference : MonoBehaviour
{
    public DialogueSystemTrigger DSTReference;

    private void OnValidate() {
        if (DSTReference == null) {
            DSTReference = GetComponent<DialogueSystemTrigger>(); 
        }
    }

    private void Start() {
        DSTReference.barker = UI.Player.transform;
    }
}