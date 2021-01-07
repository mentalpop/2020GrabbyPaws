using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using System;

public class FlagRepository : MonoBehaviour
{
    //public QuestCompletionFlags questFlags;
    public SecretFlagList secretFlags;
    public GameFlags flags = new GameFlags();

    public static FlagRepository instance;

    private void Awake() {
    //Singleton Pattern
        if (instance != null && instance != this) { 
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    //Initialize each to 0 //TODO: Load in these values
        foreach (QuestNames flag in Enum.GetValues(typeof(QuestNames))) {
            flags.questFlags.Add(flag.ToString(), false);
        }
        foreach (var flag in secretFlags.secrets) {
            flags.secretFlags.Add(flag.secret.ToString(), 0);
        }
        RegisterLuaFunctions();
    }

    #region Lua Functions
    private void RegisterLuaFunctions() {
        //Debug.Log("FLAGS RegisterLuaFunctions");
        Lua.RegisterFunction("QuestRead", this, SymbolExtensions.GetMethodInfo(() => QuestRead(string.Empty)));
        Lua.RegisterFunction("QuestCompelete", this, SymbolExtensions.GetMethodInfo(() => QuestCompelete(string.Empty)));
        Lua.RegisterFunction("SecretRead", this, SymbolExtensions.GetMethodInfo(() => SecretRead(string.Empty)));
        Lua.RegisterFunction("SecretFound", this, SymbolExtensions.GetMethodInfo(() => SecretFound(string.Empty)));
        Lua.RegisterFunction("SecretWrite", this, SymbolExtensions.GetMethodInfo(() => SecretWrite(string.Empty, 0)));
    }

    public bool QuestRead(string name) { return ReadQuestKey(name); }
    public void QuestCompelete(string name) { WriteQuestKey(name, true); }
    public double SecretRead(string name) { return ReadSecretKey(name); }
    public void SecretFound(string name) { SecretKeyFound(name); }
    public void SecretWrite(string name, double value) { WriteSecretKey(name, value); }

    #endregion

    //Get / Set
    public static bool ReadQuestKey(string key) {
        instance.flags.questFlags.TryGetValue(key, out bool readValue);
        return readValue;
    }

    public static void WriteQuestKey(string key, bool value) {
        instance.flags.questFlags[key] = value;
    }
//Secret Key
    public static int ReadSecretKey(string key) {
        instance.flags.secretFlags.TryGetValue(key, out int readValue);
        return readValue;
    }

    public static void SecretKeyFound(string key) { //"Find" a secret; set it's state to 1 if it isn't 2
        if (instance.flags.secretFlags.TryGetValue(key, out int readValue)) {
            if (readValue == 0)
                instance.flags.secretFlags[key] = 1;
        }
    }

    public static void SecretKeyStrike(string key) { //Strike a secret from the list, but only if it has been found
        if (instance.flags.secretFlags.TryGetValue(key, out int readValue)) {
            if (readValue == 1)
                instance.flags.secretFlags[key] = 2;
        }
    }

    public static void WriteSecretKey(string key, double value) {
        instance.flags.secretFlags[key] = (int)value;
    }
//Save / Load
    private void OnEnable() {
        UI.Instance.OnSave += Save;
        UI.Instance.OnLoad += Load;
    }

    private void OnDisable() {
        UI.Instance.OnSave -= Save;
        UI.Instance.OnLoad -= Load;
    }

    private string saveString = "flags";

    public void Save(int fileIndex) {
        ES3.Save<GameFlags>(saveString, flags);
    }

    public void Load(int fileIndex) {
        flags = ES3.Load(saveString, new GameFlags());
    }
}

public class GameFlags
{
    public Dictionary<string, bool> questFlags = new Dictionary<string, bool>();
    public Dictionary<string, int> secretFlags = new Dictionary<string, int>();
}