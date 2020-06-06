#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
namespace ECE
{
  public static class EasyColliderDraw
  {
    /// <summary>
    /// Draws a primitive collider (box, sphere, capsule) using lines.
    /// </summary>
    /// <param name="collider">Collider to draw</param>
    /// <param name="color">Color of lines to draw with</param>
    public static void DrawCollider(Collider collider, Color color)
    {
      if (collider is BoxCollider)
      {
        DrawBoxCollider(collider as BoxCollider, color);
      }
      else if (collider is SphereCollider)
      {
        DrawSphereCollider(collider as SphereCollider, color);
      }
      else if (collider is CapsuleCollider)
      {
        DrawCapsuleCollider(collider as CapsuleCollider, color);
      }
    }

    /// <summary>
    /// Draws a box collider
    /// </summary>
    /// <param name="boxCollider">Box collider to draw</param>
    /// <param name="color">Color of lines used to draw</param>
    private static void DrawBoxCollider(BoxCollider boxCollider, Color color)
    {
      Vector3 center = boxCollider.center;
      Vector3 size = boxCollider.size / 2;
      Vector3 p1 = boxCollider.transform.TransformPoint(center + size);
      Vector3 p2 = boxCollider.transform.TransformPoint(center - size);
      Vector3 p3 = boxCollider.transform.TransformPoint(center + new Vector3(size.x, size.y, -size.z));
      Vector3 p4 = boxCollider.transform.TransformPoint(center + new Vector3(size.x, -size.y, size.z));
      Vector3 p5 = boxCollider.transform.TransformPoint(center + new Vector3(size.x, -size.y, -size.z));
      Vector3 p6 = boxCollider.transform.TransformPoint(center + new Vector3(-size.x, size.y, size.z));
      Vector3 p7 = boxCollider.transform.TransformPoint(center + new Vector3(-size.x, -size.y, size.z));
      Vector3 p8 = boxCollider.transform.TransformPoint(center + new Vector3(-size.x, size.y, -size.z));
      Handles.color = color;
      Handles.DrawLine(p1, p3);
      Handles.DrawLine(p1, p4);
      Handles.DrawLine(p1, p6);
      Handles.DrawLine(p8, p3);
      Handles.DrawLine(p8, p6);
      Handles.DrawLine(p8, p2);
      Handles.DrawLine(p7, p6);
      Handles.DrawLine(p7, p2);
      Handles.DrawLine(p7, p4);
      Handles.DrawLine(p5, p4);
      Handles.DrawLine(p5, p2);
      Handles.DrawLine(p5, p3);
    }

    // Draws a sphere collider, taken from previous version
    /// <summary>
    /// Draws a sphere collider.
    /// </summary>
    /// <param name="sphereCollider">Sphere collider to draw</param>
    /// <param name="color">Color of lines used to draw</param>
    private static void DrawSphereCollider(SphereCollider sphereCollider, Color color)
    {
      Handles.color = color;
      Vector3 center = sphereCollider.transform.TransformPoint(sphereCollider.center);
      float radius = sphereCollider.radius;
      Handles.DrawWireDisc(center, sphereCollider.transform.up, radius);
      Handles.DrawWireDisc(center, sphereCollider.transform.right, radius);
      Handles.DrawWireDisc(center, sphereCollider.transform.forward, radius);
    }


