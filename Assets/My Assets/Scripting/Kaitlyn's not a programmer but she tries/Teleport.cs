using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform teleportTarget;

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            other.transform.position = teleportTarget.transform.position;
            //thePlayer.transform.GetChild(0).localPosition = new Vector3(0, 1, 88);
        }
    }
}
