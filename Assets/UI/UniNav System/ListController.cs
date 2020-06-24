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

    public delegate void ListElementEvent (int index);
	public event ListElementEvent OnSelect = delegate { };

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

    public void SetActiveIndex(int _index) {
        activeIndex = Mathf.Clamp(_index, 0, elements.Count - 1);
        focusIndex = activeIndex;
        OnSelect(activeIndex);
        for (int i = 0; i < elements.Count; i++) {
            elements[i].navButton.SetFocus(i == activeIndex);
            if (behaveAsTabs) {
                elements[i].navButton.SetActive(i == activeIndex);
            }
        }
    }

    public void SetFocus(int _index) {
        focusIndex = Mathf.Clamp(_index, 0, elements.Count - 1);
        for (int i = 0; i < elements.Count; i++) {
            elements[i].navButton.SetFocus(i == focusIndex);
        }
    }

    public void Focus() {
        listHasFocus = true;
        SetFocus(focusIndex);
    }
    
    public void Unfocus() {
        listHasFocus = false;
        for (int i = 0; i < elements.Count; i++) {
            elements[i].navButton.SetFocus(false);
        }
    }

    /*
    public void SetFocus(bool _focus) {
        for (int i = 0; i < elements.Count; i++) {
            elements[i].navButton.SetFocus(_focus && i == activeIndex);
        }
    }
    //*/

    public void FirstIndex() {
        SetFocus(0);
    }

    public void LastIndex() {
        SetFocus(elements.Count - 1);
    }

    public bool IncrementIndex() {
        if (focusIndex < elements.Count - 1) {
            SetFocus(focusIndex + 1);
            return true;
        }
        return false;
    }

    public bool DecrementIndex() {
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