using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISummoner : MonoBehaviour
{

    public Animator[] children;
    public bool Display = false;
    public float minDistance = 8;
    public float delay = 0.1f;

    // Start is called before the first frame update
    void Start()
    {

        children = GetComponentsInChildren<Animator>();
        for (int a = 0; a < children.Length; a++)
        {
            children[a].SetBool("Display", Display);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 delta = Camera.main.transform.position - transform.position;
        if (delta.magnitude < minDistance)
        {
            if (Display) return;
            StartCoroutine("ActivateInTurn");

        }
        else
        {
            if (!Display) return;
            StartCoroutine("DeactivateInTurn");

        }
    }

        public IEnumerator ActivateInTurn()
        {
            Display = true;
        Debug.Log("Coroutine started");

        yield return new WaitForSeconds(delay);
        Debug.Log("waited");
        for (int a = 0; a < children.Length; a++)
            {
                children[a].SetBool("Display", true);
                yield return new WaitForSeconds(delay);
            }
        }

        public IEnumerator DeactivateInTurn()
        {
            Display = false;

            yield return new WaitForSeconds(delay);
            for (int a = 0; a < children.Length; a++)
            {
                children[a].SetBool("Display", false);
                yield return new WaitForSeconds(delay);
            }
        }
    }