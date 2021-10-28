using UnityEngine;
using System.Collections;
using Cinemachine;

public class CamTrigger : MonoBehaviour
{

    public CinemachineVirtualCamera vcam;


    public bool waited = false;


    void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")

            StartCoroutine(SetBoolToTrue());


    }

    private void Update()
    {
            if (waited)
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
            waited = false;
        }
    }


    private IEnumerator SetBoolToTrue()
    {

        yield return new WaitForSeconds(2f);
        if (waited == false)
        {
            waited = true;
        }
        yield return null;
    }
}