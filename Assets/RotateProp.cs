using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateProp : MonoBehaviour
{
    public float RotationSpeed;

    void Update()
    {
        gameObject.transform.Rotate(0, 0, Time.deltaTime * RotationSpeed);
    }
}
