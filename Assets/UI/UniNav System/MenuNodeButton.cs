using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNodeButton : MenuNode
{
    public NavButton navButton;

    public override void MenuUnfocus() {
        navButton.SetFocus(false);
    }

    public override void MenuFocus() {
        navButton.SetFocus(true);
    }

    public override NavButton GetButtonInFocus() {
        return navButton;
    }

    public override void MenuNavigate(NavDir navDir, MenuNavigator menuNavigator) {
        MenuNode _mNode = null;
        switch (navDir) {
            case NavDir.Accept: _mNode = mAccept; break;
            case NavDir.Cancel: menuNavigator.MenuCancel(mCancel); break;
            case NavDir.Left: _mNode = mLeft; break;
            case NavDir.Right: _mNode = mRight; break;
            case NavDir.Up: _mNode = mUp; break;
            case NavDir.Down: _mNode = mDown; break;
            case NavDir.Forward: _mNode = mForward; break;
            case NavDir.Backward: _mNode = mBackward; break;
        }
        if (_mNode != null && _mNode.validSelection)
            menuNavigator.MenuFocus(_mNode);
    }
}