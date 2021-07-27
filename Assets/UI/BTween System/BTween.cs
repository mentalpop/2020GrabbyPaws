using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTween : BTweenSystem
{
    public TweenType Type = TweenType.CircEase;

    public enum TweenType
    {
        Linear,
        CircEase
    }

    protected override float PerformEffect() {
        float _tweenValue = 0;
        switch (Type) {
            case TweenType.Linear: _tweenValue = Mathf.Lerp(tweenStartVal, tweenTargetVal, timePassed / (doReverse ? durationClose : durationOpen)); break;
            case TweenType.CircEase: _tweenValue = CircEase(timePassed, tweenStartVal, tweenTargetVal - tweenStartVal, (doReverse ? durationClose : durationOpen)); break;
                //case TweenType.Sine: _tweenValue = Sine(timePassed, tweenDuration); break;
        }
        return _tweenValue;
    }

    private float CircEase(float _timePassed, float startVal, float targetValMinusStartVal, float duration) {
        _timePassed /= duration / 2;
        if (_timePassed < 1) return -targetValMinusStartVal / 2 * (Mathf.Sqrt(1 - _timePassed * _timePassed) - 1) + startVal;
        _timePassed -= 2;
        return targetValMinusStartVal / 2 * (Mathf.Sqrt(1 - _timePassed * _timePassed) + 1) + startVal;
    }

    /*
    private float Sine(float _timePassed, float duration) {
        float value = Mathf.Sin((_timePassed - Mathf.Floor(_timePassed)) * (Mathf.PI * 2f));//Mathf.Sin((_timePassed / duration) * (Mathf.PI * 2f));//-(Mathf.Cos(Mathf.PI * (_timePassed / duration)) - 1) / 2;
        Debug.Log("value: " + value + ", _timePassed: " + _timePassed);
        return value;
    }
    //*/
}