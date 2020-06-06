using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnAroundScript : MonoBehaviour
{


    public float turnSpeed = 20f;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.Rotate(0, Time.deltaTime * turnSpeed, 0);
    }
}
