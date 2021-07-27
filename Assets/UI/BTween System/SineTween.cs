using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineTween : BTweenSystem
{
    public float iterations = 1f;
    public bool reduceMagnitudeOverDuration = false;

    private readonly float Tau = Mathf.PI * 2;

    protected override float PerformEffect() {
        float zeroToOne = timePassed - Mathf.Floor(timePassed); //Convert 1.x ... 2.x ... 3.x to a 0-1 range
        float duration = durationOpen / iterations; //How many iterations of Sine should elapse within the alloted time?
        float incrementedCount = (zeroToOne / duration) - Mathf.Floor(zeroToOne / duration);
        if (reduceMagnitudeOverDuration) {
            return Mathf.Sin(incrementedCount * Tau) * (1f - (timePassed / (doReverse ? durationClose : durationOpen)));
        } else {
            return Mathf.Sin(incrementedCount * Tau); //Magnitude does not diminish
        }
    }
}
