using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class SoupHolder : HoldableHeld
{
    public string soupID;
    public List<GameObject> soups = new List<GameObject>();

    private Rig PlayersRig;
    private Inventory inventory;

    private void Awake()
    {
        PlayersRig = FindObjectOfType<Rig>();
    }

    private void Start() {
        PlayersRig.weight = 1.0f;
    }

    private void OnEnable()
    {
        inventory = Inventory.instance;
        inventory.OnItemChanged += inventory_OnItemChanged;
    }

    private void OnDisable()
    {
        inventory.OnItemChanged -= inventory_OnItemChanged;
    }


    private void inventory_OnItemChanged(Item item) {
        int _soups = (int)inventory.InventoryCount(soupID);
        switch (_soups) {
            case 0: 
                PlayersRig.weight = 0.0f;
                Destroy(gameObject);
                break;
            default:
                PlayersRig.weight = 1.0f;
                foreach (var soup in soups) { //Turn off all Soup models
                    soup.SetActive(false);
                }
                soups[Mathf.Min(_soups, soups.Count - 1)].SetActive(true); //Except the one that has the correct count
                break;
        }
    }
}