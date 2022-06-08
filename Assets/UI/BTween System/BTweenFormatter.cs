using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BTweenFormatter : MonoBehaviour
{
    public BTweenSystem BTween;
    [SerializeField] protected bool relative = false;
    [SerializeField] protected bool calibrateOnBegin = false;
    [SerializeField] protected bool resetOnEnd = false;
    [SerializeField] protected bool performEffectOnEnd = true;

    private void Awake() {
        Calibrate();
    }

    protected virtual void OnEnable() {
        BTween.OnBeginTween += BTween_OnBeginTween;
        BTween.OnTween += BTween_OnTween;
        BTween.OnEndTween += BTween_OnEndTween;
    }

    protected virtual void OnDisable() {
        BTween.OnBeginTween -= BTween_OnBeginTween;
        BTween.OnTween -= BTween_OnTween;
        BTween.OnEndTween -= BTween_OnEndTween;
    }

    protected virtual void BTween_OnTween(float value) {
        PerformEffect(value);
    }

    protected virtual void BTween_OnBeginTween(float value) {
        if (calibrateOnBegin)
            Calibrate();
        if (performEffectOnEnd)
            PerformEffect(value);
    }

    protected virtual void BTween_OnEndTween(float value) {
        if (resetOnEnd)
            TweenReset(value);
        if (performEffectOnEnd)
            PerformEffect(value);
    }

    protected virtual void TweenReset(float value) {
        throw new System.NotImplementedException();
    }

    public virtual void Calibrate() {
        throw new System.NotImplementedException();
    }

    protected virtual void PerformEffect(float value) {
        throw new System.NotImplementedException();
    }
}