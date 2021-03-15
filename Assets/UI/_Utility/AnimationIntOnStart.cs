using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationIntOnStart : MonoBehaviour
{
    public Animator animator;
    public string intName;
    public int value;
    public bool triggerOnStart;

    private void Start() {
        if (triggerOnStart) {
            AnimatorSetInt(intName, value);
        }
    }

    public void AnimatorSetInt() {
        AnimatorSetInt(intName, value);
    }

    public void AnimatorSetInt(string _int) {
        AnimatorSetInt(_int, value);
    }

    public void AnimatorSetInt(string _int, int _value) {
        animator.SetInteger(_int, _value);
    }
}