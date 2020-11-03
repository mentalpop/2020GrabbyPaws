using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHub : MonoBehaviour
{
	public MenuNode menuOnEnable;
    public bool rememberLastMenu = false;
	public MenuNode menuOnDisable;
    public List<MenuNode> nodes = new List<MenuNode>();

    public delegate void MenuCloseEvent();
    public event MenuCloseEvent OnMenuClose = delegate { };

    private MenuNode menuOnEnableOriginal;
    private MenuNavigator menuNavigator;

    private void Awake() {
        menuNavigator = MenuNavigator.Instance;
        if (rememberLastMenu && menuOnEnable != null) {
            menuOnEnableOriginal = menuOnEnable;
        }
    }

    private void OnEnable() {
        MenuHubOnEnable();
        menuNavigator.OnClose += MenuNavigator_OnClose;
        menuNavigator.OnInputMethodSet += MenuNavigator_OnInputMethodSet;
    }

    private void OnDisable() {
        if (rememberLastMenu) {
            menuOnEnable = menuOnEnableOriginal; //Fallback
            //Debug.Log("MenuNavigator.Instance.activeMenuNode: "+MenuNavigator.Instance.activeMenuNode);
    //Try to remember the last menu the user had active if it's one of the menu's primary "Nodes"
            foreach (var menu in nodes) {
                //Debug.Log("Checking menu: "+menu);
                if (menu == menuNavigator.activeMenuNode) {
                    menuOnEnable = menu;
                }
            }
        }
        if (menuOnDisable != null)
            menuNavigator.MenuFocus(menuOnDisable);
        menuNavigator.OnClose -= MenuNavigator_OnClose;
        menuNavigator.OnInputMethodSet -= MenuNavigator_OnInputMethodSet;
    }

    public void MenuHubOnEnable() {
        if (menuOnEnable != null && !MenuNavigator.MouseIsUsing()) {
            //Debug.Log("menuOnEnable: "+menuOnEnable);
            menuNavigator.MenuFocus(menuOnEnable);
        }
    }

    private void MenuNavigator_OnClose(MenuNode menuNode) {
        foreach (var node in nodes) {
            if (menuNode == node) {
                OnMenuClose();
                break;
            }
        }
    }

    private void MenuNavigator_OnInputMethodSet(bool isUsingMouse) {
        if (!isUsingMouse) {
    //Switched to Gamepad
            MenuHubOnEnable();
        }
    }
}