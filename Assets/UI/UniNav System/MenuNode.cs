using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNode : MonoBehaviour
{
    public enum NavDir
    {
        Accept,
        Cancel,
        Left,
        Right,
        Up,
        Down,
        Forward,
        Backward
    }
    public MenuNode mAccept;
    public MenuNode mCancel;
    public MenuNode mLeft;
    public MenuNode mRight;
    public MenuNode mUp;
    public MenuNode mDown;
    public MenuNode mForward;
    public MenuNode mBackward;
    public bool validSelection = true;

    public delegate void AbortSelectionEvent(MenuNode _fallbackMenu);
    public event AbortSelectionEvent OnSelectionAbort = delegate { };

    public virtual void MenuUnfocus() {
        
    }

    public virtual void MenuFocus() {
        
    }

    public virtual void MenuNavigate(NavDir navDir, MenuNavigator menuNavigator) {

    }

    public virtual NavButton GetButtonInFocus() {
        return null;
    }

    public void SelectionAbort(MenuNode _fallbackMenu) {
        OnSelectionAbort(_fallbackMenu);
    }
}