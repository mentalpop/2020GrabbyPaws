using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldableHeld : MonoBehaviour
{
    public HoldableData holdableData { get; private set; }

    public void Unpack(HoldableData _holdableData) {
        holdableData = _holdableData;
    }
}