using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavButtonAutoScroller : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float scrollRate = 0.25f;
    public List<NavButton> navButtons = new List<NavButton>();

    private GTween gTween;
    private float targetPosition = 0f;
    private Coroutine runningCoroutine;
    private RectTransform rectSnapTo;
    private int navButtonIndex = 0;

    private void Awake() {
        gTween = new GTween(scrollRate, 0f, 1f, false);
    }

    private void OnEnable() {
        foreach (var navButton in navButtons) {
            navButton.OnFocusGain += NavButton_OnFocusGain;
        }
    }

    private void NavButton_OnFocusGain(ButtonStateData _buttonStateData) {
        if (scrollRect != null && !MenuNavigator.MouseIsUsing()) {
    //Deduce rectSnapTo
            int i = 0;
            foreach (var navButton in navButtons) {
                if (navButton.buttonStateData.hasFocus) {
                    navButtonIndex = i;
                    rectSnapTo = navButton.GetComponent<RectTransform>();
                    break;
                }
                i++;
            }
            runningCoroutine = StartCoroutine(SnapInOneFrame());
        }
    }

    private void OnDisable() {
        foreach (var navButton in navButtons) {
            navButton.OnFocusGain -= NavButton_OnFocusGain;
        }
        if (runningCoroutine != null) {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }
    }

    private IEnumerator SnapInOneFrame() {
        yield return null;
        SnapTo(rectSnapTo);
        runningCoroutine = null;
    }

    public void SnapTo(RectTransform target) {
        Canvas.ForceUpdateCanvases();
        float _unclampedPosition = 0f; //The "targetPosition" will be the summed height of all elements that come before this list element
        for (int i = 0; i < navButtonIndex; i++) {
            float _add = scrollRect.content.transform.GetChild(i).GetComponent<RectTransform>().rect.height;
            _unclampedPosition += _add;
        }
    //Clamp upper limit is based on the delta between the Viewport (container) and the height of the content rect, but it shouldn't be less than 0
        //Debug.Log("scrollRect.content.rect.height: "+scrollRect.content.rect.height);
        //Debug.Log("scrollRect.viewport.rect.height: "+scrollRect.viewport.rect.height);
        targetPosition = Mathf.Clamp(_unclampedPosition, 0f, Mathf.Max(0f, scrollRect.content.rect.height - scrollRect.viewport.rect.height));
        gTween.Reset();
    }

    private void Update() {
        if (gTween.effectActive) {
            float tweenVal = gTween.DoTween();
            if (gTween.effectActive) {
                float _delta = Mathf.Lerp(scrollRect.content.anchoredPosition.y, targetPosition, tweenVal);
                //Debug.Log("_delta: "+_delta);
                scrollRect.content.anchoredPosition = new Vector2(scrollRect.content.anchoredPosition.x, _delta); //Shift the content
            } else {
                scrollRect.content.anchoredPosition = new Vector2(scrollRect.content.anchoredPosition.x, targetPosition);
            }
        }
    }
}