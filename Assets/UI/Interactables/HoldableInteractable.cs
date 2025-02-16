﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HoldableInteractable : Interactable
{
    public bool doSaveLoad = true;
    public HoldableData holdableData;
    public GameObject itemToPickup;

    private SceneTransitionHandler sceneTransitionHandler;

    private void Awake() {
        sceneTransitionHandler = Inventory.instance.sceneTransitionHandler;
    }

    private void OnEnable() {
        sceneTransitionHandler.OnPreEndCurrentScene += Instance_OnPreEndCurrentScene;
        SceneManager.sceneLoaded += OnSceneLoaded;
        Inventory.instance.OnPreSave += Instance_OnPreSave;
    }

    private void OnDisable() {
        sceneTransitionHandler.OnPreEndCurrentScene -= Instance_OnPreEndCurrentScene;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Inventory.instance.OnPreSave -= Instance_OnPreSave;
    }

    public override void Interact() {
        if (UI.LockControls) {
            return;
        }
        bool wasPickedUp = Inventory.instance.HoldablePickUp(holdableData, holdableData.holdableID);
        if (wasPickedUp) {
            if (itemToPickup == null) {
                Destroy(gameObject);
            } else {
                Destroy(itemToPickup);
            }
            if (holdableData.holdableType == HoldableType.HTypePermanent)
                Inventory.instance.HoldableDeregister(this); //The item in the player's hands isn't handled by the "Registered" system
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (doSaveLoad) {
            //var result = UI.CompareChange(GetSaveID());
            if (Inventory.instance.HoldableIsRegisteredHeldOrSold(holdableData.holdableID)) { //Check if registered
                Destroy(itemToPickup); //The inventory will take care of spawning this instance as necessary
            }
        }
    }

    private void Instance_OnPreSave() {
        if (holdableData.holdableType == HoldableType.HTypePermanent)
            Inventory.instance.HoldableRegister(this);
    }

    private void Instance_OnPreEndCurrentScene(string sceneName, SpawnPoints point) { //This is for transition between Spawn Points / Scenes; NOT saving
        Instance_OnPreSave(); //Save the current position of the Holdable
    }
}