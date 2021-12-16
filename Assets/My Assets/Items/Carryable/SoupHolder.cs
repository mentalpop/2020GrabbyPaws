using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoupHolder : MonoBehaviour
{
    public string soupID;
    public List<GameObject> soups = new List<GameObject>();

    private Inventory inventory;

    private void OnEnable() {
        inventory = Inventory.instance;
        inventory.OnItemChanged += inventory_OnItemChanged;
    }

    private void OnDisable() {
        inventory.OnItemChanged -= inventory_OnItemChanged;
    }

    private void inventory_OnItemChanged(Item item) {
        int _soups = (int)inventory.InventoryCount(soupID);
        switch(_soups) {
            case 0: Destroy(gameObject); break;
            default:
                foreach (var soup in soups) { //Turn off all Soup models
                    soup.SetActive(false);
                }
                soups[Mathf.Min(_soups, soups.Count - 1)].SetActive(true); //Except the one that has the correct count
                break;
        }
    }
}