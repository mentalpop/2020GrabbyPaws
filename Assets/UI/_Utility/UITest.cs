using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITest : MonoBehaviour
{
    public ReadableData sampleBook;

    private void Start() {
//Debug; 
        //UI.Instance.ShowLappyMenu(true);  //open Lappy menu instantly to test
        UI.Instance.ShowInventoryDisplay();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.B)) { //B for Book
            UI.Instance.DisplayReadable(sampleBook);
        }
        /*
        if (Input.GetKeyDown(KeyCode.Keypad0)) {
            float test = Sonos.GetVolume(AudioType.Music);
            Debug.Log("AudioType.Music: "+test);
        }
        //*/
        if (Input.GetKeyDown(KeyCode.Keypad0)) {
            FlagRepository.WriteQuestKey(QuestNames.q001TwilightCottonCandy.ToString(), true);
        }
//Saving / Loading
        if (Input.GetKeyDown(KeyCode.V)) { //Save
            UI.Instance.SaveGameData(0);
        }
        if (Input.GetKeyDown(KeyCode.L)) { //Load
            UI.Instance.LoadGameData(0);
        }
//Currency
        if (Input.GetKeyDown(KeyCode.KeypadPlus)) { //Add LOTS of funds
            Currency.instance.Cash += 10000000m;
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus)) { //Subtract Funds
            Currency.instance.Cash -= 100m;
        }
        if (Input.GetKeyDown(KeyCode.KeypadMultiply)) { //Try a Purchase, display if successful
            if (Currency.instance.Purchase(500m)) {
                Debug.Log("Purchase successful!");
            } else {
                Debug.Log("Not enough funds!");
            }
        }
    }
}