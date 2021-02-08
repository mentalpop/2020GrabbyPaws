using UnityEngine;

namespace UnityMovementAI
{
    public class FleeUnit : MonoBehaviour
    {
        [HideInInspector] public Transform target;

        SteeringBasics steeringBasics;
        Flee flee;

        void Start()
        {
            steeringBasics = GetComponent<SteeringBasics>();
            flee = GetComponent<Flee>();
            target = SceneTransitionHandler.GetPlayer().transform;
        }

        void FixedUpdate()
        {
            Vector3 accel = flee.GetSteering(target.position);

            steeringBasics.Steer(accel);
            steeringBasics.LookWhereYoureGoing();
        }
    }
}