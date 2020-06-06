using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabSortMenu : MonoBehaviour
{
    public GameObject tabPrefab;
	/*
    public GameObject separatorPrefab;
    public Sprite spriteMOver;
    public Sprite spriteActive;
    public bool addLastSeparator = true;
    //*/
    public int activeTab = 0;
    [HideInInspector] public List<GameObject> tabs = new List<GameObject>();

	public delegate void TabSelectEvent (int choiceMade);
	public event TabSelectEvent OnTabSelected = delegate { };

    public void InstantiateTabs(List<TabData> _tabData) {
        int i = 0;
        //int sepCount = addLastSeparator ? _tabData.Count : _tabData.Count - 1;
        foreach (var data in _tabData) {
            var newTab = Instantiate(tabPrefab, transform, false);
            tabs.Add(newTab);
            TabSortItem tab = newTab.GetComponent<TabSortItem>();
            //tab.TabResize(tabs.Count);
            /*
            if (i < sepCount) {
                tab.InsertSeparator(separatorPrefab);
            }
            //*/
            tab.UnpackData(data, this, i++);
            //tab.tabID = i++;
        }
    //Activate the first tab in the list
        //TabSelect(activeTab);
    }
        
    public void TabSelect(int _activateTabID) {
        //Debug.Log("_activateTabID: " + _activateTabID);
        OnTabSelected(_activateTabID);
        activeTab = _activateTabID;
//Iterate through all tabs and disable all but the selected one
        for (int i = 0; i < tabs.Count; i++) {
            TabSortItem tab = tabs[i].GetComponent<TabSortItem>();
            tab.contentActive = i == _activateTabID;
            //bwt.content.SetActive(i == _activateTabID);
            //tab.SetOpacity();
        }
    }
}