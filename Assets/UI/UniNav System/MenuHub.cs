using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHub : MonoBehaviour
{
	public MenuNode menuOnEnable;
	public MenuNode menuOnDisable;
    public List<MenuNode> nodes = new List<MenuNode>();

    public delegate void MenuCloseEvent();
    public event MenuCloseEvent OnMenuClose = delegate { };

    private void OnEnable() {
        if (menuOnEnable != null) {
            //Debug.Log("menuOnEnable: "+menuOnEnable);
            MenuNavigator.Instance.MenuFocus(menuOnEnable);
        }
        MenuNavigator.Instance.OnClose += MenuNavigator_OnClose;
    }

    private void OnDisable() {
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