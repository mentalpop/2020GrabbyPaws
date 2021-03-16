using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TabData", menuName = "Tab Data", order = 1)]
public class TabData : NavButtonData {
    
    public Sprite icon;
    public Color bgColor = Color.white;
    public Color bgColorHighlight = Color.white;
    public string text;
}