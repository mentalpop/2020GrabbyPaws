using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNavigator : MonoBehaviour
{
    public MenuNode activeMenuNode;
    //public MenuNode defaultMenuNode;

    public delegate void MenuEvent(MenuNode menuNode);
    public event MenuEvent OnClose = delegate { };
    public event MenuEvent OnMenuFocus = delegate { };

    protected NavButton activeButton;
    protected NavButton heldButton;

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
            if (activeMenuNode != null) {
                activeMenuNode.OnSelectionAbort -= ActiveMenuNode_OnSelectionAbort;
                activeMenuNode.MenuUnfocus();
            }
            activeMenuNode = _mNode;
            activeMenuNode.MenuFocus();
            activeMenuNode.OnSelectionAbort += ActiveMenuNode_OnSelectionAbort;
            OnMenuFocus(_mNode);
        }
    }

    private void ActiveMenuNode_OnSelectionAbort(MenuNode _fallbackMenu) {
        MenuFocus(_fallbackMenu);
    }

    public void MenuPress() {
        activeButton = activeMenuNode.GetButtonInFocus();
        heldButton = activeButton;//activeMenuNode.listController.focusIndex;
        activeButton.buttonStateData.inputPressed = true;
        activeButton.StateUpdate();
    }

    public void MenuRelease() {
        activeButton = activeMenuNode.GetButtonInFocus();
        if (activeButton != null && activeButton == heldButton) {
            activeButton.buttonStateData.inputPressed = false;
            if (activeMenuNode.mAccept == null) {
                if (activeButton.buttonStateData.hasToggleState)
                    activeButton.buttonStateData.stateActive = !activeButton.buttonStateData.stateActive;
                activeButton.Select();
            } else {
                MenuNavigate(MenuNode.NavDir.Accept);
                activeButton.StateUpdate();
            }
        }
        heldButton = null;
    }

    public void MenuNavigate(MenuNode.NavDir navDir) {
        activeMenuNode.MenuNavigate(navDir, this);
    }

    /*
    public bool IsActiveMenu(MenuNode _mCheck) {

    }
    //*/
}