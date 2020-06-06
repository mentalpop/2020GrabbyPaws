#if (UNITY_EDITOR)
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace ECE
{
  /// <summary>
  /// Used to draw gizmos for selected / hovered vertices
  /// Gizmos draw significantly faster than handles.
  /// </summary>

  [System.Serializable]
  public class EasyColliderGizmos : MonoBehaviour
  {
    public GIZMO_TYPE GizmoType = GIZMO_TYPE.SPHERE;
    public float SelectedVertexScale = 0.15f;
    public Color SelectedVertexColor = Color.green;
    public List<Vector3> SelectedVertexPositions = new List<Vector3>();
    public float HoveredVertexScale = 0.1f;
    public Color HoveredVertexColor = Color.cyan;
    public HashSet<Vector3> HoveredVertexPositions = new HashSet<Vector3>();
    public float OverlapVertexScale = 0.125f;
    public Color OverlapVertexColor = Color.red;
    public float DisplayVertexScale = 0.05f;
    public Color DisplayVertexColor = Color.blue;
    public HashSet<Vector3> DisplayVertexPositions = new HashSet<Vector3>();
    public bool DisplayAllVertices = false;
    public bool UseFixedGizmoScale = true;
    public bool DrawGizmos = true;
    void OnDrawGizmos()
    {
      if (DrawGizmos)
      {
        // Keep track of gizmos color to reset at end
        Color original = Gizmos.color;
        // Selected vertices.
        Gizmos.color = SelectedVertexColor;
        // size is modified for each vertex if using fixed scaling from handle utility.
        Vector3 size = Vector3.one * SelectedVertexScale;
        // original size is kept track of to make calculations easier.
        Vector3 originalSize = size;
        // scale for spheres.
        float scale = SelectedVertexScale;
        foreach (Vector3 vert in SelectedVertexPositions)
        {
          if (UseFixedGizmoScale)
          {
            scale = HandleUtility.GetHandleSize(vert);
            size = originalSize * scale;
            scale *= SelectedVertexScale;
          }
          DrawAGizmo(vert, size, scale, GizmoType);
        }

        // Hover vertices.
        Gizmos.color = HoveredVertexColor;
        size = Vector3.one * HoveredVertexScale;
        Vector3 sizeOverlap = Vector3.one * OverlapVertexScale;
        originalSize = size;
        scale = HoveredVertexScale;
        foreach (Vector3 vert in HoveredVertexPositions)
        {
          if (SelectedVertexPositions.Contains(vert))
          {
            if (UseFixedGizmoScale)
            {
              scale = HandleUtility.GetHandleSize(vert);
              sizeOverlap = originalSize * scale;
              scale *= OverlapVertexScale;
            }
            Gizmos.color = OverlapVertexColor;
            DrawAGizmo(vert, sizeOverlap, scale, GizmoType);
          }
          else
          {
            if (UseFixedGizmoScale)
            {
              scale = HandleUtility.GetHandleSize(vert);
              size = originalSize * scale;
              scale *= HoveredVertexScale;
            }
            Gizmos.color = HoveredVertexColor;
            DrawAGizmo(vert, size, scale, GizmoType);
          }
        }

        // Display all vertices.
        if (DisplayAllVertices)
        {
          Gizmos.color = DisplayVertexColor;
          size = Vector3.one * DisplayVertexScale;
          originalSize = size;
          scale = DisplayVertexScale;
          foreach (Vector3 vert in DisplayVertexPositions)
          {
            if (UseFixedGizmoScale)
            {
              scale = HandleUtility.GetHandleSize(vert);
              size = originalSize * scale;
              scale *= DisplayVertexScale;
            }
            DrawAGizmo(vert, size, scale, GizmoType);
          }
        }
        // reset default gizmos color.
        Gizmos.color = original;
      }
    }

    /// <summary>
    /// Draws a gizmo of type at position at size or scale.
    /// </summary>
    /// <param name="position">World position to draw at</param>
    /// <param name="size">Size of cube to draw</param>
    /// <param name="scale">Radius of sphere to draw</param>
    /// <param name="gizmoType">Sphere or Cubes?</param>
    private void DrawAGizmo(Vector3 position, Vector3 size, float scale, GIZMO_TYPE gizmoType)
    {
      switch (gizmoType)
      {
        case GIZMO_TYPE.SPHERE:
          Gizmos.DrawSphere(position, scale / 2);
          break;
        case GIZMO_TYPE.CUBE:
          Gizmos.DrawCube(position, size);
          break;
      }
    }
  }
}
#endif