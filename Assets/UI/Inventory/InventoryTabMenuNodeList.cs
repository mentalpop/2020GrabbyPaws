using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryTabMenuNodeList : MenuNodeList
{
    [HideInInspector] public InventoryDisplay InventoryDisplay;

    public override void MenuNavigate(NavDir navDir) {
        //Debug.Log("MenuNavigate: "+name);
        MenuNode _mNode = null;
        switch (navDir) {
            case NavDir.Accept: _mNode = mAccept; break;
            case NavDir.Cancel:
                InventoryDisplay.inventoryScrollRect.scrollResize.Collapse(); //Close the Pocket when pressing "back"
                MenuNavigator.Instance.MenuCancel(mCancel);
                break;
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
                if (!listController.DecrementIndex()) {
                    _mNode = mUp;
                }
                break;
            case NavDir.Down:
                if (!listController.IncrementIndex()) {
                    _mNode = mDown;
                //Find the List Controller on this menu
                    MenuNodeList _nList = _mNode as MenuNodeList;
                //Increment the menu node or jump to the first index
                    bool success = _nList.listController.IncrementIndex();
                    if (!success) {
                        _nList.listController.FirstIndex();
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