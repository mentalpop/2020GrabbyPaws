using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotSecretLineItem : MonoBehaviour
{
    public NavButton navButton;
    public TextMeshProUGUI text;
    public RectTransform myRect;

    [HideInInspector] public bool isStricken;

    private NotSecretData notSecretData;
    private NotSecrets notSecretsRef;
    private string myText;

    private void OnEnable() {
        navButton.OnSelect += NavButton_OnSelect;
        navButton.OnSelectExt += NavButton_OnSelectExt;
    }

    private void OnDisable() {
        navButton.OnSelect -= NavButton_OnSelect;
        navButton.OnSelectExt -= NavButton_OnSelectExt;
    }

    private void NavButton_OnSelect(ButtonStateData _buttonStateData) {
        int strikeState = FlagRepository.ReadSecretKey(notSecretData.secret.ToString());
        if (strikeState == 1) {
            isStricken = true;
            FlagRepository.SecretKeyStrike(notSecretData.secret.ToString());
            transform.SetAsLastSibling();
        } else {
            isStricken = false;
            FlagRepository.WriteSecretKey(notSecretData.secret.ToString(), 1);
        }
        notSecretsRef.SortListElementsUnderTransform();
        TextSetStricken();
    }

    private void NavButton_OnSelectExt(ButtonStateData _buttonStateData, object _data) {
        NavButton_OnSelect(_buttonStateData);
    }

    public void Unpack(NotSecretData nsData, NotSecrets _NotSecrets) {
        notSecretsRef = _NotSecrets;
        notSecretData = nsData;
        myText = nsData.text;
        isStricken = FlagRepository.ReadSecretKey(notSecretData.secret.ToString()) == 2; //2 is stricken
        TextSetStricken();
        myRect.sizeDelta = new Vector2(myRect.sizeDelta.x, text.preferredHeight);
    }

    public void TextSetStricken() {
        text.text = isStricken ? "<s>"+myText+"</s>" : myText;
    }
}