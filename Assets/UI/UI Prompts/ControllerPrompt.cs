using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerPrompt : MonoBehaviour
{
    public Image myImage;

    public void Unpack(Sprite _sprite) {
        myImage.sprite = _sprite;
        myImage.SetNativeSize();
        StartCoroutine(DestroyAfterWait());
    }

    private IEnumerator DestroyAfterWait() {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}