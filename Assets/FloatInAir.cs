using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatInAir : MonoBehaviour
{
    public float amp1;
    public float amp2;
    public float speed;              

    private float tempValY;
    private float tempValX;
    private Vector3 tempPos;

    void Start()
    {
        tempValY = transform.localPosition.y;
        tempValX = transform.localPosition.x;
    }

    void Update()
    {
        tempPos.y = tempValY + amp1 * Mathf.Sin(speed * Time.time);
        tempPos.x = tempValX + amp2 * Mathf.Sin(speed * Time.time);
        transform.localPosition = new Vector3(tempPos.x, tempPos.y, transform.localPosition.z);
    }
}