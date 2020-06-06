using UnityEngine;
using System.Collections;
namespace Invector
{
    public static class vCapsuleHelper
    {
        public static bool CheckCapsule(this CapsuleCollider capsule, Vector3 dir, out RaycastHit hit, LayerMask mask, bool drawGizmos = false)
        {
            var pCenter = capsule.transform.TransformPoint(capsule.center);
            var p1 = pCenter + capsule.transform.up * ((capsule.height * 0.5f) - capsule.radius);
            var p2 = pCenter - capsule.transform.up * ((capsule.height * 0.5f) - capsule.radius);

            // Debug.DrawLine(p1, p2);

            if (drawGizmos)
                Gizmos.color = Color.green;
            var check = false;
            if (Physics.CapsuleCast(p1, p2, 0.2f, dir, out hit, capsule.radius * 0.5f, mask))
            {
                if (drawGizmos)
                    Gizmos.color = Color.red;
                check = true;
            }
            if (drawGizmos)
            {
                Gizmos.DrawWireSphere(p1, capsule.radius);
                Gizmos.DrawWireSphere(p2, capsule.radius);
            }

            return check;
        }

        public static bool CheckCapsule(this CapsuleCollider capsule, Vector3 dir, LayerMask mask, bool drawGizmos = false)
        {

            var pCenter = capsule.transform.TransformPoint(capsule.center);
            var p1 = pCenter + capsule.transform.up * ((capsule.height * 0.5f) - capsule.radius);// + capsule.transform.forward * 0.3f;
            var p2 = pCenter - capsule.transform.up * ((capsule.height * 0.5f) - capsule.radius);

            if (drawGizmos)
                Gizmos.color = Color.green;
            var check = false;

            if (Physics.CapsuleCast(p1, p2, 0, dir, capsule.radius + 0.3f, mask, QueryTriggerInteraction.Collide))
            {
                if (drawGizmos)
                    Gizmos.color = Color.red;
                check = true;
            }
            if (drawGizmos)
            {

                Gizmos.DrawWireSphere(p1, capsule.radius + 0.3f);
                Gizmos.DrawWireSphere(p2, capsule.radius + 0.3f);
            }

            return check;
        }
    }
}