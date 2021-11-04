using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLightHandler : MonoBehaviour
{
    public RectTransform spotlight;
    public Canvas myCanvas;

    private void Update() {
        //spotlight.localPosition = Input.mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out Vector2 pos);
        spotlight.position = myCanvas.transform.TransformPoint(pos);
    }
}