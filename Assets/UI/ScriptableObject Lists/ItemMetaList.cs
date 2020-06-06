using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemMetaList", menuName = "Lists/ItemMetaList", order = 1)]
public class ItemMetaList : ScriptableObject
{
    public List<Item> items = new List<Item>();

    public int GetIndex(Item item) {
        for (int i = 0; i < items.Count; i++) {
            if (item == items[i]) {
                return i;
            }
        }
        Debug.LogWarning("Could not find item in list: "+item.name);
        return -1;
    }

    public Item GetItem(int index) {
        if (index <= items.Count)
            return items[index];
        Debug.LogWarning("Could not find item in list with index: "+index);
        return null;
    }
}