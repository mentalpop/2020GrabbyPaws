using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationPlayOnStart : MonoBehaviour
{
    public Animator animator;
    public string currentState;
    

    private void Start()
    {
            animator.Play(currentState);
    }


}