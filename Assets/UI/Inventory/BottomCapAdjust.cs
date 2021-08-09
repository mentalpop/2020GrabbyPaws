using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomCapAdjust : MonoBehaviour
{
    public float heightOfAllElementsToSubtract; //THIS IS A TERRIBLE MAGIC NUMBER = Height of Buckles display + Height of 3 Tabs + Height of Top Cap
    public RectTransform myRect;

    private float screenHeight = 2160f;

    public void UpdateHeight(float heightOfScrollRect) {
        //Debug.Log("heightOfScrollRect: " + heightOfScrollRect);
        myRect.sizeDelta = new Vector2(myRect.sizeDelta.x, Mathf.Max(16f, screenHeight - heightOfAllElementsToSubtract - heightOfScrollRect)); //
        //Debug.Log("myRect.sizeDelta: " + myRect.sizeDelta.y);
    }
}