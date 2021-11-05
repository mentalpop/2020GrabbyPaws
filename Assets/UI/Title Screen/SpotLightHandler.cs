using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLightHandler : MonoBehaviour
{
    public RectTransform spotlight;
    public Canvas myCanvas;
    public GTween gTween;
    public Vector2 offsetPosition;
    public Sine sineX;
    public Sine sineY;
    public Sine sineScale;

    private float percentInfluenceGamepad;
    private bool isUsingMouse = true;
    private bool effectisUsingMouse = true;

    private void OnEnable() {
        MenuNavigator.Instance.OnInputMethodSet += Instance_OnInputMethodSet;
    }

    private void OnDisable() {
        MenuNavigator.Instance.OnInputMethodSet -= Instance_OnInputMethodSet;
    }
    private void Instance_OnInputMethodSet(bool _isUsingMouse) {
        isUsingMouse = _isUsingMouse;
    }

    private void Update() {
        sineScale.Increment();
    //Mouse
        RectTransformUtility.ScreenPointToLocalPointInRectangle(myCanvas.transform as RectTransform, Input.mousePosition, myCanvas.worldCamera, out Vector2 mouseActiveSpotlightPosition);
    //Gamepad
        sineX.Increment();
        sineY.Increment();
        Vector2 gamepadActiveSpotlightPosition = new Vector2(sineX.GetSineMagnitude() + offsetPosition.x,
                sineY.GetSineMagnitude() + offsetPosition.y);
    //Perform Lerp
        if (gTween.effectActive) {
            float tweenVal = gTween.DoTween();
            if (gTween.effectActive) {
                percentInfluenceGamepad = tweenVal;
            }
        }
        if (!gTween.effectActive) {
            percentInfluenceGamepad = isUsingMouse ? 0f : 1f;
            if (isUsingMouse != effectisUsingMouse) {
                effectisUsingMouse = isUsingMouse;
                if (effectisUsingMouse) {
                    gTween.Reverse(); //Go to 0
                } else {
                    gTween.Reset(); //Go to 1
                }
            }
        }
    /*More direct, but flickers if you switch input types rapidly
        Vector2 destPosition = Vector2.Lerp(mouseActiveSpotlightPosition, gamepadActiveSpotlightPosition, percentInfluenceGamepad);
        spotlight.position = myCanvas.transform.TransformPoint(destPosition);
        //*/
    //Smoother
        Vector2 destPosition = Vector2.Lerp(mouseActiveSpotlightPosition, gamepadActiveSpotlightPosition, percentInfluenceGamepad);
        spotlight.position = Vector2.Lerp(spotlight.position, myCanvas.transform.TransformPoint(destPosition), 3 * Time.deltaTime);
        ;
    }
}