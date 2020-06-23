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
            activeMenuNode.MenuUnfocus();
            activeMenuNode = _mNode;
            activeMenuNode.MenuFocus();
            OnMenuFocus(_mNode);
        }
    }

    public void MenuPress() {
        heldButton = activeButton;//activeMenuNode.listController.focusIndex;
        activeButton.buttonStateData.inputPressed = true;
        activeButton.StateUpdate();
    }

    public void MenuRelease() {
        if (activeButton == heldButton) {
            activeButton.buttonStateData.inputPressed = false;
            if (activeButton.buttonStateData.hasToggleState)
                activeButton.buttonStateData.stateActive = !activeButton.buttonStateData.stateActive;
            activeButton.Select();
        }
        heldButton = null;
    }

    public void MenuNavigate(MenuNode.NavDir navDir) {
        activeMenuNode.MenuNavigate(navDir, this);
        activeButton = activeMenuNode.GetButtonInFocus();
    }

    /*
    public bool IsActiveMenu(MenuNode _mCheck) {

    }
    //*/
}