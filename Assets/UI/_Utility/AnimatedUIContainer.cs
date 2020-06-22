using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedUIContainer : MonoBehaviour
{
    public RectTransform myRect;
    public float direction = 90f;

    public delegate void EffectComplete(bool reverse);
    public event EffectComplete OnEffectComplete = delegate { };

    [HideInInspector] public GTween gTween;

    private Vector2 originalAnchoredPosition;
    private Vector2 offScreenPosition;
    private Vector2 vDirection;

    private void Awake() {
        gTween = new GTween(0.3f, 0f, 1f, false);
        originalAnchoredPosition = myRect.anchoredPosition;
        vDirection = (Quaternion.Euler(0,0,direction) * Vector2.right); //Convert the supplied angle measure into a Vector2
        //Debug.Log("vDirection: "+vDirection);
        offScreenPosition = new Vector2(originalAnchoredPosition.x - myRect.sizeDelta.x * vDirection.x, originalAnchoredPosition.y - myRect.sizeDelta.y * vDirection.y);
        myRect.anchoredPosition = offScreenPosition;
    }

    private void OnEnable() {
        gTween.Reset();
    }

    private void OnDisable() {
        myRect.anchoredPosition = offScreenPosition;
    }

    private void Update() {
        if (gTween.effectActive) {
            float tweenVal = gTween.DoTween();
            if (gTween.effectActive) {
                myRect.anchoredPosition = new Vector2(originalAnchoredPosition.x - myRect.sizeDelta.x * vDirection.x + myRect.sizeDelta.x * tweenVal * vDirection.x,
                    originalAnchoredPosition.y - myRect.sizeDelta.y * vDirection.y + myRect.sizeDelta.y * tweenVal * vDirection.y);
            } else {
                if (gTween.doReverse) {

                } else {
                    myRect.anchoredPosition = originalAnchoredPosition;
                }
                OnEffectComplete.Invoke(gTween.doReverse);
            }
        }
    }
}