using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTrigger : MonoBehaviour
{


    public bool lightsOn = false;
    public GameObject Light;


    void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {

            if (lightsOn == false)
            {
                Light.SetActive(true);
                lightsOn = true;
            }
            else
            {
                Light.SetActive(false);
                lightsOn = false;
            }
        }

    }



    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            Light.SetActive(false);
            lightsOn = false;
        }
    }
}
