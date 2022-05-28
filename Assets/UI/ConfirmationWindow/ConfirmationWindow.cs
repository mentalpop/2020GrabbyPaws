using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConfirmationWindow : MonoBehaviour
{
    public MenuHub menuHub;
    public TextMeshProUGUI header;
    public TextMeshProUGUI description;
    public TextMeshProUGUI buttonA;
    public TextMeshProUGUI buttonB;
    public NavButton buttonOK;
    public NavButton buttonNo;
    //public UI UIRef;
    public ClickToClose clickToClose;
    [HideInInspector] public ConfirmationPromptData promptData;
    public RectTransform headerTransform;
    public float headerPadding = 32f;

    //private MenuNode menuOnDisable;

    public delegate void ConfirmationEvent(bool result);
    public event ConfirmationEvent OnChoiceMade = delegate { };

    private void OnEnable() {
        buttonOK.OnSelect += OnClickOK;
        buttonOK.OnSelectExt += OnClickOKExt;
        buttonNo.OnSelect += OnClickNo;
        buttonNo.OnSelectExt += OnClickNoExt;
        clickToClose.OnClick += OnClickCancel;
        menuHub.OnMenuClose += OnClickCancel;
    }

    /*
    private void Instance_OnClose() { //MenuNode menuNode
        OnClickCancel();
    }
    //*/

    private void OnDisable() {
        buttonOK.OnSelect -= OnClickOK;
        buttonOK.OnSelectExt -= OnClickOKExt;
        buttonNo.OnSelect -= OnClickNo;
        buttonNo.OnSelectExt -= OnClickNoExt;
        clickToClose.OnClick -= OnClickCancel;
        menuHub.OnMenuClose -= OnClickCancel;
        /*
        buttonOK.SetFocus(false);
        buttonNo.SetFocus(false);
        //*/
    }

    public void OnClickOK(ButtonStateData _buttonStateData) {
        OnChoiceMade(true);
        Close();
    }

    private void OnClickOKExt(ButtonStateData _buttonStateData, object _data) {
        OnClickOK(_buttonStateData);
    }

    public void OnClickNo(ButtonStateData _buttonStateData) {
        OnChoiceMade(false);
        Close();
    }

    private void OnClickNoExt(ButtonStateData _buttonStateData, object _data) {
        OnClickNo(_buttonStateData);
    }

    public void OnClickCancel() {
        OnChoiceMade(false);
        Close();
    }

    public void Close() {
        gameObject.SetActive(false);
    }

    public void Unpack(ConfirmationPromptData _data, MenuNode _menuOnDisable) {
        promptData = _data;
        //Debug.Log("_data: "+_data);
        header.text = _data.header.ToUpper();
        headerTransform.sizeDelta = new Vector2(header.preferredWidth + headerPadding, headerTransform.sizeDelta.y); //Resize Header to match width of text
        description.text = _data.description.ToUpper();
        buttonA.text = _data.buttonA.ToUpper();
        buttonB.text = _data.buttonB.ToUpper();
        menuHub.menuOnDisable = _menuOnDisable;
        if (MenuNavigator.MouseIsUsing()) {
    //Only apply a style if using Controller
            buttonOK.SetFocus(false);
            buttonNo.SetFocus(false);
        }
        buttonOK.StateUpdate();
        buttonNo.StateUpdate();
    }
}