using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoupHolder : HoldableHeld
{
    public int soupUses = 4;
    public List<GameObject> soups = new List<GameObject>();
    public List<HoldableData> holdableDatas = new List<HoldableData>();

    private void Start() {
        ModelUpdate();
    }

    public override void Use() {
        soupUses--;
        ModelUpdate();
    }

    public void ModelUpdate() {
        switch (soupUses) {
            case 0: Inventory.instance.HoldableClear(); break;
            default:
                foreach (var soup in soups) { //Turn off all Soup models
                    soup.SetActive(false);
                }
                soups[Mathf.Min(soupUses, soups.Count - 1)].SetActive(true); //Except the one that has the correct count
                holdableData = holdableDatas[soupUses];
                break;
        }
    }
}