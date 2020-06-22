using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuNode : MonoBehaviour
{
    public enum NavigationType
    {
        Vertical,
        Horizontal,
        Grid
    }
    public enum NavDir
    {
        Left,
        Right,
        Up,
        Down,
        Forward,
        Backward,
        Cancel
    }
    public ListController listController;
    public NavigationType navigationType;
    public Vector2Int menuSize;
    public bool outOfBoundsLoop = false;
    public MenuNode mLeft;
    public MenuNode mRight;
    public MenuNode mUp;
    public MenuNode mDown;
    public MenuNode mForward;
    public MenuNode mBackward;
    public MenuNode mCancel;

    /*
    public void SetFocus(bool _hasFocus) {

    }
    //*/
}