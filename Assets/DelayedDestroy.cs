using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedDestroy : MonoBehaviour
{

    public float var;


    void DestroyObjectDelayed()
    {
        Destroy(gameObject, var);
    }

    private void OnEnable()
    {
        DestroyObjectDelayed();
    }
}
