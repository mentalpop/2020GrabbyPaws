﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNodeSneakDiaryList : MenuNodeList
{
    public override void MenuNavigate(NavDir navDir) {
        //Debug.Log("MenuNavigate: "+name);
        MenuNode _mNode = null;
        switch (navDir) {
            case NavDir.Accept: _mNode = mAccept; break;
            case NavDir.Cancel: MenuNavigator.Instance.MenuCancel(mCancel); break;
            case NavDir.Left:
        //Jump to the last index of the Xs list
                /*
                if (outOfBoundsLoop) {
                    listController.FirstIndex();
                } else {
                    _mNode = mUp;
                }
                //*/
                break;
            case NavDir.Right: listController.SetActiveIndex(listController.activeIndex); break; //Jump into the Xs list
            case NavDir.Up:
                if (!listController.DecrementIndex()) {
                    if (outOfBoundsLoop) {
                        listController.LastIndex();
                    } else {
                        _mNode = mUp;
                    }
                }
                break;
            case NavDir.Down:
                if (!listController.IncrementIndex()) {
                    if (outOfBoundsLoop) {
                        listController.FirstIndex();
                    } else {
                        _mNode = mDown;
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