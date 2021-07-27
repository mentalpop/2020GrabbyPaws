using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTweenRectAnchor : BTweenFormatter
{
    public RectTransform subjectRect;
    public AnimationCurve curveX = AnimationCurve.Constant(0f, 1f, 1f);
    public AnimationCurve curveY = AnimationCurve.Constant(0f, 1f, 1f);
    [SerializeField] private bool positionAsPercent = true;
    public Vector2 positionMin;
    public Vector2 positionMax = Vector2.one;

    private Vector2 originalValue;

    protected override void PerformEffect(float value) {
        Vector2 minPosition = positionAsPercent ? new Vector2(positionMin.x * subjectRect.rect.width, positionMin.y * subjectRect.rect.height) : positionMin;
        Vector2 maxPosition = positionAsPercent ? new Vector2(positionMax.x * subjectRect.rect.width, positionMax.y * subjectRect.rect.height) : positionMax;
        if (relative) {
            subjectRect.anchoredPosition = originalValue + new Vector2(Mathf.LerpUnclamped(minPosition.x, maxPosition.x, value) * curveX.Evaluate(BTween.PercentComplete), Mathf.LerpUnclamped(minPosition.y, maxPosition.y, value) * curveY.Evaluate(BTween.PercentComplete));
        } else {
            subjectRect.anchoredPosition = originalValue + new Vector2(Mathf.LerpUnclamped(minPosition.x, maxPosition.x, curveX.Evaluate(value)), Mathf.LerpUnclamped(minPosition.y, maxPosition.y, curveY.Evaluate(value)));
            //subjectRect.anchoredPosition = new Vector2(Mathf.LerpUnclamped(minPosition.x, maxPosition.x, curveX.Evaluate(value)), Mathf.LerpUnclamped(minPosition.y, maxPosition.y, curveY.Evaluate(value)));
        }
    }

    public override void Calibrate() {
        originalValue = subjectRect.anchoredPosition;
    }

    protected override void TweenReset(float value) {
        subjectRect.anchoredPosition = originalValue;
    }
}
