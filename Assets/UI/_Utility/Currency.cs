using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using System;

public class Currency : MonoBehaviour
{
    public int startingFunds = 500;
    public int maxFunds = 99999999;

    public static Currency instance;
    public int Cash {
        get { return _cash; }
        set {
            _cash = value;
            if (_cash > maxFunds)
                _cash = maxFunds;
            if (_cash < 0) {
                _cash = 0;
                Debug.LogWarning("Cash should not go below zero. This should be fixed!");
            }
            OnCashChanged?.Invoke();
        }
    }
    private int _cash;

    private string saveString = "currency";

    public delegate void CurrencyEvent();
    public CurrencyEvent OnCashChanged = delegate { };

    private void Awake() {
    //Singleton Pattern
        if (instance != null && instance != this) { 
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
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
        //Debug.Log("Currency RegisterLuaFunctions");
        Lua.RegisterFunction("BuckleCount", this, SymbolExtensions.GetMethodInfo(() => BuckleCount()));
        Lua.RegisterFunction("BuckleAdd", this, SymbolExtensions.GetMethodInfo(() => BuckleAdd(0f)));
        Lua.RegisterFunction("BuckleBuy", this, SymbolExtensions.GetMethodInfo(() => BuckleBuy(0f)));
    }

    private bool BuckleBuy(double v) { //The Lua method which uses Doubles
        return BuckleBuy((int)v);
    }

    private void BuckleAdd(double v) {
        Cash += (int)v;
    }

    private double BuckleCount() {
        return Cash;
    }
    #endregion

    public void Save(int fileIndex) {
        ES3.Save<decimal>(saveString, _cash);
    }

    public void Load(int fileIndex) {
        Cash = ES3.Load(saveString, startingFunds);
    }

    private void Start() {
        Cash = startingFunds;
    }

    public bool BuckleBuy(int cost) {
        bool purchaseSuccess = false;
        if (Cash >= cost) {
            Cash -= cost;
            purchaseSuccess = true;
        }
        return purchaseSuccess;
    }

    /*
    public bool Purchase(int cost) {
//Pass in a float, convert it to decimal
        return Purchase(cost);
    }
    //*/
}