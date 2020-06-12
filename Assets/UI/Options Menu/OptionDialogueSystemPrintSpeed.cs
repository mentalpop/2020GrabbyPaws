using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class OptionDialogueSystemPrintSpeed : MonoBehaviour
{
    public AbstractTypewriterEffect abstractTypewriterEffect;

    private float printSpeedInitial;
    private UI UIRef;

    private void Awake() {
        UIRef = UI.Instance;
        printSpeedInitial = abstractTypewriterEffect.GetSpeed();
    }

    private void OnEnable() {
        UIRef.OnPrintSpeedSet += UIRef_OnPrintSpeedSet;
        UIRef_OnPrintSpeedSet(UI.GetPrintSpeed());
    }

    private void OnDisable() {
        UIRef.OnPrintSpeedSet -= UIRef_OnPrintSpeedSet;
    }

    private void UIRef_OnPrintSpeedSet(float speed) {
        abstractTypewriterEffect.SetSpeed(printSpeedInitial * speed);
    }
}