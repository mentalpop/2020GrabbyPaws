using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtonNode : MenuNode
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
        switch (navDir) {
            case NavDir.Accept: menuNavigator.MenuFocus(mAccept); break;
            case NavDir.Cancel: menuNavigator.MenuCancel(mCancel); break;
            case NavDir.Left: menuNavigator.MenuFocus(mLeft); break;
            case NavDir.Right: menuNavigator.MenuFocus(mRight); break;
            case NavDir.Up: menuNavigator.MenuFocus(mUp); break;
            case NavDir.Down: menuNavigator.MenuFocus(mDown); break;
            case NavDir.Forward: menuNavigator.MenuFocus(mForward); break;
            case NavDir.Backward: menuNavigator.MenuFocus(mBackward); break;
        }
    }
}