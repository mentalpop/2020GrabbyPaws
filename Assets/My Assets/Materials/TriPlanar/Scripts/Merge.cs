using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Merge : MonoBehaviour
{


    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;
    private float length;
  
    void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();
    }

    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            length = hit.point.y - 0.15f;
            print(length);
        }


        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetFloat("_MergeY", length);
        _renderer.SetPropertyBlock(_propBlock);

        //if (Input.GetKeyDown(KeyCode.A))
        // {
       // GetComponent<MeshFilter>().mesh.RecalculateNormals(100);
       // }
    }

   
}
