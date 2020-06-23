using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListUnpacker : MonoBehaviour
{
    public ListController listController;
    public Transform targetTransform;
    public GameObject listObject;
    public List<NavButtonData> buttonData = new List<NavButtonData>();

    private void Awake() {
        if (buttonData != null)
            Unpack(buttonData);
    }

    public void Unpack(List<NavButtonData> _buttonData) {
        buttonData = _buttonData;
        ClearList();
        List<ListElement> _elements = new List<ListElement>();
        for (int i = 0; i < buttonData.Count; i++) {
            var buttonClone = Instantiate(listObject, targetTransform, false);
            ListElement liEl = buttonClone.GetComponent<ListElement>();
            liEl.Unpack(listController, i);
            _elements.Add(liEl);
            NavButton _NavButton = buttonClone.GetComponent<NavButton>();
            _NavButton.Unpack(buttonData[i]);
        }
        listController.elements = _elements;
        if (listController.behaveAsTabs) {
            listController.SetActiveIndex(listController.activeIndex);
        } else if (listController.listHasFocus) {
            listController.SetFocus(listController.focusIndex);
        }
        //listController.DefineIndex();
    }

    public void ClearList() {
        if (targetTransform.childCount > 0) {
            foreach (Transform child in targetTransform) {
                Destroy(child.gameObject);
            }
        }
    }
}