using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomCapAdjust : MonoBehaviour
{
    public float heightOfAllElementsToSubtract; //THIS IS A TERRIBLE MAGIC NUMBER = Height of Buckles display + Height of 3 Tabs + Height of Top Cap
    public RectTransform myRect;

    public void UpdateHeight(float heightOfScrollRect) {
        myRect.sizeDelta = new Vector2(myRect.sizeDelta.x, Mathf.Max(32f, Screen.height - heightOfAllElementsToSubtract - heightOfScrollRect)); //
    }
}