using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControllerPrompt : MonoBehaviour
{
    public Image myImage;
    public TextMeshProUGUI tmpTitle;

    [Header("Animation Effect")]
    public BTween btween;
    public BTweenCanvasGroup canvasFadeOut;
    public BTweenRectAnchor rectFadeIn;
    public float secondsRemainOpen = 2f;

    private bool closing = false;
    private WaitForSeconds shortWait;

    private void Awake() {
        shortWait = new WaitForSeconds(secondsRemainOpen);
    }

    private void OnEnable() {
        btween.OnEndTween += Btween_OnEndTween;
    }

    private void OnDisable() {
        btween.OnEndTween -= Btween_OnEndTween;
    }

    public void Unpack(Sprite _sprite, string _text) {
        myImage.sprite = _sprite;
        myImage.SetNativeSize();
        tmpTitle.text = _text;
    //Effect
        btween.PlayFromZero();
        StartCoroutine(CloseAfterDelay());
        //transform.localScale = Vector3.one; //Something changes the z-depth to zero, so this has to compensate
    }


    private IEnumerator CloseAfterDelay() {
        yield return shortWait;
        Close(); //This could be called early
    }

    public void Close() {
        if (!closing) {
            closing = true;
            canvasFadeOut.enabled = true; //Only enable for fade-out
            //scaleDown.enabled = true; //Only enable for fade-out
            rectFadeIn.enabled = false; //Disable for fade-out
            btween.PlayFromEnd();
        }
    }

    private void Btween_OnEndTween(float value) {
        if (closing) {
            Destroy(gameObject);
        }
    }
}