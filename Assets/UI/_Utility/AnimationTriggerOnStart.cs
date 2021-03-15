using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationTriggerOnStart : MonoBehaviour
{
    public Animator animator;
    public string trigger;
    public bool triggerOnStart;

    private void Start() {
        if (triggerOnStart) {
            AnimatorSetTrigger(trigger);
        }
    }

    public void AnimatorSetTrigger() {
        AnimatorSetTrigger(trigger);
    }

    public void AnimatorSetTrigger(string _trigger) {
        animator.SetTrigger(_trigger);
    }
}