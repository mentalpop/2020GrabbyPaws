using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrencyDisplay : MonoBehaviour
{
    public AnimatedUIContainer container;
    public TextMeshProUGUI cashDisplay;
    public InventoryDisplay inventoryDisplay; //Reference to this to check if it is open before closing self
    
    public float timeShowCurrency = 1.5f;
    public float durationShake;
    //public Sine updateShake;

    private int localCash;
    private int displayCash;
    private int deltaCash;
    private int cashPrevious;
    private bool effectActive = false;
    private float durationCount;

    private Coroutine currencyDisplayRoutine;

    private void OnEnable() {
        container.OnEffectComplete += Container_OnEffectComplete;
    }

    private void OnDisable() {
        container.OnEffectComplete -= Container_OnEffectComplete;
    }

    private void Container_OnEffectComplete(bool reverse) {
        if (reverse) {
            gameObject.SetActive(false);
        }
    }

    public void Open() {
        container.gTween.Reset();
    }

    public void Close() {
        if (!container.gTween.doReverse)
            container.gTween.Reverse();
    }

    private void Update() {
        if (effectActive) {
            if (durationCount < durationShake) {
                durationCount += Time.deltaTime;
                displayCash = (int)(cashPrevious + deltaCash * (float)(durationCount / durationShake));
            } else {
                effectActive = false;
                displayCash = localCash; //Set to the correct amount
                ClearCloseFlag();
                currencyDisplayRoutine = StartCoroutine(DelayHideCurrencyDisplay());
            }
            cashDisplay.text = string.Format("{0:n0}", displayCash); //Display currency amount with commas, no decimals (although there shouldn't be any!!)
        }
    }

    public void UpdateCashDisplay() {
        effectActive = true;
        cashPrevious = localCash; //The amount of money before the update
        displayCash = localCash; //The cached Cash amount to display with the String
        deltaCash = Currency.instance.Cash - localCash; //The delta which will be added to the display amount
        localCash = Currency.instance.Cash;
        durationCount = 0f;
        ClearCloseFlag();
        //updateShake.Reset();
    }

    public void ClearCloseFlag() {
        if (currencyDisplayRoutine != null)
            StopCoroutine(currencyDisplayRoutine); //Clear the coroutine in case it was active
    }
    
    IEnumerator DelayHideCurrencyDisplay() {
        yield return new WaitForSeconds(timeShowCurrency);
        currencyDisplayRoutine = null;
        if (!inventoryDisplay.gameObject.activeSelf) {
            Close();
        }
    }
}