using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNodeRaccoonProfile : MenuNode
{
    public ListController listController;
    public bool outOfBoundsLoop = true;
    private void OnEnable() {
        listController.OnListEmpty += ListController_OnListEmpty;
    }

    private void OnDisable() {
        listController.OnListEmpty -= ListController_OnListEmpty;
    }

    private void ListController_OnListEmpty(bool _isEmpty) {
        if (_isEmpty) {
            SelectionAbort(mCancel);
            validSelection = false;
        } else {
            validSelection = true;
        }
    }

    public override void MenuUnfocus() {
        listController.Unfocus();
    }

    public override void MenuFocus() {
        listController.Focus();
    }

    public override void MenuNavigate(NavDir navDir) {
        //Debug.Log("MenuNavigate: "+name);
        MenuNode _mNode = null;
        switch (navDir) {
            case NavDir.Accept: _mNode = mAccept; break;
            case NavDir.Cancel: MenuNavigator.Instance.MenuCancel(mCancel); break;
            case NavDir.Left:
                if (!listController.DecrementIndex()) {
                    if (outOfBoundsLoop) {
                        listController.LastIndex();
                    } else {
                //"Cancel" out of this list
                        MenuNavigator.Instance.MenuCancel(mCancel); //_mNode = mLeft;
                    }
                }
                break;
            case NavDir.Right:
                if (!listController.IncrementIndex()) {
                    if (outOfBoundsLoop) {
                        listController.FirstIndex();
                    } else {
                        _mNode = mRight;
                    }
                }
                break;
            case NavDir.Up: MenuNavigator.Instance.MenuCancel(mCancel);
                /*
                if (outOfBoundsLoop) {
                    listController.FirstIndex();
                } else {
                    _mNode = mUp;
                }
                //*/
                break;
            case NavDir.Down: MenuNavigator.Instance.MenuCancel(mCancel);
                /*
                if (outOfBoundsLoop) {
                    listController.LastIndex();
                } else {
                    _mNode = mDown;
                }
                //*/
                break;
            case NavDir.Forward: _mNode = mForward; break;
            case NavDir.Backward: _mNode = mBackward; break;
        }
        //Debug.Log("_mNode.validSelection: "+_mNode.validSelection);
        if (_mNode != null && _mNode.validSelection) {
            MenuNavigator.Instance.MenuFocus(_mNode);
        }
    }

    public override NavButton GetButtonInFocus() {
        return listController.Elements[listController.focusIndex].navButton;
    }
}