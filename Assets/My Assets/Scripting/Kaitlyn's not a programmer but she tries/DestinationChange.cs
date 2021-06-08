using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationChange : MonoBehaviour
{
    public int xPos;
    public int zPos;

    void OnTriggerEnter(Collider other)
    {
     if (other.tag == "NPC")
        {
            xPos = Random.Range(-12, 6);
            zPos = Random.Range(158, 174);

            this.gameObject.transform.position = new Vector3(xPos, 22f, zPos);
        }   
    }
}
