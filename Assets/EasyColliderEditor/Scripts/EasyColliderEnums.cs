#if (UNITY_EDITOR)
namespace ECE
{
  /// <summary>
  /// enum for Shaders or Gizmos to draw vertices with.
  /// </summary>
  public enum RENDER_POINT_TYPE
  {
    SHADER,
    GIZMOS,
  }

  /// <summary>
  /// Enum for spheres ot cubes when drawing gizmos
  /// </summary>
  public enum GIZMO_TYPE
  {
    CUBE,
    SPHERE,

  }

  /// <summary>
  /// Type of collider to create
  /// </summary>
  public enum CREATE_COLLIDER_TYPE
  {
    BOX,
    ROTATED_BOX,
    SPHERE,
    CAPSULE,
    ROTATED_CAPSULE,
  }

  /// <summary>
  /// Orientation of collider
  /// </summary>
  public enum COLLIDER_ORIENTATION
  {
    NORMAL,
    ROTATED,
  }

  /// <summary>
  /// Sphere method to use when creating a sphere
  /// </summary>
  public enum SPHERE_COLLIDER_METHOD
  {
    BestFit,
    Distance,
    MinMax,
  }

  /// <summary>
  /// Capsule method to use when creating a capsule
  /// </summary>
  public enum CAPSULE_COLLIDER_METHOD
  {
    BestFit,
    MinMax,
    MinMaxPlusRadius,
    MinMaxPlusDiameter,
  }
}
#endif