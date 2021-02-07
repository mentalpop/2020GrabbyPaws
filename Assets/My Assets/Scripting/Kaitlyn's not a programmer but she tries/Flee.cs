using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Flee : MonoBehaviour
{
    private NavMeshAgent _agent;

    public GameObject Player;

    public float PlayerDistanceRun = 4.0f;


    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
   
    }

    // Update is called once per frame
    void Update()
    {
        float squaredDist = (transform.position - Player.transform.position).sqrMagnitude;
        float PlayerDistanceRunSqrt = PlayerDistanceRun * PlayerDistanceRun;

        // Run away from Player
        if (squaredDist < PlayerDistanceRunSqrt)
        {
            Vector3 dirToPlayer = transform.position - Player.transform.position;

            Vector3 newPos = transform.position + dirToPlayer;

            _agent.SetDestination(newPos);
        }
    }
}
