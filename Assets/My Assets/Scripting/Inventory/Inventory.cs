using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class Inventory : MonoBehaviour//Singleton<Inventory>//, IFileIO<List<int>>
{
    public ItemMetaList itemMetaList;
    public InventoryItem nullItem;
    public GadgetList gadgetList;
    public Vector3 dropPosition;
    public ItemTooltip itemTooltip;
    public GameObject pickupSphere;
    public List<InventoryItem> items = new List<InventoryItem>();
    //public UI uiRef;
    [HideInInspector] public List<bool> gadgetUnlocked = new List<bool>();

    private string saveStringItemIDs = "itemID";
    private string saveStringItemCount = "itemQuantity";
    private string saveStringGadgets = "gadgets";

    public delegate void InventoryEvent();
    public InventoryEvent OnItemChanged;
    public InventoryEvent OnPickUp;
    public InventoryEvent OnDrop;

    public static Inventory instance;

    private void Awake() {
    //Singleton Pattern
        if (instance != null && instance != this) { 
            Destroy(gameObject);
            return;
        }
        instance = this;
        Debug.Log("instance: "+instance);
        DontDestroyOnLoad(gameObject);
//Set all Gadgets as locked initially
        for (int i = 0; i < gadgetList.gadgets.Count; i++) {
            gadgetUnlocked.Add(false);
        }
    }

    private void OnEnable() {
        UI.Instance.OnSave += Save;
        UI.Instance.OnLoad += Load;
        RegisterLuaFunctions();
    }

    private void OnDisable() {
        UI.Instance.OnSave -= Save;
        UI.Instance.OnLoad -= Load;
    }
    #region Lua Functions
    private void RegisterLuaFunctions() {
        //Debug.Log("Inventory RegisterLuaFunctions");
        Lua.RegisterFunction("InventoryHas", this, SymbolExtensions.GetMethodInfo(() => InventoryHas(string.Empty)));
        Lua.RegisterFunction("InventoryCount", this, SymbolExtensions.GetMethodInfo(() => InventoryCount(string.Empty)));
        Lua.RegisterFunction("InventoryAdd", this, SymbolExtensions.GetMethodInfo(() => InventoryAdd(string.Empty, 0f)));
        Lua.RegisterFunction("InventoryRemove", this, SymbolExtensions.GetMethodInfo(() => InventorySubtract(string.Empty, 0f)));
        Lua.RegisterFunction("InventoryRemoveAllOf", this, SymbolExtensions.GetMethodInfo(() => InventoryRemove(string.Empty)));
    }

    public bool InventoryHas(string name) {
        Debug.Log("Looking for a: " + name);
        foreach (var item in items) { //Ensure the item exists in the inventory
            //Debug.Log("Comparing against: " + item.item.name);
            if (item.item.ID == name) {
                Debug.Log("Found it");
                return true;
            }
        }
        Debug.Log("Did not find it");
        return false;
    }

    public double InventoryCount(string name) {
        int count = 0;
        //Debug.Log("Counting: " + name);
        foreach (var item in items) { //Count occurrences of the item in the inventory
            //Debug.Log("Comparing against: " + item.item.name);
            if (item.item.ID == name) {
                count = item.quantity;
                break;
            }
        }
        //Debug.Log("Found: " + name);
        return count;
    }

    public void InventoryAdd(string name, double quantity) {
        Debug.Log("Attempting to add to Inventory: " + name);
        bool _success = false;
        foreach (var item in itemMetaList.items) { //Find the Item in the Meta list based on String reference, add X of it to the inventory
            if (item.ID == name) {
                //items.Add(new InventoryItem(item, quantity));
                Add(item, (int)quantity);
                Debug.Log("Successfully added to Inventory: " + item.ID);
                _success = true;
                break;
            }
        }
        if (!_success) {
            Debug.LogWarning("Failed to add \""+name+ "\" to Inventory, are you sure it's in the ItemMetaList?");
        }
        OnItemChanged?.Invoke();
    }

    public void InventorySubtract(string name, double _quantity) {
        int quantity = (int)_quantity;
        while (quantity > 0) {
            foreach (var item in items) { //Try to remove an item if it exists in the inventory
                if (item.item.ID == name) {
                    while (quantity > 0) {
                        if (item.quantity > 0)
                            Remove(item.item);
                        else
                            Debug.LogWarning("Trying to remove more of an item from the Inventory than exists in the inventory");
                        quantity--;
                    }
                    break;
                }
            }
            quantity--;
        }
        OnItemChanged?.Invoke();
    }

    public void InventoryRemove(string name) {
//Remove all occurrences of an item from the inventory, good if you don't want to be specific
        foreach (var item in items) { 
            if (item.item.ID == name) {
                RemoveAll(item.item);
                break;
            }
        }
        //items.RemoveAll(item => item.item.name == name); //Remove everything that matches the name
        OnItemChanged?.Invoke();
    }
    
    #endregion
    public void Save(int fileIndex) {
        ES3.Save<List<bool>>(saveStringGadgets, gadgetUnlocked);
        List<int> itemIDs = new List<int>();
        List<int> itemCount = new List<int>();
        foreach (var item in items) {
            itemIDs.Add(itemMetaList.GetIndex(item.item));
            itemCount.Add(item.quantity);
        }
        ES3.Save<List<int>>(saveStringItemIDs, itemIDs);
        ES3.Save<List<int>>(saveStringItemCount, itemCount);
        //List<bool> _gadgetsUnlocked = new List<bool>();
    }

    public void Load(int fileIndex) {
        gadgetUnlocked = ES3.Load(saveStringGadgets, new List<bool>());
        List<int> loadItems = ES3.Load(saveStringItemIDs, new List<int>());
        List<int> loadCount = ES3.Load(saveStringItemCount, new List<int>());
        items.Clear();
        for (int i = 0; i < loadItems.Count; i++) {
            items.Add(new InventoryItem(itemMetaList.GetItem(loadItems[i]), loadCount[i]));
        }
        /*
        foreach (var item in loadItems) {
            items.Add(new InventoryItem(itemMetaList.GetItem(item.itemID), item.quantity));
        }
        //*/
    }

    public bool Add(Item item) {
//If you don't specify a quantity, add 1
        Add(item, 1);
        return true;
    }

    public bool Add(Item item, int quantity) {
//Check if the item is already in the inventory
        bool foundInInventory = false;
        foreach (var iItem in items) {
            if (iItem.item == item) {
        //Add to it's quantity
                iItem.quantity += quantity;
                foundInInventory = true;
            }
        }
        if (!foundInInventory) {
    //if there are none, add to the list instead
            items.Add(new InventoryItem(item, quantity));
        }
        OnItemChanged?.Invoke();
        return true;
    }

    public void Remove(Item item) {
        Remove(item, 1);
    }

    public void Remove(Item item, int quantity) {
//Check if the item is already in the inventory
        foreach (var iItem in items) {
            if (iItem.item == item) {
        //Remove one from it's quantity
                iItem.quantity -= quantity;
                if (iItem.quantity < 1) {
            //If you have none left, actually remove the item
                    items.Remove(iItem);
                }
                break;
            }
        }
        OnItemChanged?.Invoke();
    }

    public void RemoveAll(Item item) {
//Check if the item is already in the inventory
        foreach (var iItem in items) {
            if (iItem.item == item) {
        //Remove all instances of it
                items.Remove(iItem);
                break;
            }
        }
        OnItemChanged?.Invoke();
    }

    public GameObject Drop(Item _toDrop) {
        GameObject toDrop = null;
//Drop an item from the Inventory
        foreach (var item in items) { //Ensure the item exists in the inventory
            if (item.item == _toDrop) {
                if (item.item.physicalItem != null) {
                    toDrop = Instantiate(item.item.physicalItem, dropPosition, Quaternion.identity);
                    Instantiate(pickupSphere, dropPosition, Quaternion.identity); //Drop a sphere where the item was dropped
                    OnDrop();
                }
                Remove(item.item);
                break;
            }
        }
        return toDrop;
    }

    public static ItemTooltip GetItemTooltip() {
        return instance.itemTooltip;
    }


    /* This is a disaster, removing it
    public int ReturnValues() {

        ThieveControl controller = FindObjectOfType<ThieveControl>();
        int myValue = 0;

        foreach (var item in items)
        {
            myValue += item.value;
            controller.collectedItems.Add(item);
        }
        items.Clear();
        return myValue;
    }
    //*/

    public float ReturnWeights() {
        float weight = 0;
        for(int i = 0; i < items.Count; i++) {
            weight += items[i].item.weight * items[i].quantity; //Return item multiplied by weight to account for Quantity
        }
        return weight;
    }
}

[System.Serializable]
public class InventoryItem
{
    public Item item;
    public int quantity;

    public InventoryItem(Item item, int quantity) {
        this.item = item;
        this.quantity = quantity;
    }
}

/*
[System.Serializable]
public class IItemID
{
    public int itemID;
    public int quantity;

    public IItemID(int itemID, int quantity) {
        this.itemID = itemID;
        this.quantity = quantity;
    }
}
//*/