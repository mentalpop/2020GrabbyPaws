using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BTweenSystem : MonoBehaviour
{
    public float durationOpen = 0.2f;
    public float durationClose = 0.3f;
    [HideInInspector] public float tweenStartVal = 0f;
    [HideInInspector] public float tweenTargetVal = 1f;
    /*[HideInInspector] */public bool effectActive = false;
    [HideInInspector] public bool doReverse = false;
    public float PercentComplete => doReverse ? timePassed / durationClose : timePassed / durationOpen;

    protected float timePassed = 0f;

    public delegate void TweenEvent(float value);
    public event TweenEvent OnBeginTween = delegate { };
    public event TweenEvent OnTween = delegate { };
    public event TweenEvent OnEndTween = delegate { };

    protected virtual void Update() {
        if (effectActive) {
            if (doReverse) {
        //Counting down
                timePassed -= Time.deltaTime;
                if (timePassed < Mathf.Epsilon) {
                    effectActive = false;
                    timePassed = 0f;
                    DoOnTweenEnd(tweenStartVal);
                }
            } else {
        //Counting up
                timePassed += Time.deltaTime;
                if (timePassed >= durationOpen) {
                    timePassed = durationOpen;
                    effectActive = false;
                    DoOnTweenEnd(tweenTargetVal);
                }
            }
        //If the effect is still active, perform the calculation
            if (effectActive) {
                DoOnTween(PerformEffect());
            }
        }
    }

    protected virtual float PerformEffect() {
        return default;
    }

    public virtual void PlayFromZero() {
        timePassed = 0f;
        doReverse = false;
        effectActive = true;
        DoOnBegin(tweenStartVal);
    }

    public virtual void PlayFromEnd() {
        timePassed = durationClose;
        doReverse = true;
        effectActive = true;
        DoOnBegin(tweenTargetVal);
    }

    public virtual void Cancel() { //End the effect without triggering OnTweenEnd
        if (effectActive) {
            if (doReverse) {
                timePassed = 0f;
            } else {
                timePassed = durationOpen;
            }
            effectActive = false;
        }
    }

    protected void DoOnBegin(float _val) {
        OnBeginTween(_val);
    }

    protected void DoOnTween(float _val) {
        OnTween(_val);
    }

    protected void DoOnTweenEnd(float _val) {
        OnEndTween(_val);
    }
}