using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListController : MonoBehaviour
{
    public bool behaveAsTabs = false;
    //public List<ListElement> elements = new List<ListElement>();
    //private int listElementCount = -1; //Initialize to an invalid count
    private List<ListElement> elements;
    public List<ListElement> Elements
    {
        get { return elements; }
        set {
            elements = value;
            DefineIndex();
        }
    }
    public int activeIndex = 0;
    public int focusIndex = 0;
    public bool listHasFocus = false;
    public ListElement ActiveElement => elements[activeIndex];
    public ListElement FocusElement => elements[focusIndex];
    //public RectTransform contentPanel;

    public delegate void ListElementEvent (int index);
	public event ListElementEvent OnSelect = delegate { };
	public event ListElementEvent OnFocus = delegate { };

    public delegate void ListEmptyEvent (bool _isEmpty);
	public event ListEmptyEvent OnListEmpty = delegate { };

    /*
    private void Awake() {
        if (elements.Count != 0) {
            for (int i = 0; i < elements.Count; i++) {
                elements[i].Unpack(this, i);
            }
        }
    }
    //*/
    private void OnEnable() {
        MenuNavigator.Instance.OnInputMethodSet += Instance_OnInputMethodSet;
    }

    private void OnDisable() {
        MenuNavigator.Instance.OnInputMethodSet -= Instance_OnInputMethodSet;
    }

    private void Instance_OnInputMethodSet(bool isUsingMouse) {
        if (elements.Count > 0) {
            if (isUsingMouse) {
                Unfocus();
            } else {
                SetActiveIndex(activeIndex);
            }
        }
    }

    public virtual void SetActiveIndex(int _index) {
        OnSelectEvent(_index);
        //bool _focusOrMouseUse = listHasFocus || MenuNavigator.MouseIsUsing();
        for (int i = 0; i < elements.Count; i++) {
            elements[i].navButton.SetFocus(listHasFocus && i == activeIndex);//_focusOrMouseUse
            if (behaveAsTabs) {
                elements[i].navButton.SetActive(i == activeIndex);
            }
        }
    }

    protected void OnSelectEvent(int _index) {
        //Debug.Log("_index: "+_index);
        activeIndex = Mathf.Clamp(_index, 0, elements.Count - 1);
        focusIndex = activeIndex;
        OnSelect(activeIndex);
    }

    public void SetFocus(int _index) {
        if (elements == null) {
            OnListEmpty(true);
            Debug.Log("Tried to SetFocus on elements, but elements list is null; "+gameObject.name);
        } else {
            focusIndex = Mathf.Clamp(_index, 0, elements.Count - 1);
            for (int i = 0; i < elements.Count; i++) {
                elements[i].navButton.SetFocus(i == focusIndex);
            }
            OnFocus(focusIndex);
        }
    }

    public virtual void Focus() {
        listHasFocus = true;
        SetFocus(focusIndex);
    }
    
    public virtual void Unfocus() {
        listHasFocus = false;
        if (elements != null) {
            for (int i = 0; i < elements.Count; i++) {
                elements[i].navButton.SetFocus(false);
            }
        }
    }

    /*
    public void SetFocus(bool _focus) {
        for (int i = 0; i < elements.Count; i++) {
            elements[i].navButton.SetFocus(_focus && i == activeIndex);
        }
    }
    //*/

    public virtual void FirstIndex() {
        SetFocus(0);
    }

    public virtual void LastIndex() {
        SetFocus(elements.Count - 1);
    }

    public virtual bool IncrementIndex() {
        if (focusIndex < elements.Count - 1) {
            SetFocus(focusIndex + 1);
            return true;
        }
        return false;
    }

    public virtual bool DecrementIndex() {
        if (focusIndex > 0) {
            SetFocus(focusIndex - 1);
            return true;
        }
        return false;
    }

    public void DefineIndex() {
        if (elements.Count == 0) {
            OnListEmpty(true);
        } else {
            OnListEmpty(false);
            for (int i = 0; i < elements.Count; i++) {
                elements[i].Unpack(this, i);
            }
        }
    }
}