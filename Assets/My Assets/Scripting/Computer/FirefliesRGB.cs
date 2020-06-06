using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirefliesRGB : MonoBehaviour {

    public Image i;

    public Slider s1;
    public Slider s2;
    public Slider s3;

    public ParticleSystem[] ps;

    // Use this for initialization
    void Start () {
        i.color = ps[0].main.startColor.color;
        s1.value = i.color.r;
        s2.value = i.color.g;
        s3.value = i.color.b;

	}
	
	// Update is called once per frame
	void Update () {
        i.color = new Color(s1.value, s2.value, s3.value);
	}


    public void updateFireflies()
    {
        foreach(ParticleSystem p in ps)
        {
            var main = p.main;
            main.startColor = i.color;
            p.gameObject.GetComponentInChildren<Light>().color = i.color;
        }
    }
}
