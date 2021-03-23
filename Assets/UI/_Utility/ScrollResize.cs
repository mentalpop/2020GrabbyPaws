using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollResize : MonoBehaviour
{
    public RectTransform myRect;
    public ScrollRect scrollRect;
    public Transform countChildTransform;
    public int maxChildrenOnScreen = 9;
    public float heightPerChild = 128;
    public bool Open => targetHeight > 0f;

    private GTween gTween;
    private float targetHeight = 0f;

    public delegate void ScrollResizeEvent();
    public event ScrollResizeEvent OnClose = delegate { };

    private void Awake() {
        gTween = new GTween(0.3f, 0f, 1f, false);
    }

    private void Update() {
        if (gTween.effectActive) {
            float tweenVal = gTween.DoTween();
            if (gTween.effectActive) {
                myRect.sizeDelta = new Vector2(myRect.rect.width, targetHeight * tweenVal);
            } else {
                if (gTween.doReverse) {
                    OnClose();
                    targetHeight = 0f;
                }
                myRect.sizeDelta = new Vector2(myRect.rect.width, targetHeight);
            }
        }
    }

    public void RectResize(int childCount) {
        float _oldtargetHeight = targetHeight;
        int numToResize = Mathf.Min(maxChildrenOnScreen, childCount);
        targetHeight = heightPerChild * numToResize;
        if (targetHeight != _oldtargetHeight) //Only resize if necessary
            gTween.Reset();
    }

    public void Collapse() {
        gTween.Reverse();
    }
}