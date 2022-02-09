using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HoldableData", menuName = "HoldableData", order = 1)]
public class HoldableData : ScriptableObject
{
    public RigIDs rigID;
    public HoldableType holdableType;
    public GameObject heldPrefab;
    public GameObject worldPrefab;
    public Item itemData;
}

public enum RigIDs
{
    RigIDSoup
}

public enum HoldableType
{
    HTypePermanent,
    HTypeTransient,
    HTypeLocked
}