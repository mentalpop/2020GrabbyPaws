#if (UNITY_EDITOR)
using UnityEngine;
namespace ECE
{
  /// <summary>
  /// Properties to use when creating a collider.
  /// </summary>
  public struct EasyColliderProperties
  {
    /// <summary>
    /// Marks the collider's isTrigger property
    /// </summary>
    public bool IsTrigger;

    /// <summary>
    /// Layer of gameobject when creating a rotated collider.
    /// </summary>
    public int Layer;

    /// <summary>
    /// Physic material to set on collider.
    /// </summary>
    public PhysicMaterial PhysicMaterial;

    /// <summary>
    /// Orientation of created collider.
    /// </summary>
    public COLLIDER_ORIENTATION Orientation;

    /// <summary>
    /// Gameobject collider gets added to.
    /// </summary>
    public GameObject AttachTo;
    public EasyColliderProperties(bool isTrigger, int layer, PhysicMaterial physicMaterial, GameObject attachTo, COLLIDER_ORIENTATION orientation = COLLIDER_ORIENTATION.NORMAL)
    {
      IsTrigger = isTrigger;
      Layer = layer;
      PhysicMaterial = physicMaterial;
      AttachTo = attachTo;
      Orientation = orientation;
    }
  }
}
#endif