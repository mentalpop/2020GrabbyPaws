using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSphere : MonoBehaviour
{
    private void Awake() { //Destroy self briefly after instantiating
        Destroy(gameObject, 0.1f);
    }
}