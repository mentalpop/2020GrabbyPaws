using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TabData", menuName = "Tab Data", order = 1)]
public class TabData : ScriptableObject {
    
    public Sprite icon;
    public Color bgColor = Color.white;
    public Color bgColorHighlight = Color.white;
    public string text;

    /*
    public TabData(Sprite icon, string text) {
        this.icon = icon;
        this.text = text;
    }
    public TabData(Sprite icon) {
        this.icon = icon;
        text = "";
    }
    //*/
}