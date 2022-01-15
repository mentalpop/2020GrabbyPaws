using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControllerPrompt : MonoBehaviour
{
    public Image myImage;
    public TextMeshProUGUI tmpTitle;

    public void Unpack(Sprite _sprite, string _text) {
        myImage.sprite = _sprite;
        myImage.SetNativeSize();
        tmpTitle.text = _text;
        StartCoroutine(DestroyAfterWait());
    }

    private IEnumerator DestroyAfterWait() {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}