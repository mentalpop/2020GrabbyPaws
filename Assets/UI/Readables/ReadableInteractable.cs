using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadableInteractable : Interactable
{
    public ReadableDataParent readableData;
    public ReadableTypes Type;

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

    public override void Interact() {
        switch (Type) {
            case ReadableTypes.Book: UI.Instance.DisplayBook(readableData as ReadableData); break;
            case ReadableTypes.Sign: UI.Instance.DisplaySign(readableData as ReadableData); break;
            case ReadableTypes.Simple: UI.Instance.DisplaySimple(readableData as ReadableData); break;
            case ReadableTypes.PC: UI.Instance.DisplayPC(readableData as ReadablePCData); break;
        }
    }
}