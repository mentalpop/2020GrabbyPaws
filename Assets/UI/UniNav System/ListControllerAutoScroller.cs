using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListControllerAutoScroller : MonoBehaviour
{
    public ListController listController;
    public ScrollRect scrollRect;
    public float scrollRate = 0.25f;

    [HideInInspector] public GTween gTween;

    private float targetPosition = 0f;
    private int indexToSnapTo = 0;
    private Coroutine runningCoroutine;

    private void Awake() {
        gTween = new GTween(scrollRate, 0f, 1f, false);
    }

    private void OnEnable() {
        listController.OnFocus += ListController_OnFocus;
    }

    private void OnDisable() {
        listController.OnFocus -= ListController_OnFocus;
        if (runningCoroutine != null) {
            StopCoroutine(runningCoroutine);
            runningCoroutine = null;
        }
    }

    private void ListController_OnFocus(int index) {
        if (scrollRect != null) {
            indexToSnapTo = index;
            runningCoroutine = StartCoroutine(SnapInOneFrame());
        }
    }

    private IEnumerator SnapInOneFrame() {
        yield return null;
        SnapTo(listController.Elements[indexToSnapTo].GetComponent<RectTransform>());
        runningCoroutine = null;
    }

    public void SnapTo(RectTransform target) {
        Canvas.ForceUpdateCanvases();
        float _unclampedPosition = 0f; //The "targetPosition" will be the summed height of all elements that come before this list element
        for (int i = 0; i < target.GetSiblingIndex(); i++) {
            _unclampedPosition += scrollRect.content.transform.GetChild(i).GetComponent<RectTransform>().rect.height;
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