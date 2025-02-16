﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.SceneManagement;
using System.Linq;

public class Inventory : MonoBehaviour//Singleton<Inventory>//, IFileIO<List<int>>
{
    public ItemMetaList itemMetaList;
    public HoldableMetaList holdableMetaList;
    public InventoryItem nullItem;
    public GadgetList gadgetList;
    //public Vector3 dropPosition;
    public ItemTooltip itemTooltip;
    public SceneTransitionHandler sceneTransitionHandler;
    public GameObject pickupSphere;
    public GameObject prefabDroppedItemInteractable;
    public List<InventoryItem> items = new List<InventoryItem>();
    //public UI uiRef;
    [HideInInspector] public List<bool> gadgetUnlocked = new List<bool>();

    private string saveStringItemIDs = "itemID";
    private string saveStringItemCount = "itemQuantity";
    private string saveStringGadgets = "gadgets";
    private string saveStringHeldItem = "heldItem";
    private string saveStringHoldableIDs = "holdableIDs";

    public delegate void InventoryEvent(Item item);
    public InventoryEvent OnPickUp;
    //public InventoryEvent OnDrop;
    public InventoryEvent OnItemChanged;
    public InventoryEvent OnItemGiven;
    public Dictionary<HoldableID, HoldableRegistered> registeredHoldables = new Dictionary<HoldableID, HoldableRegistered>();

    public delegate void HoldableEvent(HoldableData holdableData);
    public HoldableEvent OnDrop;

    public delegate void InventoryUtilityEvent();
    public event InventoryUtilityEvent OnPreSave = delegate { };

    public static Inventory instance;
    private HoldableHeld RigHeld => UI.Player == null ? null : UI.Player.rigManager.HoldableHeld;
    private int heldBetweenScenesIndex = -1;
    private bool hasSubscribedToFallbackMethod = false;

    private void Awake() {
    //Singleton Pattern
        if (instance != null && instance != this) { 
            Destroy(gameObject);
            return;
        }
        instance = this;
        //Debug.Log("instance: "+instance);
        DontDestroyOnLoad(gameObject);
//Set all Gadgets as locked initially
        for (int i = 0; i < gadgetList.gadgets.Count; i++) {
            gadgetUnlocked.Add(false);
        }
    }

    private void OnEnable() {
        UI.Instance.OnSave += Save;
        UI.Instance.OnLoad += Load;
        SceneManager.sceneLoaded += OnSceneLoaded;
        sceneTransitionHandler.OnPreEndCurrentScene += Instance_OnPreEndCurrentScene;
        //UI.Instance.OnNewGame += NewGame;
        RegisterLuaFunctions();
    }

    private void OnDisable() {
        UI.Instance.OnSave -= Save;
        UI.Instance.OnLoad -= Load;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        //UI.Instance.OnNewGame -= NewGame;
        sceneTransitionHandler.OnPreEndCurrentScene -= Instance_OnPreEndCurrentScene;
    }
    #region Lua Functions
    private void RegisterLuaFunctions() {
        //Debug.Log("Inventory RegisterLuaFunctions");
        Lua.RegisterFunction("InventoryHas", this, SymbolExtensions.GetMethodInfo(() => InventoryHas(string.Empty)));
        Lua.RegisterFunction("InventoryCount", this, SymbolExtensions.GetMethodInfo(() => InventoryCount(string.Empty)));
        Lua.RegisterFunction("InventoryAdd", this, SymbolExtensions.GetMethodInfo(() => InventoryAdd(string.Empty, 0f)));
        Lua.RegisterFunction("InventorySubtract", this, SymbolExtensions.GetMethodInfo(() => InventorySubtract(string.Empty, 0f)));
        Lua.RegisterFunction("InventoryRemove", this, SymbolExtensions.GetMethodInfo(() => InventoryRemove(string.Empty)));
        Lua.RegisterFunction("InventoryGive", this, SymbolExtensions.GetMethodInfo(() => InventoryGive(string.Empty, 0f)));

        //Holdables
        Lua.RegisterFunction("IsHolding", this, SymbolExtensions.GetMethodInfo(() => IsHolding()));
        Lua.RegisterFunction("HoldingAnything", this, SymbolExtensions.GetMethodInfo(() => HoldingAnything()));
        Lua.RegisterFunction("HoldableGive", this, SymbolExtensions.GetMethodInfo(() => HoldableGive(string.Empty, HoldableID.None)));
        Lua.RegisterFunction("HoldableClear", this, SymbolExtensions.GetMethodInfo(() => HoldableClear()));
        Lua.RegisterFunction("HoldableUse", this, SymbolExtensions.GetMethodInfo(() => HoldableUse()));
    }

