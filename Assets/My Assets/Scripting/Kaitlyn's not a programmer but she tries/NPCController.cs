using System.Collections;
using System.Collections.Generic;
using Invector.vCharacterController;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public Animator animator;
    


    void Awake()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("isHiding", true);
    }
}