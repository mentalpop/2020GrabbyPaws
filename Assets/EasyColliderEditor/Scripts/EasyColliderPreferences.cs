#if (UNITY_EDITOR)
using UnityEngine;
using UnityEditor;
namespace ECE
{
  [System.Serializable]
  public class EasyColliderPreferences : ScriptableObject
  {
    // Auto include child skinned meshes
    [SerializeField] public bool AutoIncludeChildSkinnedMeshes;

    // Key to hold before box selection to only add vertices in the box.
    [SerializeField] public KeyCode BoxSelectPlusKey;

    // Key to hold before box selection to only remove vertices in the box.
    [SerializeField] public KeyCode BoxSelectMinusKey;

    // Capsule collider generation method to use when creating a capsule collider.
    [SerializeField] private CAPSULE_COLLIDER_METHOD _capsuleColliderMethod;
    public CAPSULE_COLLIDER_METHOD CapsuleColliderMethod { get { return _capsuleColliderMethod; } set { _capsuleColliderMethod = value; } }

    // Created colliders automatically get disabled to make vertex selection easier.
    [SerializeField] public bool CreatedColliderDisabled;

    // display tips
    [SerializeField] public bool DisplayTips;

    //display vertices colour
    [SerializeField] public Color DisplayVerticesColour;

    //display vertices scaling size
    [SerializeField] public float DisplayVerticesScaling;

    // force focus scene
    [SerializeField] public bool ForceFocusScene;

    // gizmo type
    [SerializeField] private GIZMO_TYPE _gizmoType;
    public GIZMO_TYPE GizmoType { get { return _gizmoType; } set { _gizmoType = value; } }

    //hover vertices scaling colour
    [SerializeField] public Color HoverVertColour;

    //hover vertices scaling size
    [SerializeField] public float HoverVertScaling;

    //overlapped vertice scaling colour
    [SerializeField] public Color OverlapSelectedVertColour;

    //overlapped selected vertex scale
    [SerializeField] public float OverlapSelectedVertScale;

    // Raycast delay time
    [SerializeField] public float RaycastDelayTime;

    // Render point method
    [SerializeField] public RENDER_POINT_TYPE RenderPointType;

    //If true, puts rotated colliders on the same layer as the selected gameobject.
    [SerializeField] public bool RotatedOnSelectedLayer;

    // Saves convex hull's mesh at the same path as the selected gameobject if true
    [SerializeField] public bool SaveConvexHullMeshAtSelected;

    // if SaveConvexHullMeshAtSelected is false, saves at the path specified.
    [SerializeField] public string SaveConvexHullPath;

    // Color to draw the select collider lines/handles.
    [SerializeField] public Color SelectedColliderColour;

    //selected vertice scaling colour
    [SerializeField] public Color SelectedVertColour;

    //selected vertice scaling size
    [SerializeField] public float SelectedVertScaling;
    // public float SelectedVertScaling { get { return _SelectedVertScaling; } set { _SelectedVertScaling = value; } }

    // Sphere method to use when creating a sphere collider.
    [SerializeField] private SPHERE_COLLIDER_METHOD _sphereColliderMethod;
    public SPHERE_COLLIDER_METHOD SphereColliderMethod { get { return _sphereColliderMethod; } set { _sphereColliderMethod = value; } }

    [SerializeField] public bool UseFixedGizmoScale;

    //Key used to select vertices.
    [SerializeField] public KeyCode VertSelectKeyCode;

    [SerializeField] public KeyCode PointSelectKeyCode;

    public void SetDefaultValues()
    {
      AutoIncludeChildSkinnedMeshes = true;

      // shifts do not work.
      BoxSelectMinusKey = KeyCode.Z;
      BoxSelectPlusKey = KeyCode.A;

      CapsuleColliderMethod = CAPSULE_COLLIDER_METHOD.MinMax;

      CreatedColliderDisabled = true;

      DisplayTips = true;

      DisplayVerticesColour = Color.blue;
      DisplayVerticesScaling = 0.05f;

      GizmoType = GIZMO_TYPE.SPHERE;

      ForceFocusScene = true;

      HoverVertColour = Color.cyan;
      HoverVertScaling = 0.1F;

      OverlapSelectedVertColour = Color.red;
      OverlapSelectedVertScale = 0.1f;

      PointSelectKeyCode = KeyCode.B;

      RaycastDelayTime = 0.1f;

      if (SystemInfo.graphicsShaderLevel < 45)
      {
        RenderPointType = RENDER_POINT_TYPE.GIZMOS;
      }
      else
      {
        RenderPointType = RENDER_POINT_TYPE.SHADER;
      }

      RotatedOnSelectedLayer = true;

      SaveConvexHullMeshAtSelected = true;
      SaveConvexHullPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
      SaveConvexHullPath = SaveConvexHullPath.Remove(SaveConvexHullPath.LastIndexOf("/")) + "/";

      SelectedColliderColour = Color.red;

      SelectedVertColour = Color.green;
      SelectedVertScaling = 0.1f;

      SphereColliderMethod = SPHERE_COLLIDER_METHOD.MinMax;

      UseFixedGizmoScale = true;

      VertSelectKeyCode = KeyCode.V;
    }
  }
}
#endif