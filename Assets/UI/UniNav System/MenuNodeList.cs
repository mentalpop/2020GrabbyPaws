using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNodeList : MenuNode
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

    public override void MenuNavigate(NavDir navDir, MenuNavigator menuNavigator) {
        MenuNode _mNode = null;
        switch (navDir) {
            case NavDir.Accept: _mNode = mAccept; break;
            case NavDir.Cancel: menuNavigator.MenuCancel(mCancel); break;
            case NavDir.Left:
                switch (navigationType) {
                    case NavigationType.Horizontal:
                        if (!listController.DecrementIndex()) {
                            if (outOfBoundsLoop) {
                                listController.LastIndex();
                            } else {
                                _mNode = mLeft;
                            }
                        }
                        break;
                    case NavigationType.Vertical:
                        if (outOfBoundsLoop) {
                            listController.FirstIndex();
                        } else {
                            _mNode = mUp;
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
                                _mNode = mRight;
                            }
                        }
                        break;
                    case NavigationType.Vertical:
                        if (outOfBoundsLoop) {
                            listController.LastIndex();
                        } else {
                            _mNode = mDown;
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
                            _mNode = mUp;
                        }
                        break;
                    case NavigationType.Vertical:
                        if (!listController.DecrementIndex()) {
                            if (outOfBoundsLoop) {
                                listController.LastIndex();
                            } else {
                                _mNode = mUp;
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
                            _mNode = mDown;
                        }
                        break;
                    case NavigationType.Vertical:
                        if (!listController.IncrementIndex()) {
                            if (outOfBoundsLoop) {
                                listController.FirstIndex();
                            } else {
                                _mNode = mDown;
                            }
                        }
                        break;
                    case NavigationType.Grid:

                        break;
                }
                break;
            case NavDir.Forward: _mNode = mForward; break;
            case NavDir.Backward: _mNode = mBackward; break;
        }
        //Debug.Log("_mNode.validSelection: "+_mNode.validSelection);
        if (_mNode != null && _mNode.validSelection) {
            menuNavigator.MenuFocus(_mNode);
        }
    }

    public override NavButton GetButtonInFocus() {
        return listController.Elements[listController.focusIndex].navButton;
    }
}