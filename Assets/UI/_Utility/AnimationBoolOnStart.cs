using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationBoolOnStart : MonoBehaviour
{
    public Animator animator;
    public string boolName;
    public bool value;
    public bool triggerOnStart;

    private void Start() {
        if (triggerOnStart) {
            AnimatorSetBool(boolName, value);
        }
    }

    public void AnimatorSetBool() {
        AnimatorSetBool(boolName, value);
    }

    public void AnimatorSetBool(string _bool) {
        AnimatorSetBool(_bool, value);
    }

    public void AnimatorSetBoolTrue(string _bool) {
        AnimatorSetBool(_bool, true);
    }

    public void AnimatorSetBoolFalse(string _bool) {
        AnimatorSetBool(_bool, false);
    }

    public void AnimatorToggleBool() {
        AnimatorToggleBool(boolName);
    }

    public void AnimatorToggleBool(string _bool) {
        AnimatorSetBool(_bool, !animator.GetBool(boolName));
    }

    public void AnimatorSetBool(string _bool, bool _value) {
        animator.SetBool(_bool, _value);
    }
}