    public bool InventoryHas(string name) {
        Debug.Log("Looking for a: " + name);
        foreach (var item in items) { //Ensure the item exists in the inventory
            //Debug.Log("Comparing against: " + item.item.name);
            if (item.item == null) {
                Debug.LogWarning(item.ToString() + "'s item property is Null. This should be fixed!");
            } else if (item.item.ID == name) {
                Debug.Log("Found it");
                return true;
            }
        }
        Debug.Log("Did not find it");
        return false;
    }

    public double InventoryCount(string name) {
        int count = 0;
        Debug.Log("Counting: " + name);
        foreach (var item in items) { //Count occurrences of the item in the inventory
            //Debug.Log("Comparing against: " + item.item.name);
            if (item.item == null) {
                Debug.LogWarning(item.ToString() + "'s item property is Null. This should be fixed!");
            } else if (item.item.ID == name) {
                count = item.quantity;
                break;
            }
        }
        Debug.Log("Counted: " + count);
        return count;
    }

    public void InventoryAdd(string name, double quantity) {
        Debug.Log("Attempting to add to Inventory: " + name);
        bool _success = false;
        Item newItem = null;
        foreach (var item in itemMetaList.items) { //Find the Item in the Meta list based on String reference, add X of it to the inventory
            if (item.ID == "") {
                Debug.LogWarning(item.ToString() + "'s ID property is an empty string. This should be fixed!");
            } else if (item.ID == name) {
                //items.Add(new InventoryItem(item, quantity));
                newItem = item;
                Add(item, (int)quantity);
                Debug.Log("Successfully added to Inventory: " + item.ID);
                _success = true;
                break;
            }
        }
        if (!_success) {
            Debug.LogWarning("Failed to add \""+name+ "\" to Inventory, are you sure it's in the ItemMetaList?");
        }
        if (newItem != null) {
            OnItemChanged?.Invoke(newItem);
        }
    }

    public void InventoryGive(string name, double quantity) {
        Debug.Log("Attempting to add to Inventory: " + name);
        bool _success = false;
        Item newItem = null;
        foreach (var item in itemMetaList.items) { //Find the Item in the Meta list based on String reference, add X of it to the inventory
            if (item.ID == "") {
                Debug.LogWarning(item.ToString() + "'s ID property is an empty string. This should be fixed!");
            } else if (item.ID == name) {
                //items.Add(new InventoryItem(item, quantity));
                newItem = item;
                Add(item, (int)quantity);
                Debug.Log("Successfully added to Inventory: " + item.ID);
                _success = true;
                OnItemGiven?.Invoke(item);
                break;
            }
        }
        if (!_success) {
            Debug.LogWarning("Failed to add \"" + name + "\" to Inventory, are you sure it's in the ItemMetaList?");
        }
        if (newItem != null) {
            OnItemChanged?.Invoke(newItem);
        }
    }

    public void InventorySubtract(string name, double _quantity) {
        int quantity = (int)_quantity;
        Debug.Log("Subtracting " + quantity + " " + name);
        Item newItem = null;
        while (quantity > 0) {
            foreach (var item in items) { //Try to remove an item if it exists in the inventory
                if (item.item == null) {
                    Debug.LogWarning(item.ToString() + "'s item property is Null. This should be fixed!");
                } else if (item.item.ID == name) {
                    newItem = item.item;
                    while (quantity > 0) {
                        if (item.quantity > 0) {
                            Remove(item.item);
                            newItem = null;
                            Debug.Log("Successfully subtracted");
                        } else
                            Debug.LogWarning("Trying to remove more of an item from the Inventory than exists in the inventory");
                        quantity--;
                    }
                    break;
                }
            }
            quantity--;
        }
        if (newItem != null) {
            OnItemChanged?.Invoke(newItem);
        }
    }

