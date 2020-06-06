#if (UNITY_EDITOR)
using System;
using UnityEngine;
namespace ECE
{
  /// <summary>
  /// A vertex represented by the transform it's attached to and it's local position.
  /// </summary>
  [System.Serializable]
  public class EasyColliderVertex : IEquatable<EasyColliderVertex>
  {
    /// It doesn't matter if the mesh is different (for our purposes) if the transform and local position are the same.

    /// <summary>
    /// Transform the vertex comes from.
    /// </summary>
    public Transform T;
    /// <summary>
    /// Local position of the vertex on the transform.
    /// </summary>
    public Vector3 LocalPosition;

    public EasyColliderVertex(Transform transform, Vector3 localPosition)
    {
      this.T = transform;
      this.LocalPosition = localPosition;
    }

    public bool Equals(EasyColliderVertex other)
    {
      return (other.LocalPosition == this.LocalPosition && other.T == this.T);
    }

    public override int GetHashCode()
    {
      int hashCode = 13 * 31 + LocalPosition.GetHashCode();
      return hashCode * 31 + T.GetHashCode();
    }

  }
}
#endif