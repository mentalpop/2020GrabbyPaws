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
    }

    private void OnDisable() {
        navButton.OnSelect -= NavButton_OnSelect;
    }

    private void NavButton_OnSelect(ButtonStateData _buttonStateData) {
        Debug.Log("NavButton_OnSelect: "+_buttonStateData);
        listController.SetActiveIndex(listIndex);
    }

    public void Unpack(ListController _listController, int index) {
        listController = _listController;
        listIndex = index;
    }
}