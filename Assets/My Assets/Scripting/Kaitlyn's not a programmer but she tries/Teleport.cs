using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
  //  public Transform teleportTarget;
    public Transform thePlayer;

 
    void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {

            thePlayer.transform.GetChild(0).localPosition = new Vector3(0,1,88);
        }

    }

    private void OnValidate()
    {
        if (thePlayer == null)
        {
            thePlayer = GetComponent<Transform>();
        }
    }

    private void Start()
    {
        thePlayer = UI.Player.transform;
    }
}
