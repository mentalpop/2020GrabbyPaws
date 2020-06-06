using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateItem : MonoBehaviour
{
    public float RotationSpeed;

    void Update() {
        gameObject.transform.Rotate(0, Time.deltaTime * RotationSpeed, 0);
    }
}
