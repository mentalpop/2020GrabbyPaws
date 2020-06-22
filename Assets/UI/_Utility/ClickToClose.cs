using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class ClickToClose : MonoBehaviour, IPointerClickHandler
{
    public delegate void CloseEvent();
    public CloseEvent OnClick;

    private void OnEnable() {
//Center and fill screen
        RectTransform myRect = GetComponent<RectTransform>();
        myRect.pivot = new Vector2(0.5f, 0.5f);
        myRect.sizeDelta = new Vector2(ScreenSpace.Width, ScreenSpace.Height);
        transform.position = Vector2.zero;
    }

    public void OnPointerClick (PointerEventData evd) {
        //Debug.Log("OnPointerClick");
		ManualClose();
	}

    public void ManualClose() {
        OnClick?.Invoke();
    }
}