﻿using UnityEngine;
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


    /*
    IEnumerator DropInventory(int count)
    {

        for(int _i = 0; _i < count; _i++)
        {
            

            yield return new WaitForSeconds(0.2f);

        }
        
    }
    //*/


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
        if(targets.Count > 0 && targets[index].gameObject.GetComponent<Outline>() != null)
        {
            targets[index].gameObject.GetComponent<Outline>().enabled = true;



            if (Input.GetButtonDown("Steal"))
            {

                targets[index].gameObject.GetComponent<Interactable>().Interact();
                targets.RemoveAt(index);
            }
        }
    }
     // Short answer, yes. When you remove a moth from the hell fire...

    void IncreaseIndex()
    {
        index++;
        if(index >= targets.Count)
        {
            index = 0;
        }
    }

    void DecreaseIndex()
    {
        index--;
        if(index < 0)
        {
            index = targets.Count - 1;
        }
    }

        private void Update()
        {

        ResetAllOutlines();
        CycleTarget();


        if(index >= targets.Count)
        {
            index = 0;
        }

        if (Input.GetButtonDown("ChangeFocus"))
        {
            IncreaseIndex();
        }

        if (Input.mouseScrollDelta.y > 0f )  {// forward
            //Debug.Log("mouseScrollDelta.y" + ": " + Input.mouseScrollDelta.y);
            IncreaseIndex();
        } else if (Input.mouseScrollDelta.y<  0f )  {// backwards
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
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }


    List<Collider> GetNearestObject()
    {

        
        Collider[] colliders = Physics.OverlapSphere(transform.position, checkRadius, checkLayers);
        Array.Sort(colliders, new DistanceComparer(transform));
        List<Collider> colliderList = new List<Collider>();

        for(int _i = 0; _i < colliders.Length; _i++)
        {
            if (IsTransformInCone(colliders[_i].transform, transform.position, transform.forward, 45))
            {
                colliderList.Add(colliders[_i]);
            }
        }


        return colliderList;

        /*if (colliderList.Count > 0)
        {
            if (colliderList[0].gameObject.GetComponent<Outline>() != null && colliderList[0] != null)
            {
                colliderList[0].gameObject.GetComponent<Outline>().enabled = true;
            }


            if (Input.GetButtonDown("Steal"))
            {

                colliderList[0].gameObject.GetComponent<Interactable>().Interact();

            }
        }*/

        /*
        if (colliders.Length > 0)
        {
            if (colliders[0].gameObject.GetComponent<Outline>() != null && colliders[0] != null)
            {
                colliders[0].gameObject.GetComponent<Outline>().enabled = true;
            }


            if (Input.GetButtonDown("Steal"))
            {

                colliders[0].gameObject.GetComponent<Interactable>().Interact();
                
            }
        }*/
    }

    void ResetAllOutlines()
    {
        foreach (Outline o in outlines)
        {
            if (o != null)
            {
                o.enabled = false;
            }
        }
    }

    bool IsTransformInCone(Transform t, Vector3 coneTipPos, Vector3 coneDirection, float coneHalfAngle)
    {
        Vector3 directionTowardT = t.position - coneTipPos;
        float angleFromConeCenter = Vector3.Angle(directionTowardT, coneDirection);
        return angleFromConeCenter <= coneHalfAngle;
    }




}
