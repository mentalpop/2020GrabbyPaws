using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListElement : MonoBehaviour
{
    public NavButton navButton;

    [HideInInspector] public ListController listController;
    [HideInInspector] public int listIndex;

    private void OnEnable() {
        navButton.OnSelect += NavButton_OnSelect;
        navButton.OnSelectExt += NavButton_OnSelectExt;
    }

    private void OnDisable() {
        navButton.OnSelect -= NavButton_OnSelect;
        navButton.OnSelectExt -= NavButton_OnSelectExt;
    }

    private void NavButton_OnSelect(ButtonStateData _buttonStateData) {
        //Debug.Log("NavButton_OnSelect: "+name);
        listController.SetActiveIndex(listIndex);
    }

    private void NavButton_OnSelectExt(ButtonStateData _buttonStateData, object _data) {
        NavButton_OnSelect(_buttonStateData);
    }

    public void Unpack(ListController _listController, int index) {
        listController = _listController;
        listIndex = index;
    }
}