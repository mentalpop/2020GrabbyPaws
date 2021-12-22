using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;
using PixelCrushers.DialogueSystem;

public class SpawnDoor : Interactable
{
    public Outline outline;
    public string sceneName;
    public SpawnPoints destinationPoint;
    [Header("Limits")]
    public List<Item> items = new List<Item>();
    public List<QuestNames> quests = new List<QuestNames>();
    public List<Secrets> secrets = new List<Secrets>();
    [Header("Barks")]
    public DialogueSystemTrigger barkTrigger;
    /*
    public List<string> barks = new List<string>();
    public string stringOverride = "";
    //*/
    [Header("Colors")]
    public int colorOpen = 2;
    public int colorLocked = 1;

    private bool isUsable = false;

    private void Start() {
        CheckConditions();
    }

    public override void Interact() {
        //Debug.Log("sceneName: "+sceneName);
        CheckConditions();
        if (isUsable) {
            SceneTransitionHandler.SceneGoto(sceneName, destinationPoint);
        } else {
            if (barkTrigger != null) {
        //Inject the player's transform before it is used
                barkTrigger.barker = SceneTransitionHandler.GetPlayer().transform;
                //barkTrigger.conversationActor = barkTrigger.barker;
                barkTrigger.gameObject.SetActive(true);
            }
        }
    }

    public void CheckConditions() {
        if (UI.LockControls) {
            isUsable = false;
            return;
        }
        isUsable = true;
        if (items.Count > 0) {
            foreach (var item in items) {
                if (!Inventory.instance.InventoryHas(item.ID)) {
                    isUsable = false;
                    break;
                }
            }
        }
        if (quests.Count > 0) {
            foreach (var quest in quests) {
                if (!FlagRepository.ReadQuestKey(quest.ToString())) {
                    isUsable = false;
                    break;
                }
            }
        }
        if (secrets.Count > 0) {
            foreach (var secret in secrets) {
                if (FlagRepository.ReadSecretKey(secret.ToString()) < 1) { //Secret hasn't been found
                    isUsable = false;
                    break;
                }
            }
        }
        outline.color = isUsable ? colorOpen : colorLocked;
    }
}