using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplayMenuNodeList : MenuNodeList
{
    public InventoryDisplay InventoryDisplay;

    public override void MenuNavigate(NavDir navDir) {
        //Debug.Log("MenuNavigate: "+name);
        MenuNode _mNode = null;
        InventoryTab _tab;
        switch (navDir) {
            case NavDir.Accept: _mNode = mAccept; break;
            case NavDir.Cancel: MenuNavigator.Instance.MenuCancel(mCancel); break;
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
                ListElement _li;
            //If you are navigating upwards, check the index of the list element one less than the current index
                if (listController.focusIndex == 0) {
                    _li = listController.Elements[listController.Elements.Count - 1]; //Unless it's element zero, in which case, check the last one in the list
                } else {
                    _li = listController.Elements[listController.focusIndex - 1];
                }
                _tab = _li as InventoryTab;
                //Debug.Log("_tab.inventoryScrollRect: " + _tab.inventoryScrollRect);
                if (_tab.inventoryScrollRect == InventoryDisplay.inventoryScrollRect && _tab.inventoryScrollRect.scrollResize.Open) {
                    _mNode = _tab.GetComponent<InventoryTabMenuNodeList>();
                    _tab.listController.LastIndex(); //Going upwards, focus the LAST element in the ScrollRect
                    Debug.Log("_tab.listController.focusIndex: " + _tab.listController.focusIndex);
                    _mNode.mCancel = this;
                //Put the correct Tab in focus
                    if (listController.focusIndex == 0) { //If you're going UP from the FIRST index, select the last
                        listController.LastIndex();
                    } else {
                        listController.DecrementIndex();
                    }
                } else {
                    if (!listController.DecrementIndex()) {
                        if (outOfBoundsLoop) {
                            listController.LastIndex();
                        } else {
                            _mNode = mUp;
                        }
                    }
                }
                break;
            case NavDir.Down:
                _tab = listController.FocusElement as InventoryTab;
                if (_tab.inventoryScrollRect == InventoryDisplay.inventoryScrollRect && _tab.inventoryScrollRect.scrollResize.Open) {
                    _mNode = _tab.GetComponent<InventoryTabMenuNodeList>();
                    _tab.listController.FirstIndex(); //Going downwards, focus the FIRST element in the ScrollRect
                    Debug.Log("_tab.listController.focusIndex: " + _tab.listController.focusIndex);
                    _mNode.mCancel = this;
                } else {
                    if (!listController.IncrementIndex()) {
                        if (outOfBoundsLoop) {
                            listController.FirstIndex();
                        } else {
                            _mNode = mDown;
                        }
                    }
                }
                break;
            case NavDir.Forward: _mNode = mForward; break;
            case NavDir.Backward: _mNode = mBackward; break;
        }
        //Debug.Log("_mNode.validSelection: "+_mNode.validSelection);
        if (_mNode != null && _mNode.validSelection) {
            MenuNavigator.Instance.MenuFocus(_mNode);
        }
    }
}