    public void InventoryRemove(string name) {
        Debug.Log("Removing " + name);
        //Remove all occurrences of an item from the inventory, good if you don't want to be specific
        Item newItem = null;
        foreach (var item in items) {
            if (item.item == null) {
                Debug.LogWarning(item.ToString() + "'s item property is Null. This should be fixed!");
            } else if (item.item.ID == name) {
                Debug.Log("Found and successfully removed " + name);
                RemoveAll(item.item);
                break;
            }
        }
        //items.RemoveAll(item => item.item.name == name); //Remove everything that matches the name
        if (newItem != null) {
            OnItemChanged?.Invoke(newItem);
        }
    }
    #endregion

    #region Holdable Functions
    public string IsHolding() {
        if (RigHeld == null) {
            return "";
        }
        return RigHeld.holdableData.name;
    }

    public bool HoldingAnything() {
        return RigHeld != null;
    }

    public void HoldableGive(string ID, HoldableID holdableID) {
        //HoldablePickUp(instance.holdableMetaList.GetHoldable(instance.holdableMetaList.GetIndex(ID)));
        HoldablePickUp(holdableMetaList.holdables.FirstOrDefault(p => p.name == ID), holdableID);
    }

    public bool HoldablePickUp(HoldableData holdableData, HoldableID holdableID) {
        if (RigHeld == null) {
            UI.Player.rigManager.AssignRig(holdableData);
            return true;
        }
        return false;
    }


    public GameObject HoldableDrop() {
        GameObject toDrop = null; //Drop the Held item
        if (RigHeld != null && RigHeld.holdableData.worldPrefab != null) {
            if (RigHeld.CanDrop()) {
                toDrop = Instantiate(RigHeld.holdableData.worldPrefab, RigHeld.transform.position, RigHeld.transform.rotation);
                OnDrop?.Invoke(RigHeld.holdableData);
                HoldableInteractable holdableInteractable = toDrop.GetComponentInChildren<HoldableInteractable>();
                //Debug.Log("holdableInteractable: " + holdableInteractable);
                if (holdableInteractable.holdableData.holdableType == HoldableType.HTypePermanent)
                    HoldableRegister(holdableInteractable);
                HoldableClear();
            }
        }
        return toDrop;
    }

    public void HoldableClear() {
        UI.Player.rigManager.ClearRig();
    }

    public void HoldableUse() {
        if (RigHeld != null) {
            RigHeld.Use();
        }
    }

    public void HoldableRegister(HoldableInteractable holdableInteractable) {
        HoldableRegistered foundHoldable;
        string _currentScene = SceneManager.GetActiveScene().name;
        registeredHoldables.TryGetValue(holdableInteractable.holdableData.holdableID, out foundHoldable);
        if (foundHoldable == null) {
            foundHoldable = new HoldableRegistered();
            foundHoldable.Register(_currentScene, holdableMetaList.GetIndex(holdableInteractable.holdableData), holdableInteractable.holdableData.holdableID);
            registeredHoldables.Add(holdableInteractable.holdableData.holdableID, foundHoldable);
        }
        foundHoldable.EndOfSceneUpKeep(_currentScene, holdableInteractable);
    }

    public void HoldableDeregister(HoldableInteractable holdableInteractable) {
        //Debug.Log("HoldableDeregister: " + holdableInteractable.holdableID);
        HoldableRegistered foundHoldable;
        registeredHoldables.TryGetValue(holdableInteractable.holdableData.holdableID, out foundHoldable);
        if (foundHoldable != null) {
            registeredHoldables.Remove(holdableInteractable.holdableData.holdableID);
        }
    }

