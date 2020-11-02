using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickToClose : MonoBehaviour
{
    public NavButton button;
    
    public delegate void CloseEvent();
    public CloseEvent OnClick;

    private void OnEnable() {
        UI.Instance.OnUIScaled += Instance_OnUIScaled;
        button.OnSelect += button_OnSelect;
        Instance_OnUIScaled(1f);
    }

    private void OnDisable() {
        UI.Instance.OnUIScaled -= Instance_OnUIScaled;
        button.OnSelect -= button_OnSelect;
    }

    private void Instance_OnUIScaled(float scale) {
//Center and fill screen
        RectTransform myRect = GetComponent<RectTransform>();
        myRect.pivot = new Vector2(0.5f, 0.5f);
        myRect.sizeDelta = new Vector2(ScreenSpace.Width, ScreenSpace.Height);
        transform.position = Vector2.zero;
    }

    private void button_OnSelect(ButtonStateData _buttonStateData) {
        //Debug.Log("OnPointerClick");
		ManualClose();
    }

    public void ManualClose() {
        OnClick?.Invoke();
    }
}