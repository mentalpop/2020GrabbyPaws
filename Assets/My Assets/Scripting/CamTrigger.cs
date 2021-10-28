using UnityEngine;
using System.Collections;
using Cinemachine;

public class CamTrigger : MonoBehaviour
{

    public CinemachineVirtualCamera vcam;
 


    void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {
            vcam.gameObject.SetActive(true);
            vcam.m_Priority = 99;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            vcam.gameObject.SetActive(false);
            vcam.m_Priority = 0;
        }
    }
}