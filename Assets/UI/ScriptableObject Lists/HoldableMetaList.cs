using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HoldableMetaList", menuName = "Lists/HoldableMetaList", order = 1)]
public class HoldableMetaList : ScriptableObject
{
    public List<HoldableData> holdables = new List<HoldableData>();

    public int GetIndex(HoldableData holdable) {
        for (int i = 0; i < holdables.Count; i++) {
            if (holdable == holdables[i]) {
                return i;
            }
        }
        Debug.LogWarning("Could not find item in list: " + holdable.name);
        return -1;
    }

    public HoldableData GetHoldable(int index) {
        if (index <= holdables.Count)
            return holdables[index];
        Debug.LogWarning("Could not find item in list with index: " + index);
        return null;
    }
}