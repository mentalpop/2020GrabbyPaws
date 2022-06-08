using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STweenRectAnchor : BTweenFormatter
{
    public RectTransform subjectRect;
    [SerializeField] private bool positionAsPercent = false;
    public Vector2 magnitude;

    private Vector2 originalValue;

    protected override void PerformEffect(float value) {
        Vector2 position = positionAsPercent ? new Vector2(magnitude.x * subjectRect.rect.width, magnitude.y * subjectRect.rect.height) : magnitude;
        if (relative) {
            subjectRect.anchoredPosition = originalValue + new Vector2(position.x * value, position.y * value);
        } else {
            subjectRect.anchoredPosition = new Vector2(position.x * value, position.y * value);
        }
    }

    public override void Calibrate() {
        originalValue = subjectRect.anchoredPosition;
    }

    protected override void TweenReset(float value) {
        subjectRect.anchoredPosition = originalValue;
    }
}