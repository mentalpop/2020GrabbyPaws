using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTweenScale : BTweenFormatter
{
    public Transform subjectTransform;
    public AnimationCurve curveX = AnimationCurve.Constant(0f, 1f, 1f);
    public AnimationCurve curveY = AnimationCurve.Constant(0f, 1f, 1f);
    public AnimationCurve curveZ = AnimationCurve.Constant(0f, 1f, 1f);
    [SerializeField] private bool sizeAsPercent = true;
    public Vector3 scaleMin;
    public Vector3 scaleMax = Vector3.one;

    private Vector3 originalValue;

    protected override void PerformEffect(float value) {
        Vector2 minScale = scaleMin;
        Vector2 maxScale = scaleMax;
        if (sizeAsPercent) { //Convert the value into a percent
            minScale = new Vector3(scaleMin.x / subjectTransform.localScale.x, scaleMin.y / subjectTransform.localScale.y, scaleMin.z / subjectTransform.localScale.z);
            maxScale = new Vector3(scaleMax.x / subjectTransform.localScale.x, scaleMax.y / subjectTransform.localScale.y, scaleMax.z / subjectTransform.localScale.z);
        }
        if (relative) {
            subjectTransform.localScale = originalValue + new Vector3(
                Mathf.LerpUnclamped(minScale.x, maxScale.x, value) * curveX.Evaluate(BTween.PercentComplete),
                Mathf.LerpUnclamped(minScale.y, maxScale.y, value) * curveY.Evaluate(BTween.PercentComplete),
                Mathf.LerpUnclamped(minScale.y, maxScale.y, value) * curveZ.Evaluate(BTween.PercentComplete));
        } else {
            subjectTransform.localScale = new Vector3(
                Mathf.LerpUnclamped(minScale.x, maxScale.x, curveX.Evaluate(value)),
                Mathf.LerpUnclamped(minScale.y, maxScale.y, curveY.Evaluate(value)),
                Mathf.LerpUnclamped(minScale.y, maxScale.y, curveZ.Evaluate(value)));
        }
    }

    public override void Calibrate() {
        originalValue = subjectTransform.localScale;
    }

    protected override void TweenReset(float value) {
        subjectTransform.localScale = originalValue;
    }
}