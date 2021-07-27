using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTweenCanvasGroup : BTweenFormatter
{
    public CanvasGroup canvasGroup;
    public AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    public float minValue = 0f;
    public float maxValue = 1f;

    private float originalValue;

    //protected override void OnEnable() {
    //    base.OnEnable();
    //    if (minValue == 0f) {
    //        Calibrate();
    //        canvasGroup.alpha = minValue;
    //    }
    //}

    //protected override void OnDisable() {
    //    base.OnDisable();
    //    if (minValue == 0f)
    //        canvasGroup.alpha = minValue;
    //}

    protected override void PerformEffect(float value) {
        if (relative) {
            canvasGroup.alpha = originalValue + Mathf.LerpUnclamped(minValue, maxValue, value) * curve.Evaluate(value);
        } else {
            canvasGroup.alpha = Mathf.LerpUnclamped(minValue, maxValue, curve.Evaluate(value));
        }
    }

    public override void Calibrate() {
        originalValue = canvasGroup.alpha;
    }

    protected override void TweenReset(float value) {
        canvasGroup.alpha = originalValue;
    }
}