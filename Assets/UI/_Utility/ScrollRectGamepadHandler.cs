using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollRectGamepadHandler : MonoBehaviour
{
    public ScrollRect scrollRect;
    public GameObject blockerPrefab;

    private GameObject blockerActive;

    private void Awake() {
        blockerActive = Instantiate(blockerPrefab, scrollRect.transform.parent, false); //Block the ScrollRect
    }

    private void OnEnable() {
        MenuNavigator.Instance.OnInputMethodSet += Instance_OnInputMethodSet;
        SetScrollRectInteractive(MenuNavigator.MouseIsUsing());
    }

    private void OnDisable() {
        MenuNavigator.Instance.OnInputMethodSet -= Instance_OnInputMethodSet;
    }

    private void Instance_OnInputMethodSet(bool isUsingMouse) {
        SetScrollRectInteractive(isUsingMouse);
    }

    private void SetScrollRectInteractive(bool _isUsingMouse) {
        blockerActive.SetActive(!_isUsingMouse);
    }
}