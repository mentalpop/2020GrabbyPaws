using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabActiveFormatter : MonoBehaviour
{
    public ListController ListController;
    public List<GameObject> tabContents = new List<GameObject>();

    private void OnEnable() {
        ListController.OnSelect += ListController_OnSelect;
    }

    private void OnDisable() {
        ListController.OnSelect -= ListController_OnSelect;
    }

    private void ListController_OnSelect(int index) {
        for (int i = 0; i < tabContents.Count; i++) {
            tabContents[i].SetActive(i == index);
        }
    }
}