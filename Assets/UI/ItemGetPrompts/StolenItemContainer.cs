using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StolenItemContainer : MonoBehaviour
{
    public GameObject sipPrefab;
    public Inventory inventory;

    private bool subbedEvents = false;
    private List<StolenItemPrompt> prompts = new List<StolenItemPrompt>();

    private void OnEnable() {
        if (!subbedEvents && SceneTransitionHandler.instance != null) {
            SceneTransitionHandler.instance.OnBeginTransitionToNewScene += Instance_OnBeginTransitionToNewScene;
            subbedEvents = true;
        }
        inventory.OnPickUp += PickUp;
    }

    private void OnDisable() {
        if (subbedEvents && SceneTransitionHandler.instance != null) {
            SceneTransitionHandler.instance.OnBeginTransitionToNewScene -= Instance_OnBeginTransitionToNewScene;
            subbedEvents = false;
        }
        inventory.OnPickUp -= PickUp;
    }

    private void Start() { //Try subscribing in the Start method as a fallback because OnEnable might happen too soon?
        if (!subbedEvents && SceneTransitionHandler.instance != null) {
            SceneTransitionHandler.instance.OnBeginTransitionToNewScene += Instance_OnBeginTransitionToNewScene;
            subbedEvents = true;
        }
    }

    private void PickUp(Item item) {
        GameObject newGO = Instantiate(sipPrefab, transform, false);
        StolenItemPrompt prompt = newGO.GetComponent<StolenItemPrompt>();
        prompt.Unpack(Inventory.GetInventoryItem(item), this);
    //Shift all existing prompts up
        float destPosition = 0f;
        foreach (var _prompt in prompts) {
            if (destPosition < 2f) {
                BTweenRectAnchor bTweenRectAnchor = _prompt.rectFadeIn;
                bTweenRectAnchor.positionMin = new Vector2(0f, destPosition);
                bTweenRectAnchor.positionMax = new Vector2(0f, destPosition + 1f);
                bTweenRectAnchor.BTween.PlayFromZero(); //This is sort of cheating, but the transition happens so quickly that it isn't too noticeable that the transition restarts, jumping to the new starting position instead of smoothly moving into place from its current position
                destPosition += 1f;
            } else {
        //Any prompts beyond this should just close immediately
                _prompt.Close();
            }
        }
        prompts.Insert(0, prompt); //New items placed at top of list
    }

    public void RemoveThis(StolenItemPrompt prompt) {
        prompts.Remove(prompt);
    }

    private void Instance_OnBeginTransitionToNewScene(string sceneName, SpawnPoints point) {
    //Close all Prompts
        foreach (var _prompt in prompts) {
            _prompt.Close();
        }
    }
}