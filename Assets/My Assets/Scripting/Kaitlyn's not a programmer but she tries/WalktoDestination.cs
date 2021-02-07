using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WalktoDestination : MonoBehaviour
{

    public GameObject CharacterDestination;

    NavMeshAgent _Agent;


    // Start is called before the first frame update
    void Start()
    {
        _Agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        _Agent.SetDestination(CharacterDestination.transform.position);
    }
}
