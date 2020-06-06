using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipSmall : MonoBehaviour
{
    public float padding = 16f;
    public RectTransform myRect;

    public GameObject bubbleLeft;
    public TextMeshProUGUI titleLeft;
    
    public GameObject bubbleRight;
    public TextMeshProUGUI titleRight;

    public void Unpack(string text, bool faceLeft) {
        RectTransform tooltipRect;
        TextMeshProUGUI title;
        if (faceLeft) {
            bubbleLeft.SetActive(true);
            bubbleRight.SetActive(false);
            titleLeft.text = text;
            title = titleLeft;
            tooltipRect = bubbleLeft.GetComponent<RectTransform>();
        } else {
            bubbleLeft.SetActive(false);
            bubbleRight.SetActive(true);
            titleRight.text = text;
            title = titleRight;
            tooltipRect = bubbleRight.GetComponent<RectTransform>();
        }
        tooltipRect.sizeDelta = new Vector2(Mathf.Max(title.preferredWidth + padding, tooltipRect.sizeDelta.x), tooltipRect.sizeDelta.y);
    }
}