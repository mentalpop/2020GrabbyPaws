using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollNavHandler : MonoBehaviour
{
    public ScrollRect scrollRect;
    public Vector2 speedMultiplier = Vector2.one;
    public float scrollPastLimit = 0.1f;
    public bool inFocus = true;

    public string inputHorizontal;
    public string inputVertical;
    public float axisThreshold = 0.025f;

    public void Scroll(Vector2 scrollDir) {
        if (inFocus) {
            if (scrollRect.horizontalScrollbar != null)
                scrollRect.horizontalScrollbar.value = Mathf.Clamp(scrollRect.horizontalScrollbar.value + scrollDir.x * Time.deltaTime * speedMultiplier.x, -scrollPastLimit, 1f + scrollPastLimit);
            if (scrollRect.verticalScrollbar != null)
                scrollRect.verticalScrollbar.value = Mathf.Clamp(scrollRect.verticalScrollbar.value + scrollDir.y * Time.deltaTime * speedMultiplier.y, -scrollPastLimit, 1f + scrollPastLimit);
        }
    }

    private void Update() {
        Vector2 inputVector = new Vector2(Input.GetAxis(inputHorizontal), Input.GetAxis(inputVertical));
        if (inputVector.magnitude > axisThreshold) {
            Scroll(inputVector);
        }
    }
}