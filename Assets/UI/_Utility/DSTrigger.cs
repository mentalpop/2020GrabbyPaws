using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DSTrigger : MonoBehaviour
{
    public List<GameObject> objects = new List<GameObject>();

    private void OnEnable() {
        foreach (var item in objects) {
            item.SetActive(true);
        }
    }

    private void OnDisable() {
        foreach (var item in objects) {
            item.SetActive(false);
        }
    }
}