using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNodeButton : MenuNode
{
    public NavButton navButton;

    private void OnEnable() {
        navButton.OnSelect += NavButton_OnSelect;
        navButton.OnSelectExt += NavButton_OnSelectExt;
    }

    private void OnDisable() {
        navButton.OnSelect -= NavButton_OnSelect;
        navButton.OnSelectExt -= NavButton_OnSelectExt;
    }

    public override void MenuUnfocus() {
        navButton.SetFocus(false);
    }

    public override void MenuFocus() {
        navButton.SetFocus(true);
    }

    public override NavButton GetButtonInFocus() {
        return navButton;
    }

    private void NavButton_OnSelect(ButtonStateData _buttonStateData) {
        if (mAccept != null) {
            MenuNavigator.Instance.MenuNavigate(NavDir.Accept);
        }
    }

    private void NavButton_OnSelectExt(ButtonStateData _buttonStateData, object _data) {
        NavButton_OnSelect(_buttonStateData);
    }

    public override void MenuNavigate(NavDir navDir) {
        MenuNode _mNode = null;
        switch (navDir) {
            case NavDir.Accept: _mNode = mAccept; break;
            case NavDir.Cancel: MenuNavigator.Instance.MenuCancel(mCancel); break;
            case NavDir.Left: _mNode = mLeft; break;
            case NavDir.Right: _mNode = mRight; break;
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