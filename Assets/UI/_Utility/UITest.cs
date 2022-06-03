using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITest : MonoBehaviour
{
    public ReadableData sampleBook;
    public ReadableData sampleSign;
    public ReadablePCData samplePC;
    public ShopUIData shopUIData;

    private void Start() {
//Debug; 
        //UI.Instance.ShowLappyMenu(true);  //open Lappy menu instantly to test
        //UI.Instance.ShowInventoryDisplay();
        //UI.Instance.DisplayPC(samplePC);
    //Unlock Secrets so they become visible in the Not Secrets window
        FlagRepository.SecretKeyFound(Secrets.s001Test.ToString());
        FlagRepository.SecretKeyFound(Secrets.s002Test.ToString());
        FlagRepository.SecretKeyFound(Secrets.s003Test.ToString());
        FlagRepository.SecretKeyFound(Secrets.s004Test.ToString());
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.B)) { //B for Book
            UI.Instance.DisplayBook(sampleBook);
        }
        if (Input.GetKeyDown(KeyCode.N)) { //N for SigN
            UI.Instance.DisplaySign(sampleSign);
        }
        if (Input.GetKeyDown(KeyCode.P)) { //P for PC
            UI.Instance.DisplayPC(samplePC);
        }
        if (Input.GetKeyDown(KeyCode.H)) { //H for sHop
            UI.Instance.DisplayShop(shopUIData);
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
            UI.Instance.SaveGameData(UI.GetCurrentFile());
        }
        if (Input.GetKeyDown(KeyCode.L)) { //Load
            UI.Instance.LoadGameData(UI.GetCurrentFile());
        }
//Currency
        if (Input.GetKeyDown(KeyCode.KeypadPlus)) { //Add LOTS of funds
            Currency.instance.Cash += 10000000;
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus)) { //Subtract Funds
            Currency.instance.Cash -= 100;
        }
        if (Input.GetKeyDown(KeyCode.KeypadMultiply)) { //Try a Purchase, display if successful
            if (Currency.instance.BuckleBuy(500)) {
                Debug.Log("Purchase successful!");
            } else {
                Debug.Log("Not enough funds!");
            }
        }
    }
}