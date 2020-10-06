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

    private void Awake() {
        if (rememberLastMenu && menuOnEnable != null) {
            menuOnEnableOriginal = menuOnEnable;
        }
    }

    private void OnEnable() {
        if (menuOnEnable != null) {
            //Debug.Log("menuOnEnable: "+menuOnEnable);
            MenuNavigator.Instance.MenuFocus(menuOnEnable);
        }
        MenuNavigator.Instance.OnClose += MenuNavigator_OnClose;
    }

    private void OnDisable() {
        if (rememberLastMenu) {
            menuOnEnable = menuOnEnableOriginal; //Fallback
            //Debug.Log("MenuNavigator.Instance.activeMenuNode: "+MenuNavigator.Instance.activeMenuNode);
    //Try to remember the last menu the user had active if it's one of the menu's primary "Nodes"
            foreach (var menu in nodes) {
                //Debug.Log("Checking menu: "+menu);
                if (menu == MenuNavigator.Instance.activeMenuNode) {
                    menuOnEnable = menu;
                }
            }
        }
        if (menuOnDisable != null)
            MenuNavigator.Instance.MenuFocus(menuOnDisable);
        MenuNavigator.Instance.OnClose -= MenuNavigator_OnClose;
    }

    private void MenuNavigator_OnClose(MenuNode menuNode) {
        foreach (var node in nodes) {
            if (menuNode == node) {
                OnMenuClose();
                break;
            }
        }
    }
}