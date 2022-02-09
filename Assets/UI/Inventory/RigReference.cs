using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigReference : MonoBehaviour
{
    public Rig rig;
    public RigTransform rigTransform;
    public RigIDs rigID;

    private void OnValidate() {
        if (rig == null) {
            rig = GetComponent<Rig>(); 
        }
    }
}