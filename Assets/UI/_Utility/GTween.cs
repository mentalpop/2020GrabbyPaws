using UnityEngine;

[System.Serializable]
public class GTween {
    public float tweenDuration = 1f;
    public float tweenStartVal = 1f;
    public float tweenTargetVal = 0f;
    public bool effectActive = true;
        
    private float timePassed = 0f;
    public bool doReverse = false;

    public GTween(float _tweenDuration, float _tweenStartVal, float _tweenTargetVal, bool _doReverse) {
        Initiate(_tweenDuration, _tweenStartVal, _tweenTargetVal, _doReverse);
    }

    public void Initiate(float _tweenDuration, float _tweenStartVal, float _tweenTargetVal, bool _doReverse) {
        tweenDuration = _tweenDuration;
        tweenStartVal = _tweenStartVal;
        tweenTargetVal = _tweenTargetVal;
        doReverse = _doReverse;
        timePassed = _doReverse ? tweenDuration : 0f;
    }
        
    public void Reset() {
        timePassed = 0f;
        doReverse = false;
        effectActive = true;
    }

    public void Reverse() {
        timePassed = tweenDuration;
        doReverse = true;
        effectActive = true;
    }

    public float DoTween() {
        if (effectActive) {
            if (doReverse) {
                timePassed -= Time.deltaTime;
                if (timePassed < Mathf.Epsilon) {
                    effectActive = false;
                    return tweenStartVal;
                } else {
                    return CircEase(timePassed, tweenStartVal, tweenTargetVal - tweenStartVal, tweenDuration);
                }
            } else {
                timePassed += Time.deltaTime;
                if (timePassed < tweenDuration) {
                    return CircEase(timePassed, tweenStartVal, tweenTargetVal - tweenStartVal, tweenDuration);
                } else {
                    effectActive = false;
                    return tweenTargetVal;
                }
            }
        } else {
            return tweenTargetVal;
        }
        //plugin.Value = ease(timePassed, startVal, targetVal - startVal, duration, easeParams);
    }

    private float CircEase(float _timePassed, float startVal, float targetValMinusStartVal, float duration) {
        _timePassed /= duration/2;
        if (_timePassed < 1) return -targetValMinusStartVal/2 * (Mathf.Sqrt(1 - _timePassed*_timePassed) - 1) + startVal;
        _timePassed -= 2;
        return targetValMinusStartVal/2 * (Mathf.Sqrt(1 - _timePassed*_timePassed) + 1) + startVal;
    }
}