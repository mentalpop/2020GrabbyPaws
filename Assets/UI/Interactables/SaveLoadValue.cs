using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadValue : MonoBehaviour
{
    public bool Value {
        get { return _SavedValue; }
        set {
            _SavedValue = value;

        }
    }
    private bool _SavedValue;

    private void OnEnable() {
        UI.Instance.OnSave += Save;
        UI.Instance.OnLoad += Load;
    }

    private void OnDisable() {
        UI.Instance.OnSave -= Save;
        UI.Instance.OnLoad -= Load;
    }

    public void Save(int fileIndex) {
        ES3.Save(GetSaveID(), _SavedValue);
    }

    public void Load(int fileIndex) {
        _SavedValue = ES3.Load(GetSaveID(), false, UI.Instance.saveSettings);
    }

    public virtual string GetSaveID() {
        return string.Format("{0}_{1}_{2}_{3}_{4}_{5}", UI.GetCurrentFile(), SceneTransitionHandler.CurrentScene(), gameObject.name, transform.position.x, transform.position.y, transform.position.z);
    }
}