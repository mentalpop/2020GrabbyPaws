using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Sine {
    public float div = 60f;
    public float magnitude = 0.5f;
    public float value = 0f;
    public float duration = 2f;
    public bool durationElapsed = false;

    private const double Tau = Math.PI * 2;

    public Sine(float div, float magnitude, float value, float duration, bool durationElapsed) {
        this.div = div;
        this.magnitude = magnitude;
        this.value = value;
        this.duration = duration;
        this.durationElapsed = durationElapsed;
    }

    public void Increment() {
        if (!durationElapsed) {
            value += (float)Math.PI / div;
            if (duration == 0f) {
                if (value > Tau)
                    value -= (float)Tau;
            } else if (value > duration * (float)Math.PI) {
                value = duration * (float)Math.PI;
                durationElapsed = true;
            }
        }
    }

    public void Decrement() {
        if (!durationElapsed) {
            value -= (float)Math.PI / div;
            if (duration != 0f && value < (float)Math.PI / div) {//duration * (float)Math.PI / div) {
                value = 0;
                durationElapsed = true;
            }
        }
    }

    public float GetSine() {
        return (float)Math.Sin((double)value);
    }

    public float GetSineMagnitude() {
        return (float)Math.Sin((double)value) * magnitude;
    }

    public float GetSineDuration() {
        return (float)Math.Sin((double)value) * magnitude * (1f - (value / (duration * (float)Math.PI)));
    }

    public bool PeriodsElapsed(float numPeriods) {
        return value > duration * numPeriods * (float)Math.PI;
    }

    public void Reset() {
        value = 0f;
        durationElapsed = false;
    }

    public void Max() {
        value = duration * (float)Math.PI; //Set this precisely
        durationElapsed = false;
    }
}