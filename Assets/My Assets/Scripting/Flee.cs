using UnityEngine;

namespace UnityMovementAI
{
    [RequireComponent(typeof(MovementAIRigidbody))]
    public class Flee : MonoBehaviour
    {
        public float panicDist = 3.5f;

        public bool decelerateOnStop = true;

        public float maxAcceleration = 10f;

        public float timeToTarget = 0.1f;

        MovementAIRigidbody rb;

        Rigidbody toot;

        public Animator anim;

        void Awake()
        {
            rb = GetComponent<MovementAIRigidbody>();
            toot = GetComponent<Rigidbody>();
        }


        void Update()
        {
            if (toot.velocity.sqrMagnitude > 0)
            {
            anim.SetFloat("InputMagnitude", 1);
            Debug.Log(toot.velocity.sqrMagnitude);
            }

            else
            {
                anim.SetFloat("InputMagnitude", 0);
            }
        }


            public Vector3 GetSteering(Vector3 targetPosition)
        {
            /* Get the direction */
            Vector3 acceleration = transform.position - targetPosition;

            /* If the target is far way then don't flee */
            if (acceleration.magnitude > panicDist)
            {
                /* Slow down if we should decelerate on stop */
                if (decelerateOnStop && rb.Velocity.magnitude > 0.001f)
                {
                    /* Decelerate to zero velocity in time to target amount of time */
                    acceleration = -rb.Velocity / timeToTarget;

                    if (acceleration.magnitude > maxAcceleration)
                    {
                        acceleration = GiveMaxAccel(acceleration);
                    }

                    return acceleration;
                }
                else
                {
                    rb.Velocity = Vector3.zero;
                    return Vector3.zero;
                }
            }

            return GiveMaxAccel(acceleration);
        }

        Vector3 GiveMaxAccel(Vector3 v)
        {
            v.Normalize();

            /* Accelerate to the target */
            v *= maxAcceleration;

            return v;
        }
   

    }
}