using UnityEngine;

namespace UnityMovementAI
{
    public class FleeUnit : MonoBehaviour
    {
        [HideInInspector] public Transform target;
        public float bodgeRotation = 90f;

        SteeringBasics steeringBasics;
        Flee flee;

        void Start()
        {
            steeringBasics = GetComponent<SteeringBasics>();
            flee = GetComponent<Flee>();
            flee.bodgeRotation = bodgeRotation;
            target = SceneTransitionHandler.GetPlayer().transform;
        }

        void FixedUpdate()
        {
            Vector3 accel = flee.GetSteering(target.position);

            steeringBasics.Steer(accel);
            steeringBasics.LookWhereYoureGoing(bodgeRotation);
        }
    }
}