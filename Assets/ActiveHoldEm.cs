using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveHoldEm : MonoBehaviour
{

    public Transform HoldEmsParent;
    Holdables holdable;
    public UnityEngine.Animations.Rigging.Rig handik;
    public int soupNumber;

    // Start is called before the first frame update
    void Start()
    {
        Holdables existingHoldable = GetComponentInChildren<Holdables>();
        if (existingHoldable)
        {
            Hold(existingHoldable);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (soupNumber > 1)
        {
            handik.weight = 1.0f;

        }

        else
        {
            handik.weight = 0.0f;
        }
    }

    public void Hold(Holdables newHoldable)
    {
        if (holdable)
        {
            Destroy(holdable.gameObject);
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
