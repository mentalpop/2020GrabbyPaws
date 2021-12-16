using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveHoldEm : MonoBehaviour
{

    public Transform HoldEmsParent;
    Holdables holdable;
    public UnityEngine.Animations.Rigging.Rig handik;
    

    // Start is called before the first frame update
    void Start()
    {
        Holdables existingHoldable = GetComponentInChildren<Holdables>();
        if (existingHoldable)
        {
            Hold(existingHoldable);
        }
    }



    public void Hold(Holdables newHoldable)
    {
        if (holdable)
        {
            Destroy(holdable.gameObject);
            handik.weight = 0f;
            {
                holdable = newHoldable;
                holdable.transform.parent = HoldEmsParent;
                holdable.transform.localPosition = Vector3.zero;
                holdable.transform.localRotation = Quaternion.identity;
                handik.weight = 1.0f;
            }
        }
    }
}
