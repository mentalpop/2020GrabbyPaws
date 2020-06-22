using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNavigator : MonoBehaviour
{
    public MenuNode activeMenuNode;
    public MenuNode defaultMenuNode;
    public int indexHeld = -1;

    public delegate void MenuEvent(MenuNode menuNode);
    public event MenuEvent OnClose = delegate { };
    public event MenuEvent OnMenuFocus = delegate { };

    private NavButton activeButton;

    /*
    private void Awake() {
        if (defaultMenuNode != null) {
            MenuFocus(defaultMenuNode);
        }
    }
    //*/

    public void MenuCancel(MenuNode _mNode) {
        if (_mNode == null) {
            OnClose(activeMenuNode);
        } else {
            MenuFocus(_mNode);
        }
    }

    public void MenuFocus(MenuNode _mNode) {
        if (_mNode != null) {
            activeMenuNode.listController.Unfocus();
            activeMenuNode = _mNode;
            activeMenuNode.listController.Focus();
            OnMenuFocus(_mNode);
        }
    }

    public void MenuPress() {
        indexHeld = activeMenuNode.listController.focusIndex;
        activeButton.buttonStateData.inputPressed = true;
        activeButton.StateUpdate();
    }

    public void MenuRelease() {
        if (indexHeld == activeMenuNode.listController.focusIndex) {
            activeButton.buttonStateData.inputPressed = false;
            if (activeButton.buttonStateData.hasToggleState)
                activeButton.buttonStateData.stateActive = !activeButton.buttonStateData.stateActive;
            activeButton.Select();
        }
        indexHeld = -1;
    }

    public void MenuNavigate(MenuNode.NavDir navDir) {
        switch (navDir) {
            case MenuNode.NavDir.Left:
                switch (activeMenuNode.navigationType) {
                    case MenuNode.NavigationType.Horizontal:
                        if (!activeMenuNode.listController.DecrementIndex()) {
                            if (activeMenuNode.outOfBoundsLoop) {
                                activeMenuNode.listController.LastIndex();
                            } else {
                                MenuFocus(activeMenuNode.mLeft);
                            }
                        }
                        break;
                    case MenuNode.NavigationType.Vertical:
                        if (activeMenuNode.outOfBoundsLoop) {
                            activeMenuNode.listController.FirstIndex();
                        } else {
                            MenuFocus(activeMenuNode.mUp);
                        }
                        break;
                    case MenuNode.NavigationType.Grid:
                        
                        break;
                }
                break;
            case MenuNode.NavDir.Right:
                switch (activeMenuNode.navigationType) {
                    case MenuNode.NavigationType.Horizontal:
                        if (!activeMenuNode.listController.IncrementIndex()) {
                            if (activeMenuNode.outOfBoundsLoop) {
                                activeMenuNode.listController.FirstIndex();
                            } else {
                                MenuFocus(activeMenuNode.mRight);
                            }
                        }
                        break;
                    case MenuNode.NavigationType.Vertical:
                        if (activeMenuNode.outOfBoundsLoop) {
                            activeMenuNode.listController.LastIndex();
                        } else {
                            MenuFocus(activeMenuNode.mDown);
                        }
                        break;
                    case MenuNode.NavigationType.Grid:

                        break;
                }
                break;
            case MenuNode.NavDir.Up:
                switch (activeMenuNode.navigationType) {
                    case MenuNode.NavigationType.Horizontal:
                        if (activeMenuNode.outOfBoundsLoop) {
                            activeMenuNode.listController.FirstIndex();
                        } else {
                            MenuFocus(activeMenuNode.mUp);
                        }
                        break;
                    case MenuNode.NavigationType.Vertical:
                        if (!activeMenuNode.listController.DecrementIndex()) {
                            if (activeMenuNode.outOfBoundsLoop) {
                                activeMenuNode.listController.LastIndex();
                            } else {
                                MenuFocus(activeMenuNode.mUp);
                            }
                        }
                        break;
                    case MenuNode.NavigationType.Grid:

                        break;
                }
                break;
            case MenuNode.NavDir.Down:
                switch (activeMenuNode.navigationType) {
                    case MenuNode.NavigationType.Horizontal:
                        if (activeMenuNode.outOfBoundsLoop) {
                            activeMenuNode.listController.LastIndex();
                        } else {
                            MenuFocus(activeMenuNode.mDown);
                        }
                        break;
                    case MenuNode.NavigationType.Vertical:
                        if (!activeMenuNode.listController.IncrementIndex()) {
                            if (activeMenuNode.outOfBoundsLoop) {
                                activeMenuNode.listController.FirstIndex();
                            } else {
                                MenuFocus(activeMenuNode.mDown);
                            }
                        }
                        break;
                    case MenuNode.NavigationType.Grid:

                        break;
                }
                break;
            case MenuNode.NavDir.Forward: MenuFocus(activeMenuNode.mForward); break;
            case MenuNode.NavDir.Backward: MenuFocus(activeMenuNode.mBackward); break;
            case MenuNode.NavDir.Cancel: MenuCancel(activeMenuNode.mCancel); break;
        }
        activeButton = activeMenuNode.listController.elements[activeMenuNode.listController.focusIndex].navButton;
    }

    /*
    public bool IsActiveMenu(MenuNode _mCheck) {

    }
    //*/
}