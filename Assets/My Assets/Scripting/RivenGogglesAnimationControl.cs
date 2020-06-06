using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RivenGogglesAnimationControl : MonoBehaviour
{


    public float speed;
    private float val;
    private Renderer r;
    // Start is called before the first frame update
    void Start()
    {
        r = gameObject.GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        r.materials[3].SetFloat("_Offset", val);
        val += speed * Time.deltaTime;
        if(val > 1.0f)
        {
            val = 0;
        }
    }
}
