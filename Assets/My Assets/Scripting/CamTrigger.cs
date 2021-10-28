using UnityEngine;
using System.Collections;
using Cinemachine;

public class CamTrigger : MonoBehaviour
{

    public CinemachineVirtualCamera vcam;
    public float waitTime = 2f;

    //public bool waited = false;
    private WaitForSeconds cachedWaitForSeconds;
    private Coroutine myCoroutine;

    private void Awake() {
        cachedWaitForSeconds = new WaitForSeconds(waitTime);
    }


    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            myCoroutine = StartCoroutine(CheckIfPlayerStillPresent());
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            vcam.gameObject.SetActive(false);
            vcam.m_Priority = 0;
            if (myCoroutine != null) {
                StopCoroutine(myCoroutine);
            }
        }
    }


    private IEnumerator CheckIfPlayerStillPresent() {
        yield return cachedWaitForSeconds;
        vcam.gameObject.SetActive(true);
        vcam.m_Priority = 99;
        //yield return null;
    }
}