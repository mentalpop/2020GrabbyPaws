using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuListNode : MenuNode
{
    public ListController listController;
    public NavigationType navigationType;
    public Vector2Int menuSize;
    public bool outOfBoundsLoop = false;
    public enum NavigationType
    {
        Vertical,
        Horizontal,
        Grid
    }

    public override void MenuUnfocus() {
        listController.Unfocus();
    }

    public override void MenuFocus() {
        listController.Focus();
    }

    public override void MenuNavigate(NavDir navDir, MenuNavigator menuNavigator) {
        switch (navDir) {
            case NavDir.Accept: menuNavigator.MenuFocus(mAccept); break;
            case NavDir.Cancel: menuNavigator.MenuCancel(mCancel); break;
            case NavDir.Left:
                switch (navigationType) {
                    case NavigationType.Horizontal:
                        if (!listController.DecrementIndex()) {
                            if (outOfBoundsLoop) {
                                listController.LastIndex();
                            } else {
                                menuNavigator.MenuFocus(mLeft);
                            }
                        }
                        break;
                    case NavigationType.Vertical:
                        if (outOfBoundsLoop) {
                            listController.FirstIndex();
                        } else {
                            menuNavigator.MenuFocus(mUp);
                        }
                        break;
                    case NavigationType.Grid:
                        
                        break;
                }
                break;
            case NavDir.Right:
                switch (navigationType) {
                    case NavigationType.Horizontal:
                        if (!listController.IncrementIndex()) {
                            if (outOfBoundsLoop) {
                                listController.FirstIndex();
                            } else {
                                menuNavigator.MenuFocus(mRight);
                            }
                        }
                        break;
                    case NavigationType.Vertical:
                        if (outOfBoundsLoop) {
                            listController.LastIndex();
                        } else {
                            menuNavigator.MenuFocus(mDown);
                        }
                        break;
                    case NavigationType.Grid:

                        break;
                }
                break;
            case NavDir.Up:
                switch (navigationType) {
                    case NavigationType.Horizontal:
                        if (outOfBoundsLoop) {
                            listController.FirstIndex();
                        } else {
                            menuNavigator.MenuFocus(mUp);
                        }
                        break;
                    case NavigationType.Vertical:
                        if (!listController.DecrementIndex()) {
                            if (outOfBoundsLoop) {
                                listController.LastIndex();
                            } else {
                                menuNavigator.MenuFocus(mUp);
                            }
                        }
                        break;
                    case NavigationType.Grid:

                        break;
                }
                break;
            case NavDir.Down:
                switch (navigationType) {
                    case NavigationType.Horizontal:
                        if (outOfBoundsLoop) {
                            listController.LastIndex();
                        } else {
                            menuNavigator.MenuFocus(mDown);
                        }
                        break;
                    case NavigationType.Vertical:
                        if (!listController.IncrementIndex()) {
                            if (outOfBoundsLoop) {
                                listController.FirstIndex();
                            } else {
                                menuNavigator.MenuFocus(mDown);
                            }
                        }
                        break;
                    case NavigationType.Grid:

                        break;
                }
                break;
            case NavDir.Forward: menuNavigator.MenuFocus(mForward); break;
            case NavDir.Backward: menuNavigator.MenuFocus(mBackward); break;
        }
    }

    public override NavButton GetButtonInFocus() {
        return listController.elements[listController.focusIndex].navButton;
    }
}