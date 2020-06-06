#if (UNITY_EDITOR)
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace ECE
{
  public class EasyColliderCreator
  {

    /// <summary>
    /// Creates a gameobject attach to parent with it's local position at zero, and it's up direction oriented in the direction of the first 2 world vertices.
    /// </summary>
    /// <param name="worldVertices">List of world space vertices</param>
    /// <param name="parent">Parent to attach gameobject to</param>
    /// <param name="name">Name of gameobject to create</param>
    /// <returns></returns>
    private GameObject CreateGameObjectOrientation(List<Vector3> worldVertices, GameObject parent, string name)
    {
      GameObject obj = new GameObject(name);
      if (worldVertices.Count >= 3)
      {
        // calculate forward and up.
        Vector3 forward = worldVertices[1] - worldVertices[0];
        Vector3 up = Vector3.Cross(forward, worldVertices[2] - worldVertices[1]);
        obj.transform.rotation = Quaternion.LookRotation(forward, up);
        obj.transform.SetParent(parent.transform);
        obj.transform.localPosition = Vector3.zero;
        Undo.RegisterCreatedObjectUndo(obj, "Create Rotated GameObject");
        return obj;
      }
      return null;
    }

    /// <summary>
    /// Creates a box collider by calculating the min and max x, y, and z.
    /// </summary>
    /// <param name="worldVertices">List of world space vertices</param>
    /// <param name="properties">Properties of collider</param>
    /// <returns></returns>
    public Collider CreateBoxCollider(List<Vector3> worldVertices, EasyColliderProperties properties)
    {
      if (worldVertices.Count >= 2)
      {
        if (properties.Orientation == COLLIDER_ORIENTATION.ROTATED)
        {
          if (worldVertices.Count >= 3)
          {
            GameObject obj = CreateGameObjectOrientation(worldVertices, properties.AttachTo, "Rotated Box Collider");
            if (obj != null)
            {
              obj.layer = properties.Layer;
              properties.AttachTo = obj;
            }
          }
          else
          {
            Debug.LogWarning("Easy Collider Editor: Creating a Rotated Box Collider requires at least 3 points to be selected.");
          }
        }
        List<Vector3> localVertices = ToLocalVerts(properties.AttachTo.transform, worldVertices);
        float xMin, yMin, zMin = xMin = yMin = Mathf.Infinity;
        float xMax, yMax, zMax = xMax = yMax = -Mathf.Infinity;
        foreach (Vector3 vertex in localVertices)
        {
          //x min & max.
          xMin = (vertex.x < xMin) ? vertex.x : xMin;
          xMax = (vertex.x > xMax) ? vertex.x : xMax;
          //y min & max
          yMin = (vertex.y < yMin) ? vertex.y : yMin;
          yMax = (vertex.y > yMax) ? vertex.y : yMax;
          //z min & max
          zMin = (vertex.z < zMin) ? vertex.z : zMin;
          zMax = (vertex.z > zMax) ? vertex.z : zMax;
        }
        Vector3 max = new Vector3(xMax, yMax, zMax);
        Vector3 min = new Vector3(xMin, yMin, zMin);
        Vector3 size = max - min;
        Vector3 center = (max + min) / 2;
        BoxCollider boxCollider = Undo.AddComponent<BoxCollider>(properties.AttachTo);
        boxCollider.size = size;
        boxCollider.center = center;
        SetPropertiesOnCollider(boxCollider, properties);
        return boxCollider;
      }
      return null;
    }

    /// <summary>
    /// Creates a sphere collider by calculating the min and max in x, y, and z.
    /// </summary>
    /// <param name="worldVertices">List of world space vertices</param>
    /// <param name="properties">Properties of collider</param>
    /// <returns></returns>
    public Collider CreateSphereCollider_MinMax(List<Vector3> worldVertices, EasyColliderProperties properties)
    {
      if (worldVertices.Count >= 2)
      {
        // use local space verts.
        List<Vector3> localVertices = ToLocalVerts(properties.AttachTo.transform, worldVertices);
        float xMin, yMin, zMin = xMin = yMin = Mathf.Infinity;
        float xMax, yMax, zMax = xMax = yMax = -Mathf.Infinity;
        for (int i = 0; i < localVertices.Count; i++)
        {
          //x min & max.
          xMin = (localVertices[i].x < xMin) ? localVertices[i].x : xMin;
          xMax = (localVertices[i].x > xMax) ? localVertices[i].x : xMax;
          //y min & max
          yMin = (localVertices[i].y < yMin) ? localVertices[i].y : yMin;
          yMax = (localVertices[i].y > yMax) ? localVertices[i].y : yMax;
          //z min & max
          zMin = (localVertices[i].z < zMin) ? localVertices[i].z : zMin;
          zMax = (localVertices[i].z > zMax) ? localVertices[i].z : zMax;
        }
        // calculate center
        Vector3 center = (new Vector3(xMin, yMin, zMin) + new Vector3(xMax, yMax, zMax)) / 2;
        SphereCollider sphereCollider = Undo.AddComponent<SphereCollider>(properties.AttachTo);
        sphereCollider.center = center;
        // calculate radius to contain all points
        float maxDistance = 0.0f;
        float distance = 0.0f;
        foreach (Vector3 vertex in localVertices)
        {
          distance = Vector3.Distance(vertex, center);
          if (distance > maxDistance)
          {
            maxDistance = distance;
          }
        }
        sphereCollider.radius = maxDistance;
        SetPropertiesOnCollider(sphereCollider, properties);
        return sphereCollider;
      }
      return null;
    }

    /// <summary>
    /// Creates a sphere collider using the best fit sphere algorithm.
    /// </summary>
    /// <param name="worldVertices">List of world space vertices</param>
    /// <param name="properties">Properties of collider</param>
    /// <returns></returns>
    public Collider CreateSphereCollider_BestFit(List<Vector3> worldVertices, EasyColliderProperties properties)
    {
      if (worldVertices.Count >= 2)
      {
        // Convert to local space.
        List<Vector3> localVertices = ToLocalVerts(properties.AttachTo.transform, worldVertices);
        BestFitSphere bfs = CalculateBestFitSphere(localVertices);
        SphereCollider sphereCollider = Undo.AddComponent<SphereCollider>(properties.AttachTo);
        sphereCollider.radius = bfs.Radius;
        sphereCollider.center = bfs.Center;
        SetPropertiesOnCollider(sphereCollider, properties);
        return sphereCollider;
      }
      return null;
    }

    /// <summary>
    /// Creates a Sphere Collider by finding the 2 points with a maximum distance between them.
    /// </summary>
    /// <param name="worldVertices">List of world space vertices</param>
    /// <param name="properties">Properties of collider</param>
    /// <returns></returns>
    public Collider CreateSphereCollider_Distance(List<Vector3> worldVertices, EasyColliderProperties properties)
    {
      // with how min-max works, i dont think this is needed.
      // Future work: instead of switching to faster algorithm
      // calculating the convex hull from the list of points, and then using those points is nlogn
      // whereas our slow algorithm is n^2, and the fast one is n.
      if (worldVertices.Count >= 2)
      {
        // // if calculations take to long, it switches to a faster less accurate algorithm using the mean.
        bool switchToFasterAlgorithm = false;
        double startTime = EditorApplication.timeSinceStartup;
        double maxTime = 0.1f;
        List<Vector3> localVertices = ToLocalVerts(properties.AttachTo.transform, worldVertices);
        Vector3 distanceVert1 = Vector3.zero;
        Vector3 distanceVert2 = Vector3.zero;
        float maxDistance = -Mathf.Infinity;
        float distance = 0;
        for (int i = 0; i < localVertices.Count; i++)
        {
          for (int j = i + 1; j < localVertices.Count; j++)
          {
            distance = Vector3.Distance(localVertices[i], localVertices[j]);
            if (distance > maxDistance)
            {
              maxDistance = distance;
              distanceVert1 = localVertices[i];
              distanceVert2 = localVertices[j];
            }
          }
          if (EditorApplication.timeSinceStartup - startTime > maxTime)
          {
            switchToFasterAlgorithm = true;
            break;
          }
        }

        if (switchToFasterAlgorithm)
        {
          // use a significantly faster algorithm that is less accurate for a large # of points.
          Vector3 mean = Vector3.zero;
          foreach (Vector3 vertex in localVertices)
          {
            mean += vertex;
          }
          mean = mean / localVertices.Count;
          foreach (Vector3 vertex in localVertices)
          {
            distance = Vector3.Distance(vertex, mean);
            if (distance > maxDistance)
            {
              distanceVert1 = vertex;
              maxDistance = distance;
            }
          }
          maxDistance = -Mathf.Infinity;
          foreach (Vector3 vertex in localVertices)
          {
            distance = Vector3.Distance(vertex, distanceVert1);
            if (distance > maxDistance)
            {
              maxDistance = distance;
              distanceVert2 = vertex;
            }
          }
        }

        Vector3 center = (distanceVert1 + distanceVert2) / 2;
        SphereCollider sphereCollider = Undo.AddComponent<SphereCollider>(properties.AttachTo);
        sphereCollider.radius = maxDistance / 2;
        sphereCollider.center = center;
        SetPropertiesOnCollider(sphereCollider, properties);
        return sphereCollider;
      }
      return null;
    }


    /// <summary>
    /// Creates a capsule collider using the height from first 2 vertices, and then getting radius from the best fit sphere algorithm.
    /// </summary>
    /// <param name="worldVertices">List of world vertices</param>
    /// <param name="properties">Properties of collider</param>
    /// <returns></returns>
    public Collider CreateCapsuleCollider_BestFit(List<Vector3> worldVertices, EasyColliderProperties properties)
    {
      if (worldVertices.Count >= 3)
      {
        if (properties.Orientation == COLLIDER_ORIENTATION.ROTATED)
        {
          GameObject obj = CreateGameObjectOrientation(worldVertices, properties.AttachTo, "Rotated Capsule Collider");
          if (obj != null)
          {
            properties.AttachTo = obj;
          }
        }
        // use local verts.
        List<Vector3> localVertices = ToLocalVerts(properties.AttachTo.transform, worldVertices);
        // height from first 2 verts selected.
        Vector3 v0 = localVertices[0];
        Vector3 v1 = localVertices[1];
        float height = Vector3.Distance(v0, v1);
        float dX = Mathf.Abs(v1.x - v0.x);
        float dY = Mathf.Abs(v1.y - v0.y);
        float dZ = Mathf.Abs(v1.z - v0.z);
        localVertices.RemoveAt(1);
        localVertices.RemoveAt(0);
        // radius from best fit sphere of the rest of the vertices.
        BestFitSphere bfs = CalculateBestFitSphere(localVertices);
        CapsuleCollider capsuleCollider = Undo.AddComponent<CapsuleCollider>(properties.AttachTo);
        Vector3 center = bfs.Center;
        if (dX > dY && dX > dZ)
        {
          capsuleCollider.direction = 0;
          center.x = (v1.x + v0.x) / 2;
        }
        else if (dY > dX && dY > dZ)
        {
          capsuleCollider.direction = 1;
          center.y = (v1.y + v0.y) / 2;
        }
        else
        {
          capsuleCollider.direction = 2;
          center.z = (v1.z + v0.z) / 2;
        }
        capsuleCollider.center = center;
        capsuleCollider.height = height;
        capsuleCollider.radius = bfs.Radius;
        SetPropertiesOnCollider(capsuleCollider, properties);
        return capsuleCollider;
      }
      return null;
    }

    /// <summary>
    /// Creates a capsule collider using the Min-Max method
    /// </summary>
    /// <param name="worldVertices">List of world space vertices</param>
    /// <param name="properties">Properties to set on collider</param>
    /// <param name="method">Min-Max method to use to add radius' to height.</param>
    /// <returns></returns>
    public Collider CreateCapsuleCollider_MinMax(List<Vector3> worldVertices, EasyColliderProperties properties, CAPSULE_COLLIDER_METHOD method)
    {
      if (properties.Orientation == COLLIDER_ORIENTATION.ROTATED)
      {
        GameObject obj = CreateGameObjectOrientation(worldVertices, properties.AttachTo, "Rotated Capsule Collider");
        if (obj != null)
        {
          properties.AttachTo = obj;
        }
      }
      List<Vector3> localVertices = ToLocalVerts(properties.AttachTo.transform, worldVertices);

      // calculate min and max points from vertices.
      Vector3 min = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
      Vector3 max = new Vector3(-Mathf.Infinity, -Mathf.Infinity, -Mathf.Infinity);
      foreach (Vector3 vertex in localVertices)
      {
        // Calc minimums
        min.x = vertex.x < min.x ? vertex.x : min.x;
        min.y = vertex.y < min.y ? vertex.y : min.y;
        min.z = vertex.z < min.z ? vertex.z : min.z;
        // Calc maximums
        max.x = vertex.x > max.x ? vertex.x : max.x;
        max.y = vertex.y > max.y ? vertex.y : max.y;
        max.z = vertex.z > max.z ? vertex.z : max.z;
      }
      // Deltas for max-min
      float dX = max.x - min.x;
      float dY = max.y - min.y;
      float dZ = max.z - min.z;
      CapsuleCollider capsuleCollider = Undo.AddComponent<CapsuleCollider>(properties.AttachTo);
      // center is between min and max values.
      Vector3 center = (max + min) / 2;
      // set center.
      capsuleCollider.center = center;
      // set direction and height.
      if (dX > dY && dX > dZ) // direction is x
      {
        capsuleCollider.direction = 0;
        // height is the max difference in x.
        capsuleCollider.height = dX;
      }
      else if (dY > dX && dY > dZ) // direction is y
      {
        capsuleCollider.direction = 1;
        capsuleCollider.height = dY;
      }
      else // direction is z.
      {
        capsuleCollider.direction = 2;
        capsuleCollider.height = dZ;
      }

      // Calculate radius, makes sure that all vertices are within the radius.
      // Esentially to points on plane defined by direction axis, and find the furthest distance.
      float maxRadius = -Mathf.Infinity;
      Vector3 current = Vector3.zero;
      foreach (Vector3 vertex in localVertices)
      {
        current = vertex;
        if (capsuleCollider.direction == 0)
        {
          current.x = center.x;
        }
        else if (capsuleCollider.direction == 1)
        {
          current.y = center.y;
        }
        else if (capsuleCollider.direction == 2)
        {
          current.z = center.z;
        }
        float d = Vector3.Distance(current, center);
        if (d > maxRadius)
        {
          maxRadius = d;
        }
      }
      capsuleCollider.radius = maxRadius;
      // method add radius / diameter
      if (method == CAPSULE_COLLIDER_METHOD.MinMaxPlusRadius)
      {
        capsuleCollider.height += capsuleCollider.radius;
      }
      else if (method == CAPSULE_COLLIDER_METHOD.MinMaxPlusDiameter)
      {
        capsuleCollider.height += capsuleCollider.radius * 2;
      }
      // set properties
      SetPropertiesOnCollider(capsuleCollider, properties);
      return capsuleCollider;
    }

    /// <summary>
    /// Sets the collider properties isTrigger and physicMaterial.
    /// </summary>
    /// <param name="collider">Collider to set properties on</param>
    /// <param name="properties">Properties object with the properties you want to set</param>
    private void SetPropertiesOnCollider(Collider collider, EasyColliderProperties properties)
    {
      collider.isTrigger = properties.IsTrigger;
      collider.sharedMaterial = properties.PhysicMaterial;
    }

    private struct BestFitSphere
    {
      public Vector3 Center;
      public float Radius;
      public BestFitSphere(Vector3 center, float radius)
      {
        this.Center = center;
        this.Radius = radius;
      }
    }

    /// <summary>
    /// Calculates the best fit sphere for a series of points. Providing a larger list of points increases accuracy.
    /// </summary>
    /// <param name="localVertices">Local space vertices</param>
    /// <returns>The best fit sphere</returns>
    private BestFitSphere CalculateBestFitSphere(List<Vector3> localVertices)
    {
      // # of points.
      int n = localVertices.Count;
      // Calculate average x, y, and z value of vertices.
      float xAvg, yAvg, zAvg = xAvg = yAvg = 0.0f;
      foreach (Vector3 vertex in localVertices)
      {
        xAvg += vertex.x;
        yAvg += vertex.y;
        zAvg += vertex.z;
      }
      xAvg = xAvg * (1.0f / n);
      yAvg = yAvg * (1.0f / n);
      zAvg = zAvg * (1.0f / n);
      // Do some fun math with matrices
      // B Vector.
      Vector3 B = Vector3.zero;
      // Can use a 4x4 as a 3x3 with the 4x4 as 0,0,0,1 in the last row/column.
      Matrix4x4 AM = new Matrix4x4(Vector4.zero, Vector4.zero, Vector4.zero, new Vector4(0, 0, 0, 1));
      float x2, y2, z2 = x2 = y2 = 0.0f;
      foreach (Vector3 vertex in localVertices)
      {
        AM[0, 0] += 2 * (vertex.x * (vertex.x - xAvg)) / n;
        AM[0, 1] += 2 * (vertex.x * (vertex.y - yAvg)) / n;
        AM[0, 2] += 2 * (vertex.x * (vertex.z - zAvg)) / n;
        AM[1, 0] += 2 * (vertex.y * (vertex.x - xAvg)) / n;
        AM[1, 1] += 2 * (vertex.y * (vertex.y - yAvg)) / n;
        AM[1, 2] += 2 * (vertex.y * (vertex.z - zAvg)) / n;
        AM[2, 0] += 2 * (vertex.z * (vertex.x - xAvg)) / n;
        AM[2, 1] += 2 * (vertex.z * (vertex.y - yAvg)) / n;
        AM[2, 2] += 2 * (vertex.z * (vertex.z - zAvg)) / n;
        x2 = vertex.x * vertex.x;
        y2 = vertex.y * vertex.y;
        z2 = vertex.z * vertex.z;
        B.x += ((x2 + y2 + z2) * (vertex.x - xAvg)) / n;
        B.y += ((x2 + y2 + z2) * (vertex.y - yAvg)) / n;
        B.z += ((x2 + y2 + z2) * (vertex.z - zAvg)) / n;
      }
      // Calculate the center of the best-fit sphere.
      Vector3 center = (AM.transpose * AM).inverse * AM.transpose * B;
      // Calculate radius.
      float radius = 0.0f;
      foreach (Vector3 vertex in localVertices)
      {
        radius += Mathf.Pow((vertex.x - center.x), 2) + Mathf.Pow(vertex.y - center.y, 2) + Mathf.Pow(vertex.z - center.z, 2);
      }
      radius = Mathf.Sqrt(radius / localVertices.Count);
      BestFitSphere bfs = new BestFitSphere(center, radius);
      return bfs;
    }

    // LeastSquaresCylinder (page 36) : As described in Apendix B.
    // to bounds the points Xi(i = 0->n):
    // 1. fit points by a line using the least squares algorithm described in appendix B.
    // 2. let the line be A-> + t*W-> where:
    // W-> is the unit length
    // A-> is the average of the data points
    // compute r to be the maximum distance from the datapoints to the line.
    // Select unit vectors U-> and V-> so that the matrix R = [U->   V->   W->] is orthonormal and has determinant one
    // the datapoints can be represented as X->i = A-> + R*Yi-> where Yi-> = (ui, vi, wi)
    // in the u, v, w coordinate system, the capsule axis is contained by the line t(0,0,1).
    // need to compute large Eo so that all points lie above the hemisphere u^2 + v^2 + (w - Eo)^2 = r^2 with w <=Eo
    // Eo = min(wi + sqrt(r^2 - (ui^2 + vi^2) ) )
    // where 0 <= i <= n
    // similarly there is a smallest E1 so that all points lie below the hemisphere u^2 + v^2 + (w - E1)^2 = r^2 with w>= E1
    // the value is computed as E1 = max(wi + sqrt(r^2 - (ui^2 + vi^2) ) )
    // the end points of the capsule line segment are ->Pj = ->A + Ej*->W for j = 0;
    // if instead the data points are fit by a least squares plane W*(->X - ->A) = 0 the result is the same since the unit length plane normal W-> is exactly the line direction.


    /// <summary>
    /// Converts the list of world vertices to local positions
    /// </summary>
    /// <param name="transform">Transform to use for local space</param>
    /// <param name="worldVertices">World space position of vertices</param>
    /// <returns>Localspace position w.r.t transform of worldVertices</returns>
    private List<Vector3> ToLocalVerts(Transform transform, List<Vector3> worldVertices)
    {
      List<Vector3> localVerts = new List<Vector3>(worldVertices.Count);
      foreach (Vector3 v in worldVertices)
      {
        localVerts.Add(transform.InverseTransformPoint(v));
      }
      return localVerts;
    }
  }
}
#endif