    public bool HoldableIsRegisteredHeldOrSold(HoldableID holdableID) { //TODO: Account for Holdable being sold
        return registeredHoldables.ContainsKey(holdableID) ||
            (heldBetweenScenesIndex != -1 && holdableMetaList.GetHoldable(heldBetweenScenesIndex).holdableID == holdableID) || //OR; the held item maatches the index of the Holdable Interactable checking
            (RigHeld != null && RigHeld.holdableData.holdableID == holdableID); //The rig has already been supplied with the object
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (heldBetweenScenesIndex != -1) {
            if (UI.Player == null) {
                Debug.LogWarning("Player has not spawned yet; attempting fallback");
                if (!hasSubscribedToFallbackMethod) {
                    hasSubscribedToFallbackMethod = true;
                    sceneTransitionHandler.OnPlayerSpawn += SpawnManager_OnPlayerSpawn;
                }
            } else { //Give Player Held Item
                AssignHeldBetweenScenes();
            }
        }
    //Instantiate every registered Holdable
        foreach (var _holdable in registeredHoldables.Values) {
            if (_holdable.CurrentScene == SceneManager.GetActiveScene().name) {
                Instantiate(holdableMetaList.GetHoldable(_holdable.HoldableDataIndex).worldPrefab
                    , _holdable.Position, _holdable.Rotation);
            }
        }
    }



    private void AssignHeldBetweenScenes() {
        UI.Player.rigManager.AssignRig(heldBetweenScenesIndex);
        heldBetweenScenesIndex = -1;
    }

    private void SpawnManager_OnPlayerSpawn(string sceneName, SpawnPoints point) {
        //Debug.LogError("FALLBACK SpawnManager_OnPlayerSpawn");
        sceneTransitionHandler.OnPlayerSpawn -= SpawnManager_OnPlayerSpawn;
        hasSubscribedToFallbackMethod = false;
        if (heldBetweenScenesIndex == -1) {
            Debug.LogError("heldBetweenScenesIndex has been set to -1, this should not happen!");
        } else {
            AssignHeldBetweenScenes();
        }
    }

    private void Instance_OnPreEndCurrentScene(string sceneName, SpawnPoints point) {
        //Delete the Holdable
        if (RigHeld != null) {
            if (RigHeld.holdableData.holdableType == HoldableType.HTypeLocked) {
                HoldableClear();
            } else {
                heldBetweenScenesIndex = holdableMetaList.GetIndex(RigHeld.holdableData);
            }
        }
    }
    #endregion

    public void Save(int fileIndex) {
        ES3.Save<List<bool>>(saveStringGadgets, gadgetUnlocked, UI.Instance.saveSettings);
        List<int> itemIDs = new List<int>();
        List<int> itemCount = new List<int>();
        foreach (var item in items) {
            itemIDs.Add(itemMetaList.GetIndex(item.item));
            itemCount.Add(item.quantity);
        }
        ES3.Save<List<int>>(saveStringItemIDs, itemIDs, UI.Instance.saveSettings);
        ES3.Save<List<int>>(saveStringItemCount, itemCount, UI.Instance.saveSettings);
    //Holdables
        OnPreSave();
        ES3.Save(saveStringHeldItem, RigHeld ==  null ? -1 : holdableMetaList.GetIndex(RigHeld.holdableData), UI.Instance.saveSettings);
        ES3.Save<List<HoldableID>>(saveStringHoldableIDs, registeredHoldables.Keys.ToList(), UI.Instance.saveSettings);
        foreach (var _holdable in registeredHoldables.Values) {
            _holdable.Save();
        }
        //List<bool> _gadgetsUnlocked = new List<bool>();
    }

    public void Load(int fileIndex) {
        gadgetUnlocked = ES3.Load(saveStringGadgets, new List<bool>(), UI.Instance.saveSettings);
        List<int> loadItems = ES3.Load(saveStringItemIDs, new List<int>(), UI.Instance.saveSettings);
        List<int> loadCount = ES3.Load(saveStringItemCount, new List<int>(), UI.Instance.saveSettings);
        items.Clear();
        for (int i = 0; i < loadItems.Count; i++) {
            items.Add(new InventoryItem(itemMetaList.GetItem(loadItems[i]), loadCount[i]));
        }
    //Holdables
        heldBetweenScenesIndex = ES3.Load(saveStringHeldItem, -1, UI.Instance.saveSettings);
        List<HoldableID> _holdableIDs = ES3.Load(saveStringHoldableIDs, new List<HoldableID>(), UI.Instance.saveSettings);
        registeredHoldables.Clear();
        foreach (var _holdableID in _holdableIDs) {
            HoldableRegistered loadHoldable = new HoldableRegistered();
            loadHoldable.Load(_holdableID);
            registeredHoldables.Add(_holdableID, loadHoldable);
        }
        /*
        foreach (var item in loadItems) {
            items.Add(new InventoryItem(itemMetaList.GetItem(item.itemID), item.quantity));
        }
        //*/
    }

