using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;

public class NotSecretLineItem : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI text;
    public RectTransform myRect;

    private NotSecretData notSecretData;
    //private NotSecrets notSecretsRef;
    [HideInInspector] public bool isStricken;
    private string myText;

    public void Unpack(NotSecretData nsData/*, NotSecrets _NotSecrets*/) {
        //notSecretsRef = _NotSecrets;
        notSecretData = nsData;
        myText = nsData.text;
        isStricken = FlagRepository.ReadSecretKey(notSecretData.secret.ToString()) == 2; //2 is stricken
        TextSetStricken();
        myRect.sizeDelta = new Vector2(myRect.sizeDelta.x, text.preferredHeight);
    }

    public void OnPointerClick(PointerEventData eventData) {
        int strikeState = FlagRepository.ReadSecretKey(notSecretData.secret.ToString());
        if (strikeState == 1) {
            isStricken = true;
            FlagRepository.SecretKeyStrike(notSecretData.secret.ToString());
            transform.SetAsLastSibling();
        } else {
            isStricken = false;
            FlagRepository.WriteSecretKey(notSecretData.secret.ToString(), 1);
        }
        TextSetStricken();
    }

    public void TextSetStricken() {
        text.text = isStricken ? "<s>"+myText+"</s>" : myText;
    }
}