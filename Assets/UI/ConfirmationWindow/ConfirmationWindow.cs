using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ConfirmationWindow : MonoBehaviour
{
    public TextMeshProUGUI header;
    public TextMeshProUGUI description;
    public TextMeshProUGUI buttonA;
    public TextMeshProUGUI buttonB;
    public ButtonGeneric buttonOK;
    public ButtonGeneric buttonNo;
    public UI UIRef;
    public ClickToClose clickToClose;
    [HideInInspector] public ConfirmationPromptData promptData;

    public delegate void ConfirmationEvent(bool result);
    public event ConfirmationEvent OnChoiceMade = delegate { };

    private void OnEnable() {
        buttonOK.OnClick += OnClickOK;
        buttonNo.OnClick += OnClickNo;
		clickToClose.OnClick += OnClickNo;
    }

    private void OnDisable() {
        buttonOK.OnClick -= OnClickOK;
        buttonNo.OnClick -= OnClickNo;
		clickToClose.OnClick -= OnClickNo;
    }

    public void OnClickOK() {
        OnChoiceMade(true);
        Close();
    }

    public void OnClickNo() {
        OnChoiceMade(false);
        Close();
    }

    public void Close() {
        gameObject.SetActive(false);
    }

    public void Unpack(ConfirmationPromptData _data) {
        promptData = _data;
        //Debug.Log("_data: "+_data);
        header.text = _data.header.ToUpper();
        description.text = _data.description.ToUpper();
        buttonA.text = _data.buttonA.ToUpper();
        buttonB.text = _data.buttonB.ToUpper();
    }
}