    /// <summary>
    /// Draws a capsule collider
    /// </summary>
    /// <param name="capsuleCollider">Capsule collider to draw</param>
    /// <param name="color">Color of lines to draw</param>
    private static void DrawCapsuleCollider(CapsuleCollider capsuleCollider, Color color)
    {
      Handles.color = color;
      Vector3 center = capsuleCollider.center;
      float radius = capsuleCollider.radius;
      float height = capsuleCollider.height;
      int direction = capsuleCollider.direction;
      float length = (height / 2 - radius);
      Vector3 p1 = Vector3.zero, p2, p3, p4, p5, p6, p7, p8;// = Vector3.zero;
      p1 = p2 = p3 = p4 = p5 = p6 = p7 = p8 = Vector3.zero;
      Vector3 centerTop = Vector3.zero;
      Vector3 centerBottom = Vector3.zero;
      if (direction == 0)
      {
        p1 = new Vector3(center.x - length, center.y + radius, center.z);
        p2 = new Vector3(center.x + length, center.y + radius, center.z);
        p3 = new Vector3(center.x + length, center.y, center.z + radius);
        p4 = new Vector3(center.x - length, center.y, center.z + radius);
        p5 = new Vector3(center.x + length, center.y, center.z - radius);
        p6 = new Vector3(center.x - length, center.y, center.z - radius);
        p7 = new Vector3(center.x - length, center.y - radius, center.z);
        p8 = new Vector3(center.x + length, center.y - radius, center.z);
        centerTop = new Vector3(center.x + length, center.y, center.z);
        centerBottom = new Vector3(center.x - length, center.y, center.z);
      }
      if (direction == 1)
      {
        p1 = new Vector3(center.x + radius, center.y - length, center.z);
        p2 = new Vector3(center.x + radius, center.y + length, center.z);
        p3 = new Vector3(center.x, center.y - length, center.z + radius);
        p4 = new Vector3(center.x, center.y + length, center.z + radius);
        p5 = new Vector3(center.x, center.y - length, center.z - radius);
        p6 = new Vector3(center.x, center.y + length, center.z - radius);
        p7 = new Vector3(center.x - radius, center.y - length, center.z);
        p8 = new Vector3(center.x - radius, center.y + length, center.z);
        centerTop = new Vector3(center.x, center.y + length, center.z);
        centerBottom = new Vector3(center.x, center.y - length, center.z);
      }
      if (direction == 2)
      {
        p1 = new Vector3(center.x + radius, center.y, center.z + length);
        p2 = new Vector3(center.x + radius, center.y, center.z - length);
        p3 = new Vector3(center.x, center.y + radius, center.z + length);
        p4 = new Vector3(center.x, center.y + radius, center.z - length);
        p5 = new Vector3(center.x, center.y - radius, center.z + length);
        p6 = new Vector3(center.x, center.y - radius, center.z - length);
        p7 = new Vector3(center.x - radius, center.y, center.z + length);
        p8 = new Vector3(center.x - radius, center.y, center.z - length);
        centerTop = new Vector3(center.x, center.y, center.z + length);
        centerBottom = new Vector3(center.x, center.y, center.z - length);
      } //
      p1 = capsuleCollider.transform.TransformPoint(p1);
      p2 = capsuleCollider.transform.TransformPoint(p2);
      p3 = capsuleCollider.transform.TransformPoint(p3);
      p4 = capsuleCollider.transform.TransformPoint(p4);
      p5 = capsuleCollider.transform.TransformPoint(p5);
      p6 = capsuleCollider.transform.TransformPoint(p6);
      p7 = capsuleCollider.transform.TransformPoint(p7);
      p8 = capsuleCollider.transform.TransformPoint(p8);
      Handles.DrawLine(p1, p2);
      Handles.DrawLine(p3, p4);
      Handles.DrawLine(p5, p6);
      Handles.DrawLine(p7, p8);
      centerTop = capsuleCollider.transform.TransformPoint(centerTop);
      centerBottom = capsuleCollider.transform.TransformPoint(centerBottom);
      Handles.DrawWireDisc(centerTop, capsuleCollider.transform.right, radius);
      Handles.DrawWireDisc(centerTop, capsuleCollider.transform.up, radius);
      Handles.DrawWireDisc(centerTop, capsuleCollider.transform.forward, radius);
      Handles.DrawWireDisc(centerBottom, capsuleCollider.transform.right, radius);
      Handles.DrawWireDisc(centerBottom, capsuleCollider.transform.up, radius);
      Handles.DrawWireDisc(centerBottom, capsuleCollider.transform.forward, radius);
    }
  }
}
#endif