    //private void NewGame(int fileNum) {
    //    pendingPickUps.Clear();
    //}

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
        OnItemChanged?.Invoke(item);
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
        OnItemChanged?.Invoke(item);
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
        OnItemChanged?.Invoke(item);
    }

//    public GameObject Drop(Item _toDrop) {
//        GameObject toDrop = null;
////Drop an item from the Inventory
//        foreach (var item in items) { //Ensure the item exists in the inventory
//            if (item.item == _toDrop) {
//                if (item.item.physicalItem != null) {
//                    toDrop = Instantiate(item.item.physicalItem, dropPosition, Quaternion.identity);
//                    Instantiate(prefabDroppedItemInteractable, toDrop.transform);
//                    Instantiate(pickupSphere, dropPosition, Quaternion.identity); //Drop a sphere where the item was dropped
//                    OnDrop?.Invoke(item.item);
//                }
//                Remove(item.item);
//                break;
//            }
//        }
//        return toDrop;
//    }

    public static InventoryItem GetInventoryItem(Item lookupItem) {
        InventoryItem inventoryItem = null;
        foreach (var item in instance.items) { //Ensure the item exists in the inventory
            if (item.item == lookupItem) {
                inventoryItem = item;
                break;
            }
        }
        return inventoryItem;
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

public class HoldableRegistered
{
    public HoldableID HoldableID { get; private set; }
    public int HoldableDataIndex { get; private set; }
    public string OriginalScene { get; private set; }
    public string CurrentScene { get; private set; }
    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }

    public void EndOfSceneUpKeep(string _scene, HoldableInteractable holdableInteractable) {
        CurrentScene = _scene;
        Position = holdableInteractable.transform.position;
        Rotation = holdableInteractable.transform.rotation;
    }

    public void Register(string _scene, int holdableDataIndex, HoldableID holdableID) {
        OriginalScene = _scene;
        HoldableDataIndex = holdableDataIndex;
        HoldableID = holdableID;
    }

    public void Save() {
        ES3.Save("HoldableID_" + HoldableID.ToString(), HoldableID, UI.Instance.saveSettings);
        ES3.Save("HoldableDataIndex_" + HoldableID.ToString(), HoldableDataIndex, UI.Instance.saveSettings);
        ES3.Save("OriginalScene_" + HoldableID.ToString(), OriginalScene, UI.Instance.saveSettings);
        ES3.Save("CurrentScene_" + HoldableID.ToString(), CurrentScene, UI.Instance.saveSettings);
        ES3.Save("HoldablePosition_" + HoldableID.ToString(), Position, UI.Instance.saveSettings);
        ES3.Save("HoldableRotation_" + HoldableID.ToString(), Rotation, UI.Instance.saveSettings);
    }

    public void Load(HoldableID _HoldableID) {
        HoldableID = ES3.Load("HoldableID_" + _HoldableID.ToString(), HoldableID.None, UI.Instance.saveSettings);
        HoldableDataIndex = ES3.Load("HoldableDataIndex_" + _HoldableID.ToString(), 0, UI.Instance.saveSettings);
        OriginalScene = ES3.Load("OriginalScene_" + _HoldableID.ToString(), UI.Instance.saveSettings.path, "", UI.Instance.saveSettings);
        CurrentScene = ES3.Load("CurrentScene_" + _HoldableID.ToString(), UI.Instance.saveSettings.path, "", UI.Instance.saveSettings);
        Position = ES3.Load("HoldablePosition_" + _HoldableID.ToString(), Vector3.zero, UI.Instance.saveSettings);
        Rotation = ES3.Load("HoldableRotation_" + _HoldableID.ToString(), Quaternion.identity, UI.Instance.saveSettings);
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