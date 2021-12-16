using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldablePickup : MonoBehaviour
{

    public Holdables holdablefab;

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        ActiveHoldEm activeHoldEm = other.gameObject.GetComponent<ActiveHoldEm>();
        if (activeHoldEm)
        {
            Holdables newHoldable = Instantiate(holdablefab, activeHoldEm.HoldEmsParent);
            activeHoldEm.Hold(newHoldable);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
