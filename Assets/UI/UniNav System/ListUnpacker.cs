using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListUnpacker : MonoBehaviour
{
    public ListController listController;
    public Transform targetTransform;
    public GameObject listObject;
    public List<NavButtonData> buttonData = new List<NavButtonData>();

    protected List<GameObject> lineItems = new List<GameObject>();

    private void Awake() {
        if (buttonData != null)
            Unpack(buttonData);
    }

    public virtual void Unpack(List<NavButtonData> _buttonData) {
        buttonData = _buttonData;
        ClearList();
        List<ListElement> _elements = new List<ListElement>();
        for (int i = 0; i < buttonData.Count; i++) {
            var buttonClone = Instantiate(listObject, targetTransform, false);
            lineItems.Add(buttonClone);
            ListElement liEl = buttonClone.GetComponent<ListElement>();
            _elements.Add(liEl);
            //NavButton _NavButton = buttonClone.GetComponent<NavButton>();
            liEl.navButton.Unpack(buttonData[i]);
        }
        listController.Elements = _elements;
        if (listController.behaveAsTabs) {
            listController.SetActiveIndex(listController.activeIndex);
        } else if (listController.listHasFocus) {
            listController.SetFocus(listController.focusIndex);
        }
    }

    public void ClearList() {
        if (lineItems.Count > 0) {
            foreach (var item in lineItems) {
                Destroy(item);
            }
            lineItems.Clear();
        }
        /*
        if (targetTransform.childCount > 0) {
            foreach (Transform child in targetTransform) {
                Destroy(child.gameObject);
            }
        }
        //*/
    }
}