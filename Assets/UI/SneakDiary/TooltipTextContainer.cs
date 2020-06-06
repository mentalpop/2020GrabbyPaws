using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipTextContainer : MonoBehaviour
{
    public RectTransform myRect;

    public GameObject bubbleLeft;
    public TextMeshProUGUI titleLeft;
    public TextMeshProUGUI descriptionLeft;
    
    public GameObject bubbleRight;
    public TextMeshProUGUI titleRight;
    public TextMeshProUGUI descriptionRight;

    //private bool myBool;

    public void Unpack(TimeIntervalData timeIntervalData, bool faceLeft) {
        if (faceLeft) {
            bubbleLeft.SetActive(true);
            bubbleRight.SetActive(false);
            titleLeft.text = timeIntervalData.title;
            descriptionLeft.text = timeIntervalData.description;
        } else {
            bubbleLeft.SetActive(false);
            bubbleRight.SetActive(true);
            titleRight.text = timeIntervalData.title;
            descriptionRight.text = timeIntervalData.description;
        }
    }
}