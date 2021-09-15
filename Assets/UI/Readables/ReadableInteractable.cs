using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class ReadableInteractable : Interactable
{
    public ReadableDataParent readableData;
    public ReadableTypes Type;
    public Usable usable;

    private bool subbedEvent = false;
    private Readable activeReadable;

    public enum ReadableTypes
    {
        Book,
        Sign,
        PC,
        Simple
    }

    private void Awake() {
        gameObject.AddComponent<cakeslice.Outline>();
    }

    private void OnValidate() {
        if (usable == null) {
            usable = GetComponent<Usable>();
        }
    }

    private void OnDisable() {
        if (subbedEvent) {
            activeReadable.OnReadableClose -= ActiveReadable_OnReadableClose;
        }
    }

    public override void Interact() {
        if (subbedEvent) {
            activeReadable.OnReadableClose -= ActiveReadable_OnReadableClose;
        }
        switch (Type) {
            case ReadableTypes.Book: activeReadable = UI.Instance.DisplayBook(readableData as ReadableData); break;
            case ReadableTypes.Sign: activeReadable = UI.Instance.DisplaySign(readableData as ReadableData); break;
            case ReadableTypes.Simple: activeReadable = UI.Instance.DisplaySimple(readableData as ReadableData); break;
            case ReadableTypes.PC: UI.Instance.DisplayPC(readableData as ReadablePCData); break;
        }
    //Subscribe to the Close event on the Readable
        if (activeReadable != null) {
            activeReadable.OnReadableClose += ActiveReadable_OnReadableClose;
            subbedEvent = true;
        }
    }

    //Hook into a Usable and trigger its Deselect
    private void ActiveReadable_OnReadableClose(ReadableData data) {
        if (usable != null) {
            usable.OnDeselectUsable();
        }
    }
}