using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuNodeSlider : MenuNode
{
    public Slider slider;
    public float sliderDelta = 0.1f;

    private float valueOriginal = 0f;

    private void Awake() {
        if (mAccept != null) {
    //Inherit directions
            mUp = mAccept.mUp;
            mDown = mAccept.mDown;
            mForward = mAccept.mForward;
            mBackward = mAccept.mBackward;
        }
    }
    /*
    private void OnEnable() {
        navButton.OnSelect += NavButton_OnSelect;
    }

    private void OnDisable() {
        navButton.OnSelect -= NavButton_OnSelect;
    }

    private void NavButton_OnSelect(ButtonStateData _buttonStateData) {
        if (mAccept != null) {
            MenuNavigator.Instance.MenuNavigate(NavDir.Accept);
        }
    }
    public override void MenuUnfocus() {
    }
    //*/


    public override void MenuFocus() {
        valueOriginal = slider.value;
        slider.Select();
    }

    public override void MenuNavigate(NavDir navDir) {
        //Debug.Log("navDir: "+navDir);
        MenuNode _mNode = null;
        switch (navDir) {
            case NavDir.Accept: _mNode = mAccept; break;
            case NavDir.Cancel:
        //Reset the value
                slider.value = valueOriginal;
                MenuNavigator.Instance.MenuCancel(mCancel);
                break;
            case NavDir.Left: slider.value -= sliderDelta; break;
            case NavDir.Right: slider.value += sliderDelta; break;
            case NavDir.Up: _mNode = mUp; break;
            case NavDir.Down: _mNode = mDown; break;
            case NavDir.Forward: _mNode = mForward; break;
            case NavDir.Backward: _mNode = mBackward; break;
        }
        //Debug.Log("_mNode: "+_mNode);
        if (_mNode != null && _mNode.validSelection)
            MenuNavigator.Instance.MenuFocus(_mNode);
        else {
            Debug.Log("_mNode is null");
        }
    }
}