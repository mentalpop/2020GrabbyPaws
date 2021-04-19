using UnityEngine;
using System;
using System.Collections.Generic;
using cakeslice;
using System.Collections;

public class Sense : MonoBehaviour
{
    public float checkRadius;
    public LayerMask checkLayers;
    public Outline[] outlines;
    public float radius;
    public float depth;
    public float angle;
    private Physics physics;

    public List<Transform> targets;
    public Transform selectedTarget;
    private int index;
    private Inventory inventory;

    public void Start()
    {
        outlines = FindObjectsOfType<Outline>();
        targets = new List<Transform>();
        selectedTarget = null;
        index = 0;
        inventory = Inventory.instance;
    }


    public void AddAllTargets()
    {
        List<Collider> colliderList = GetNearestObject();
        targets.Clear();

        if(colliderList.Count > 0)
        {
            for(int i = 0; i < colliderList.Count; i++)
            {
                targets.Add(colliderList[i].transform);
            }
        }


        if (index >= targets.Count)
        {
            index = 0;
        }
    }


    public void CycleTarget()
    {
        AddAllTargets();
        if(targets.Count > 0) {
            Outline _outline = targets[index].gameObject.GetComponent<Outline>();
            if (_outline != null) {
                _outline.enabled = true;
                if (!UI.Instance.lockControls && Input.GetButtonDown("Steal")) {
                    targets[index].gameObject.GetComponent<Interactable>().Interact();
                    targets.RemoveAt(index);
                }
            }
        }
    }

    void IncreaseIndex() {
        index++;
        if (index >= targets.Count) {
            index = 0;
        }
    }

    void DecreaseIndex() {
        index--;
        if (index < 0) {
            index = targets.Count - 1;
        }
    }

    private void Update() {

        ResetAllOutlines();
        CycleTarget();


        if (index >= targets.Count) {
            index = 0;
        }

        if (Input.GetMouseButtonDown(0)) //"ChangeFocus"
        {
            IncreaseIndex();
        }

        if (Input.mouseScrollDelta.y > 0f) {// forward
            //Debug.Log("mouseScrollDelta.y" + ": " + Input.mouseScrollDelta.y);
            IncreaseIndex();
        } else if (Input.mouseScrollDelta.y < 0f) {// backwards
            //Debug.Log("mouseScrollDelta.y" + ": " + Input.mouseScrollDelta.y);
            DecreaseIndex();
        }

        /*
        if (Input.GetButtonDown("Yeet"))
        {
            StartCoroutine(DropInventory(inventory.items.Count));
        }
        //*/


    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }


    private List<Collider> GetNearestObject() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, checkRadius, checkLayers);
        Array.Sort(colliders, new DistanceComparer(transform));
        List<Collider> colliderList = new List<Collider>();

        for (int _i = 0; _i < colliders.Length; _i++) {
            if (IsTransformInCone(colliders[_i].transform, transform.position, transform.forward, 45)) {
                colliderList.Add(colliders[_i]);
            }
        }
        return colliderList;
    }

    void ResetAllOutlines() {
        foreach (Outline o in outlines) {
            if (o != null) {
                o.enabled = false;
            }
        }
    }

    bool IsTransformInCone(Transform t, Vector3 coneTipPos, Vector3 coneDirection, float coneHalfAngle) {
        Vector3 directionTowardT = t.position - coneTipPos;
        float angleFromConeCenter = Vector3.Angle(directionTowardT, coneDirection);
        return angleFromConeCenter <= coneHalfAngle;
    }
}
