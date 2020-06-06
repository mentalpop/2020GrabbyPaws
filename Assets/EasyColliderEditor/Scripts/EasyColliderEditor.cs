#if (UNITY_EDITOR)
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
namespace ECE
{
  [System.Serializable]
  public class EasyColliderEditor : ScriptableObject, ISerializationCallbackReceiver
  {
    [SerializeField]
    private List<Component> _AddedComponents;
    /// <summary>
    /// List of Components added for functionality.
    /// </summary>
    /// <value></value>
    private List<Component> AddedComponents
    {
      get
      {
        if (_AddedComponents == null)
        {
          _AddedComponents = new List<Component>();
        }
        return _AddedComponents;
      }
      set { _AddedComponents = value; }
    }

    [SerializeField]
    private GameObject _AttachToObject;
    /// <summary>
    /// If different from the selected gameobject, the attach to object is used to convert to local vertices / attach the collider to.
    /// </summary>
    public GameObject AttachToObject
    {
      get
      {
        if (_AttachToObject == null)
        {
          return SelectedGameObject;
        }
        return _AttachToObject;
      }
      set { _AttachToObject = value; }
    }

    [SerializeField]
    private bool _AutoIncludeChildSkinnedMeshes;
    /// <summary>
    /// Are we automatically including child skinned meshes when include child meshes is enabled?
    /// </summary>
    public bool AutoIncludeChildSkinnedMeshes { get { return _AutoIncludeChildSkinnedMeshes; } set { _AutoIncludeChildSkinnedMeshes = value; } }

    [SerializeField]
    private bool _ColliderSelectEnabled;
    /// <summary>
    /// Is Collider Selection Enabled? Toggles colliders on and off when changed.
    /// </summary>
    public bool ColliderSelectEnabled
    {
      get { return _ColliderSelectEnabled; }
      set
      {
        ToggleCollidersEnabled(value);
        _ColliderSelectEnabled = value;
      }
    }

    [SerializeField]
    private EasyColliderCompute _Compute;
    /// <summary>
    /// Compute shader script
    /// </summary>
    public EasyColliderCompute Compute
    {
      get
      {
        if (_Compute == null && SelectedGameObject != null)
        {
          _Compute = SelectedGameObject.GetComponent<EasyColliderCompute>();
        }
        return _Compute;
      }
      set
      {
        _Compute = value;
        if (value != null && DisplayMeshVertices)
        {
          _Compute.SetDisplayAllBuffer(GetAllWorldMeshVertices());
          _Compute.DisplayAllVertices = DisplayMeshVertices;
        }
      }
    }

    [SerializeField]
    private List<Collider> _CreatedColliders;
    /// <summary>
    /// List of colliders we created
    /// </summary>
    public List<Collider> CreatedColliders
    {
      get
      {
        if (_CreatedColliders == null)
        {
          _CreatedColliders = new List<Collider>();
        }
        return _CreatedColliders;
      }
      set { _CreatedColliders = value; }
    }

    [SerializeField]
    private bool _CreatedCollidersDisabled;
    /// <summary>
    /// Are the colliders we create immediately marked as disabled until we're done with the current gameobject?
    /// </summary>
    public bool CreatedColliderDisabled { get { return _CreatedCollidersDisabled; } set { _CreatedCollidersDisabled = value; } }

    [SerializeField]
    private List<Collider> _DisabledColliders;
    /// <summary>
    /// Colliders that we disabled during setup.
    /// </summary>
    public List<Collider> DisabledColliders
    {
      get
      {
        if (_DisabledColliders == null)
        {
          _DisabledColliders = new List<Collider>();
        }
        return _DisabledColliders;
      }
      set { _DisabledColliders = value; }
    }

    [SerializeField]
    private bool _DisplayMeshVertices;
    public bool DisplayMeshVertices
    {
      get { return _DisplayMeshVertices; }
      set
      {
        _DisplayMeshVertices = value;
        if (Gizmos != null)
        {
          Undo.RegisterCompleteObjectUndo(Gizmos, "Change Display Mesh Gizmos");
          int group = Undo.GetCurrentGroup();
          Gizmos.DisplayAllVertices = value;
          Gizmos.DisplayVertexPositions = value ? GetAllWorldMeshVertices() : new HashSet<Vector3>();
          Undo.CollapseUndoOperations(group);
        }
        if (Compute != null)
        {
          Undo.RegisterCompleteObjectUndo(Compute, "Change Display Mesh Compute");
          int group = Undo.GetCurrentGroup();
          Compute.DisplayAllVertices = value;
          Compute.SetDisplayAllBuffer(value ? GetAllWorldMeshVertices() : new HashSet<Vector3>());
          Undo.CollapseUndoOperations(group);
        }
      }
    }


    [SerializeField]
    private EasyColliderGizmos _Gizmos;
    /// <summary>
    /// Gizmos component for drawing vertices and selections.
    /// </summary>
    public EasyColliderGizmos Gizmos
    {
      get
      {
        if (_Gizmos == null && _SelectedGameObject != null)
        {
          _Gizmos = SelectedGameObject.GetComponent<EasyColliderGizmos>();
        }
        return _Gizmos;
      }
      set
      {
        _Gizmos = value;
        if (value != null && DisplayMeshVertices)
        {
          _Gizmos.DisplayVertexPositions = GetAllWorldMeshVertices();
          _Gizmos.DisplayAllVertices = true;
        }
      }
    }

    [SerializeField]
    private bool _IncludeChildMeshes;
    /// <summary>
    /// Are we including child meshes for vertex selection?
    /// </summary>
    public bool IncludeChildMeshes
    {
      get { return _IncludeChildMeshes; }
      set
      {
        _IncludeChildMeshes = value;
        SetupChildObjects(value);
        if (value == false)
        {
          CleanChildSelectedVertices();
        }
      }
    }

    [SerializeField]
    private bool _IsTrigger;
    /// <summary>
    /// Created colliders marked as trigger?
    /// </summary>
    /// <value></value>
    public bool IsTrigger { get { return _IsTrigger; } set { _IsTrigger = value; } }

    [SerializeField]
    private List<EasyColliderVertex> _LastSelectedVertices;
    /// <summary>
    /// List of the vertices that were selected last
    /// </summary>
    public List<EasyColliderVertex> LastSelectedVertices
    {
      get
      {
        if (_LastSelectedVertices == null)
        {
          _LastSelectedVertices = new List<EasyColliderVertex>();
        }
        return _LastSelectedVertices;
      }
      set { _LastSelectedVertices = value; }
    }

    [SerializeField]
    private List<MeshFilter> _MeshFilters;
    /// <summary>
    /// List of mesh filters on SelectedGameobject + children (if IncludeChildMeshes)
    /// </summary>
    public List<MeshFilter> MeshFilters
    {
      get
      {
        if (_MeshFilters == null)
        {
          _MeshFilters = new List<MeshFilter>();
        }
        return _MeshFilters;
      }
      set { _MeshFilters = value; }
    }

    [SerializeField]
    private List<Rigidbody> _NonKinematicRigidbodies;
    /// <summary>
    /// Rigidbodies already on the objects that were marked as kinematic during setup.
    /// </summary>
    public List<Rigidbody> NonKinematicRigidbodies
    {
      get
      {
        if (_NonKinematicRigidbodies == null)
        {
          _NonKinematicRigidbodies = new List<Rigidbody>();
        }
        return _NonKinematicRigidbodies;
      }
      set
      {
        _NonKinematicRigidbodies = value;
      }
    }

    [SerializeField]
    private PhysicMaterial _PhysicMaterial;
    /// <summary>
    /// Physic material to add to colliders upon creation.
    /// </summary>
    public PhysicMaterial PhysicMaterial { get { return _PhysicMaterial; } set { _PhysicMaterial = value; } }

    [SerializeField]
    private List<Collider> _PreDisabledColliders;
    /// <summary>
    /// Colliders that were already disabled before setup.
    /// </summary>
    public List<Collider> PreDisabledColliders
    {
      get
      {
        if (_PreDisabledColliders == null)
        {
          _PreDisabledColliders = new List<Collider>();
        }
        return _PreDisabledColliders;
      }
      set { _PreDisabledColliders = value; }
    }


    [SerializeField] private RENDER_POINT_TYPE _RenderPointType;
    /// <summary>
    /// Method we use to render points with. Either using a shader or gizmos.
    /// </summary>
    public RENDER_POINT_TYPE RenderPointType
    {
      get { return _RenderPointType; }
      set
      {
        // add or remove one if the value is changing and it's already added
        if (value != RENDER_POINT_TYPE.SHADER && Compute != null)
        {
          AddedComponents.Remove(Compute);
          Undo.DestroyObjectImmediate(Compute);
        }
        if (value != RENDER_POINT_TYPE.GIZMOS && Gizmos != null)
        {
          AddedComponents.Remove(Gizmos);
          Undo.DestroyObjectImmediate(Gizmos);
        }
        // add the new component.
        if (value == RENDER_POINT_TYPE.GIZMOS && Gizmos == null && SelectedGameObject != null)
        {
          Gizmos = Undo.AddComponent<EasyColliderGizmos>(SelectedGameObject);
          AddedComponents.Add(Gizmos);
        }
        if (value == RENDER_POINT_TYPE.SHADER && Compute == null && SelectedGameObject != null)
        {
          Compute = Undo.AddComponent<EasyColliderCompute>(SelectedGameObject);
          AddedComponents.Add(Compute);
        }
        _RenderPointType = value;
      }
    }

    [SerializeField]
    private int _RotatedColliderLayer;
    /// <summary>
    /// Layer to set on rotated collider's gameobject if not using selected gameobject's layer.
    /// </summary>
    public int RotatedColliderLayer { get { return _RotatedColliderLayer; } set { _RotatedColliderLayer = value; } }

    [SerializeField]
    private bool _RotatedOnSelectedLayer;
    /// <summary>
    /// Should rotated collider's gameobject be on same layer as the selected gameobject?
    /// </summary>
    /// <value></value>
    public bool RotatedOnSelectedLayer { get { return _RotatedOnSelectedLayer; } set { _RotatedOnSelectedLayer = value; } }

    [SerializeField]
    private Collider _SelectedCollider;
    /// <summary>
    /// The currently selected collider, checks if the value is a valid collider for the currently selected gameobject.
    /// </summary>
    /// <value></value>
    public Collider SelectedCollider
    {
      get
      {
        return _SelectedCollider;
      }
      set
      {
        if (isValidCollider(value))
        {
          _SelectedCollider = value;
        }
        if (value == null)
        {
          _SelectedCollider = null;
        }
      }
    }

    [SerializeField]
    private GameObject _SelectedGameObject;
    /// <summary>
    /// The currently selected gameobject. Changing this causes a cleanup of the previous selected object, and initialization of the object you are setting.
    /// </summary>
    public GameObject SelectedGameObject
    {
      get { return _SelectedGameObject; }
      set
      {
        // fix 2017 null ref on ispersistent.
        if (value == null)
        {
          CleanUpObject(_SelectedGameObject, false);
          _SelectedGameObject = value;
          SelectedCollider = null;
          AttachToObject = value;
        }
        else if (!EditorUtility.IsPersistent(value))
        {
          // new selected object.
          if (value != _SelectedGameObject)
          {
            // Had a selected object, clean it up.
            if (_SelectedGameObject != null)
            {
              CleanUpObject(_SelectedGameObject, false);
            }
            // Value is an actual object, so set up everything that's needed.
            if (value != null)
            {
              SelectObject(value);
            }
          }
          _SelectedGameObject = value;
          SelectedCollider = null;
          AttachToObject = value;
        }
        else
        {
          Debug.LogError("Easy Collider Editor's Selected GameObject must be located in the scene. Select a gameobject from the scene hierarchy. If you wish to use a prefab, enter prefab isolation mode then select the object. For more information of editing prefabs, see the included documentation pdf.");
        }
      }
    }

    [SerializeField]
    private List<EasyColliderVertex> _SelectedVertices;
    /// <summary>
    /// Selected Vertices list (Needs to be a list, as hashsets are unordered, and some of the collider methods require specific order selection (like rotated ones))
    /// </summary>
    public List<EasyColliderVertex> SelectedVertices
    {
      get
      {
        if (_SelectedVertices == null)
        {
          _SelectedVertices = new List<EasyColliderVertex>();
        }
        return _SelectedVertices;
      }
      private set { _SelectedVertices = value; }
    }

    [SerializeField]
    private HashSet<EasyColliderVertex> _SelectedVerticesSet;
    /// <summary>
    /// HashSet of SelectedVertices. Used to make things a little faster to search through.
    /// </summary>
    public HashSet<EasyColliderVertex> SelectedVerticesSet
    {
      get
      {
        if (_SelectedVerticesSet == null)
        {
          _SelectedVerticesSet = new HashSet<EasyColliderVertex>();
        }
        return _SelectedVerticesSet;
      }
      set { _SelectedVerticesSet = value; }
    }

    [SerializeField]
    /// <summary>
    /// Is vertex selection enabled?
    /// </summary>
    public bool VertexSelectEnabled;

    /// <summary>
    /// Sets values on this component from preferences.
    /// </summary>
    /// <param name="preferences"></param>
    public void SetValuesFromPreferences(EasyColliderPreferences preferences)
    {
      AutoIncludeChildSkinnedMeshes = preferences.AutoIncludeChildSkinnedMeshes;
      RotatedOnSelectedLayer = preferences.RotatedOnSelectedLayer;
      CreatedColliderDisabled = preferences.CreatedColliderDisabled;
      RenderPointType = preferences.RenderPointType;
    }

    /// <summary>
    /// Selects the gameobject. Sets up the require components based for the object.
    /// </summary>
    /// <param name="obj">GameObject to select</param>
    void SelectObject(GameObject obj)
    {
      // set up mesh filter list
      MeshFilters = GetMeshFilters(obj);
      // add / disable rigidbodies + colliders
      SetRequiredComponentsFrom(obj, MeshFilters);
      // add display vertices component.
      if (RenderPointType == RENDER_POINT_TYPE.GIZMOS)
      {
        Gizmos = Undo.AddComponent<EasyColliderGizmos>(obj);
        AddedComponents.Add(Gizmos);
      }
      else if (RenderPointType == RENDER_POINT_TYPE.SHADER)
      {
        Compute = Undo.AddComponent<EasyColliderCompute>(obj);
        AddedComponents.Add(Compute);
      }
    }

    /// <summary>
    /// Sets up the required components from the parent to the children (if children are enabled)
    /// This includes rigidbodies, colliders, and mesh colliders.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="meshFilters"></param>
    public void SetRequiredComponentsFrom(GameObject parent, List<MeshFilter> meshFilters)
    {
      if (parent == null) return;
      Rigidbody[] rigidbodies;
      Collider[] colliders;

      // get either parent + children or just parent rigidbodies & colliders.
      if (IncludeChildMeshes)
      {
        rigidbodies = parent.GetComponentsInChildren<Rigidbody>();
        colliders = parent.GetComponentsInChildren<Collider>();
      }
      else
      {
        rigidbodies = parent.GetComponents<Rigidbody>();
        colliders = parent.GetComponents<Collider>();
      }

      // make sure rigidbodies are set to kinematic for raycasting
      foreach (Rigidbody rb in rigidbodies)
      {
        if (!rb.isKinematic && !NonKinematicRigidbodies.Contains(rb))
        {
          Undo.RegisterCompleteObjectUndo(rb, "change isKinmatic");
          rb.isKinematic = true;
          NonKinematicRigidbodies.Add(rb);
        }
      }

      // Disable currently enabled colliders, leave current disabled colliders disabled & keep track of which is which.
      foreach (Collider col in colliders)
      {
        if (!DisabledColliders.Contains(col) && !PreDisabledColliders.Contains(col))
        {
          if (col.enabled)
          {
            if (!ColliderSelectEnabled)
            {
              Undo.RegisterCompleteObjectUndo(col, "Disable Collider");
              col.enabled = false;
            }
            DisabledColliders.Add(col);
          }
          else { PreDisabledColliders.Add(col); }
        }
      }

      // Add a mesh collider for every mesh filter.
      foreach (MeshFilter filter in meshFilters)
      {
        MeshCollider collider = Undo.AddComponent<MeshCollider>(filter.gameObject);
        AddedComponents.Add(collider);
      }
    }


    HashSet<Vector3> _WorldMeshVertices;
    HashSet<Vector3> WorldMeshVertices
    {
      get
      {
        if (_WorldMeshVertices == null)
        {
          _WorldMeshVertices = new HashSet<Vector3>();
        }
        return _WorldMeshVertices;
      }
    }

    HashSet<Vector3> _WorldMeshPositions;
    /// <summary>
    /// Set of mesh filters positions, for update world mesh vertices.
    /// </summary>
    /// <value></value>
    HashSet<Vector3> WorldMeshPositions
    {
      get
      {
        if (_WorldMeshPositions == null)
        {
          _WorldMeshPositions = new HashSet<Vector3>();
        }
        return _WorldMeshPositions;
      }
    }

    HashSet<Quaternion> _WorldMeshRotations;
    /// <summary>
    /// Set of world mesh rotations, for updating world mesh vertices.
    /// </summary>
    HashSet<Quaternion> WorldMeshRotations
    {
      get
      {
        if (_WorldMeshRotations == null)
        {
          _WorldMeshRotations = new HashSet<Quaternion>();
        }
        return _WorldMeshRotations;
      }
    }
    HashSet<Transform> _WorldMeshTransforms;
    /// <summary>
    /// Set of world mesh transforms, for updating world mesh vertices.
    /// </summary>
    HashSet<Transform> WorldMeshTransforms
    {
      get
      {
        if (_WorldMeshTransforms == null)
        {
          _WorldMeshTransforms = new HashSet<Transform>();
        }
        return _WorldMeshTransforms;
      }
    }
    /// <summary>
    /// Gets all the locations in world space of all MeshFilters vertices.
    /// </summary>
    /// <returns>World space locations of all mesh filters</returns>
    public HashSet<Vector3> GetAllWorldMeshVertices()
    {
      bool hasChanged = false;
      // if the pos, rot, or transform count is different than the mesh filter count we know we need to update.
      if (WorldMeshPositions.Count != MeshFilters.Count || WorldMeshRotations.Count != MeshFilters.Count || WorldMeshTransforms.Count != MeshFilters.Count)
      {
        hasChanged = true;
      }
      if (!hasChanged)
      {
        // we need to update if any of the mesh transforms have moved, or translated,
        // or the transform itself is different (ie. 2 different objects with same pos and rotation still means we need to update)
        foreach (MeshFilter filter in MeshFilters)
        {
          if (!WorldMeshPositions.Contains(filter.transform.position))
          {
            hasChanged = true;
            break;
          }
          if (!WorldMeshRotations.Contains(filter.transform.rotation))
          {
            hasChanged = true;
            break;
          }
          if (!WorldMeshTransforms.Contains(filter.transform))
          {
            hasChanged = true;
            break;
          }
        }
      }
      // need to recalculate all the world locations.
      if (hasChanged)
      {
        // Clear our lists to rebuild them.
        WorldMeshVertices.Clear();
        WorldMeshPositions.Clear();
        WorldMeshRotations.Clear();
        WorldMeshTransforms.Clear();
        foreach (MeshFilter filter in MeshFilters)
        {
          if (filter != null)
          {
            Transform t = filter.transform;
            WorldMeshPositions.Add(t.position);
            WorldMeshRotations.Add(t.rotation);
            WorldMeshTransforms.Add(t);
            Vector3[] vertices = filter.sharedMesh.vertices;
            foreach (Vector3 vert in vertices)
            {
              WorldMeshVertices.Add(t.TransformPoint(vert));
            }
          }
        }
      }
      // nothings changed? just return our hashset of world points.
      return WorldMeshVertices;
    }

    /// <summary>
    /// Creates a mesh filter and bakes a mesh for a skinned mesh renderer.
    /// </summary>
    /// <param name="smr">Skinned mesh renderer to create the mesh filter for.</param>
    /// <returns>The mesh filter that was created annd baked.</returns>
    MeshFilter SetupFilterForSkinnedMesh(SkinnedMeshRenderer smr)
    {
      // Add a mesh filter and collider to the skinned mesh renderer while we select vertices.
      MeshFilter filter = Undo.AddComponent<MeshFilter>(smr.gameObject);
      // MeshCollider collider = Undo.AddComponent<MeshCollider>(smr.gameObject);
      // Create a new mesh, so we prevent null refs by setting either the collider or filter's shared mesh.
      Mesh mesh = new Mesh();
      // Bake the skinned mesh to the mesh, otherwise you can have offset colliders/filters which aren't correctly located.
      smr.BakeMesh(mesh);
      // Set the shared mesh's to that mesh.
      filter.sharedMesh = mesh;
      AddedComponents.Add(filter);
      return filter;
    }

    /// <summary>
    /// Creates new lists for all the lists used.
    /// </summary>
    void ClearListsForNewObject()
    {
      MeshFilters = new List<MeshFilter>();
      SelectedVertices = new List<EasyColliderVertex>();
      AddedComponents = new List<Component>();
      NonKinematicRigidbodies = new List<Rigidbody>();
      DisabledColliders = new List<Collider>();
      PreDisabledColliders = new List<Collider>();
      CreatedColliders = new List<Collider>();
      SelectedVerticesSet = new HashSet<EasyColliderVertex>();
      LastSelectedVertices = new List<EasyColliderVertex>();
    }

    /// <summary>
    /// Gets all the mesh filters on the object. Gets the child meshes if include children is enabled, and creates mesh filters for any skinned mesh renderers if required.
    /// </summary>
    /// <param name="go">Parent object to get mesh filters from.</param>
    /// <returns>List of mesh filters for the object, children, and skinned mesh renderers.</returns>
    List<MeshFilter> GetMeshFilters(GameObject go)
    {
      if (go == null) return null;
      List<MeshFilter> meshFilters = new List<MeshFilter>();
      if (IncludeChildMeshes)
      {
        MeshFilter[] childMeshFilters = go.GetComponentsInChildren<MeshFilter>(false);
        foreach (MeshFilter childMeshFilter in childMeshFilters)
        {
          if (childMeshFilter != null)
          {
            meshFilters.Add(childMeshFilter);
          }
        }
        if (AutoIncludeChildSkinnedMeshes)
        {
          SkinnedMeshRenderer[] childMeshRenderers = go.GetComponentsInChildren<SkinnedMeshRenderer>(false);
          foreach (SkinnedMeshRenderer smr in childMeshRenderers)
          {
            meshFilters.Add(SetupFilterForSkinnedMesh(smr));
          }
        }
      }
      else
      {
        MeshFilter meshFilter = go.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
          meshFilters.Add(meshFilter);
        }
        else
        {
          SkinnedMeshRenderer smr = go.GetComponent<SkinnedMeshRenderer>();
          if (smr != null)
          {
            meshFilters.Add(SetupFilterForSkinnedMesh(smr));
          }
        }
      }
      if (meshFilters.Count <= 0 && MeshFilters.Count <= 0)
      {
        Debug.LogError("Easy Collider Editor: No mesh filters found on selected " + (IncludeChildMeshes ? "or child gameobjects." : "gameobject."));
      }
      return meshFilters;
    }

    /// <summary>
    /// Cleans up the object and children if required. Destroys components based on if going into play mode. Reenables/disables components to pre-selection values.
    /// </summary>
    /// <param name="go">Parent object to clean up</param>
    /// <param name="intoPlayMode">Is the editor going into play mode?</param>
    public void CleanUpObject(GameObject go, bool intoPlayMode)
    {
      foreach (Component component in AddedComponents)
      {
        if (component != null)
        {
          // Need to use normal destroy immediate when going into play mode.
          if (intoPlayMode)
          {
            DestroyImmediate(component);
          }
          else
          {
            // Use undo to destroy the added components if not going into play mode.
            Undo.DestroyObjectImmediate(component);
          }
        }
      }
      foreach (Rigidbody rb in NonKinematicRigidbodies)
      {
        if (rb != null)
        {
          rb.isKinematic = false;
        }
      }
      foreach (Collider col in DisabledColliders)
      {
        if (col != null)
        {
          col.enabled = true;
        }
      }
      foreach (Collider col in PreDisabledColliders)
      {
        if (col != null)
        {
          col.enabled = false;
        }
      }
      ClearListsForNewObject();
    }


    /// <summary>
    /// Adds / Removes / Enables / Disables required child components
    /// </summary>
    /// <param name="childrenEnabled">are children enabled?</param>
    void SetupChildObjects(bool childrenEnabled)
    {
      MeshFilters = GetMeshFilters(SelectedGameObject);
      if (childrenEnabled)
      {
        // Just essentially re-check all the components
        SetRequiredComponentsFrom(SelectedGameObject, MeshFilters);
      }
      if (!childrenEnabled)
      {
        // Renable child components that were changed.
        foreach (Component component in AddedComponents)
        {
          if (component != null && component.gameObject != SelectedGameObject)
          {
            Undo.DestroyObjectImmediate(component);
          }
        }
        foreach (Rigidbody rb in NonKinematicRigidbodies)
        {
          if (rb != null & rb.gameObject != SelectedGameObject)
          {
            // without these the undo is still recored with a registercompleteobjectundo.
            Undo.RecordObject(rb, "Set isKinematic");
            rb.isKinematic = false;
          }
        }
      }
    }

    /// <summary>
    /// Selects or deselects a vertex. Returns true if selected, false if deselected.
    /// </summary>
    /// <param name="ecv">Vertex to select</param>
    /// <returns>True if selected, false if deselected.</returns>
    public bool SelectVertex(EasyColliderVertex ecv)
    {

      if (SelectedVerticesSet.Remove(ecv))
      {
        SelectedVertices.Remove(ecv);
        return false;
      }
      else
      {
        LastSelectedVertices = new List<EasyColliderVertex>();
        SelectedVerticesSet.Add(ecv);
        SelectedVertices.Add(ecv);
        LastSelectedVertices.Add(ecv);
        return true;
      }
    }

    /// <summary>
    /// Selects a bunch of vertices at once.
    /// </summary>
    /// <param name="vertices">Set of vertices</param>
    public void SelectVertices(HashSet<EasyColliderVertex> vertices)
    {
      // removes selected vertices that are in the vertices hashset. (deselects already selected vertices)
      List<EasyColliderVertex> prevSelected = SelectedVertices.Where((value, index) => !vertices.Contains(value)).ToList();
      // adds vertices in the vertices set that aren't already selected (selects unselected vertices)
      List<EasyColliderVertex> newSelected = vertices.Where((value) => !SelectedVerticesSet.Contains(value)).ToList();
      // last selected are the newly selected vertices.
      LastSelectedVertices.Clear();
      LastSelectedVertices = newSelected;
      // Combine the lists for the all currently selected vertices.
      prevSelected.AddRange(newSelected);
      SelectedVertices = prevSelected;
      // clear the selected vertices set
      SelectedVerticesSet.Clear();
      // add all the currently selected vertices to it with a union.
      SelectedVerticesSet.UnionWith(SelectedVertices);
    }

    /// <summary>
    /// Gets a list of the selected vertices in world space positions.
    /// </summary>
    /// <returns>List of world space positions.</returns>
    public List<Vector3> GetWorldVertices()
    {
      if (SelectedGameObject == null) { }
      // since order can sometimes matter, we're using lists.
      // create list with enough space for all the vertices.
      List<Vector3> worldVertices = new List<Vector3>(SelectedVertices.Count);
      foreach (EasyColliderVertex ecv in SelectedVertices)
      {
        if (ecv.T == null) continue;
        worldVertices.Add(ecv.T.TransformPoint(ecv.LocalPosition));
      }
      return worldVertices;
    }

    /// <summary>
    /// Creates a box colider with a given orientation
    /// </summary>
    /// <param name="orientation">Orientation of box collider</param>
    public void CreateBoxCollider(COLLIDER_ORIENTATION orientation = COLLIDER_ORIENTATION.NORMAL)
    {
      EasyColliderCreator ecc = new EasyColliderCreator();
      Collider createdCollider = ecc.CreateBoxCollider(GetWorldVertices(), GetProperties(orientation));
      if (createdCollider != null)
      {
        DisableCreatedCollider(createdCollider);
      }
      SelectedVertices = new List<EasyColliderVertex>();
      SelectedVerticesSet = new HashSet<EasyColliderVertex>();
    }

    /// <summary>
    /// Creates an EasyColliderProperties based on the ECEditors values.
    /// </summary>
    /// <param name="orientation">Orientation property</param>
    /// <returns>EasyColliderProperties to pass to collider creation methods</returns>
    public EasyColliderProperties GetProperties(COLLIDER_ORIENTATION orientation = COLLIDER_ORIENTATION.NORMAL)
    {
      EasyColliderProperties ecp = new EasyColliderProperties();
      ecp.IsTrigger = IsTrigger;
      ecp.PhysicMaterial = PhysicMaterial;
      ecp.Layer = RotatedOnSelectedLayer ? SelectedGameObject.layer : RotatedColliderLayer;
      ecp.AttachTo = AttachToObject;
      ecp.Orientation = orientation;
      return ecp;
    }

    /// <summary>
    /// Creates a capsule collider using the method and orientation provided.
    /// </summary>
    /// <param name="method">Capsule collider algoirthm to use</param>
    /// <param name="orientation">Orientation to use</param>
    public void CreateCapsuleCollider(CAPSULE_COLLIDER_METHOD method, COLLIDER_ORIENTATION orientation = COLLIDER_ORIENTATION.NORMAL)
    {
      EasyColliderCreator ecc = new EasyColliderCreator();
      Collider createdCollider = null;
      switch (method)
      {
        // use the same method for all min max' but pass the method in.
        case CAPSULE_COLLIDER_METHOD.MinMax:
        case CAPSULE_COLLIDER_METHOD.MinMaxPlusDiameter:
        case CAPSULE_COLLIDER_METHOD.MinMaxPlusRadius:
          createdCollider = ecc.CreateCapsuleCollider_MinMax(GetWorldVertices(), GetProperties(orientation), method);
          break;
        case CAPSULE_COLLIDER_METHOD.BestFit:
          createdCollider = ecc.CreateCapsuleCollider_BestFit(GetWorldVertices(), GetProperties(orientation));
          break;
      }
      if (createdCollider != null)
      {
        DisableCreatedCollider(createdCollider);
      }
      SelectedVertices = new List<EasyColliderVertex>();
      SelectedVerticesSet = new HashSet<EasyColliderVertex>();
    }

    /// <summary>
    /// Creates a sphere collider
    /// </summary>
    /// <param name="method">Algorith to use to create the sphere collider.</param>
    public void CreateSphereCollider(SPHERE_COLLIDER_METHOD method)
    {
      EasyColliderCreator ecc = new EasyColliderCreator();
      Collider createdCollider = null;
      switch (method)
      {
        case SPHERE_COLLIDER_METHOD.BestFit:
          createdCollider = ecc.CreateSphereCollider_BestFit(GetWorldVertices(), GetProperties());
          break;
        case SPHERE_COLLIDER_METHOD.Distance:
          createdCollider = ecc.CreateSphereCollider_Distance(GetWorldVertices(), GetProperties());
          break;
        case SPHERE_COLLIDER_METHOD.MinMax:
          createdCollider = ecc.CreateSphereCollider_MinMax(GetWorldVertices(), GetProperties());
          break;
      }
      if (createdCollider != null)
      {
        DisableCreatedCollider(createdCollider);
      }
      SelectedVertices = new List<EasyColliderVertex>();
      SelectedVerticesSet = new HashSet<EasyColliderVertex>();
    }

    /// <summary>
    /// Disables a created collider based on preferences
    /// </summary>
    /// <param name="col">Collider to disable</param>
    void DisableCreatedCollider(Collider col)
    {
      CreatedColliders.Add(col);
      if (CreatedColliderDisabled)
      {
        col.enabled = false;
        DisabledColliders.Add(col);
      }
    }

    /// <summary>
    /// Deselects all currently selected vertices.
    /// </summary>
    public void ClearSelectedVertices()
    {
      this.SelectedVertices = new List<EasyColliderVertex>();
      this.SelectedVerticesSet = new HashSet<EasyColliderVertex>();
    }

    /// <summary>
    /// Removes all colliders on the currently selected gameobject + children if include child meshes is true.
    /// </summary>
    public void RemoveAllColliders()
    {
      // Get colliders from either selected or selected + children.
      Collider[] colliders = GetAllColliders();
      foreach (Collider col in colliders)
      {
        // Only remove if it's not what we've added.
        if (!AddedComponents.Contains(col))
        {
          Undo.RecordObject(col.gameObject, "Remove Collider");
          // Remove it from the components we've disabled if we've disabled it.
          DisabledColliders.Remove(col);
          PreDisabledColliders.Remove(col);
          Undo.DestroyObjectImmediate(col);
        }
      }
    }

    /// <summary>
    /// Gets the collider if it's in any of the lists. Otherwise it tries to compare the collider to all colliders in the list by properties.null
    /// Required for when editing a prefab, as the colliders hit by raycast are NOT the colliders in the list.
    /// </summary>
    /// <param name="collider">Collider to match with</param>
    /// <returns>The collider itself it it exists in one of the lists, otherwise tries to match by properties. Null if none match.</returns>
    private Collider GetMatchedCollider(Collider collider)
    {
      if (DisabledColliders.Contains(collider) || PreDisabledColliders.Contains(collider) || CreatedColliders.Contains(collider))
      {
        return collider;
      }
      else
      {
        Collider matchedCollider = null;
        matchedCollider = tryGetMatchColliderInList(collider, DisabledColliders);
        matchedCollider = matchedCollider == null ? tryGetMatchColliderInList(collider, PreDisabledColliders) : matchedCollider;
        matchedCollider = matchedCollider == null ? tryGetMatchColliderInList(collider, CreatedColliders) : matchedCollider;
        return matchedCollider;
      }
    }

    /// <summary>
    /// Attempts to match the given collider to any collider in the given list.
    /// </summary>
    /// <param name="collider">Collider to match</param>
    /// <param name="colliderList">Collider list to find a match in.</param>
    /// <returns>The matched collider, or null if none found.</returns>
    private Collider tryGetMatchColliderInList(Collider collider, List<Collider> colliderList)
    {
      // dont match by checking physic materials, that causes issues.
      foreach (Collider col in colliderList)
      {
        if (col.gameObject.name != collider.gameObject.name)
        {
          continue;
        }
        if (col is BoxCollider && collider is BoxCollider)
        {
          BoxCollider c1 = col as BoxCollider;
          BoxCollider c2 = collider as BoxCollider;
          if (c1.center == c2.center && c1.size == c2.size && c1.isTrigger == c2.isTrigger && c1.sharedMaterial == c2.sharedMaterial)
          {
            return col;
          }
        }
        else if (col is CapsuleCollider && collider is CapsuleCollider)
        {
          CapsuleCollider c1 = col as CapsuleCollider;
          CapsuleCollider c2 = collider as CapsuleCollider;
          if (c1.center == c2.center && c1.height == c2.height && c1.direction == c2.direction && c1.height == c2.height && c1.isTrigger == c2.isTrigger && c1.sharedMaterial == c2.sharedMaterial)
          {
            return col;
          }
        }
        else if (col is SphereCollider && collider is SphereCollider)
        {
          SphereCollider c1 = col as SphereCollider;
          SphereCollider c2 = collider as SphereCollider;
          if (c1.center == c2.center && c1.radius == c2.radius && c1.isTrigger == c2.isTrigger && c1.sharedMaterial == c2.sharedMaterial)
          {
            return col;
          }
        }
      }
      return null;
    }

    /// <summary>
    /// Determines if the collider is valid. A collider is valid if it exists in the collider lists, OR it matches one in the list based on parameters.
    /// </summary>
    /// <param name="collider">Collider to match</param>
    /// <returns>True if in one of the lists, or matches one of the items in the lists.</returns>
    private bool isValidCollider(Collider collider)
    {
      if (collider == null) return false;
      Collider matched = GetMatchedCollider(collider);
      if (matched != null)
      {
        return true;
      }
      return false;
    }

    /// <summary>
    /// Removes the currently selected collider.
    /// </summary>
    public void RemoveSelectedCollider()
    {
      Collider collider = GetMatchedCollider(SelectedCollider);
      DisabledColliders.Remove(collider);
      CreatedColliders.Remove(collider);
      Undo.RecordObject(collider, "Remove collider");
      Undo.DestroyObjectImmediate(collider);
      SelectedCollider = null;
    }

    /// <summary>
    /// Enabled and disables all colliders based on if collider selection is enabled.
    /// </summary>
    /// <param name="colliderSelectionEnabled">is collider selection enabled?</param>
    private void ToggleCollidersEnabled(bool colliderSelectionEnabled)
    {
      // Get colliders from either selected or selected + children.
      Collider[] colliders = GetAllColliders();
      foreach (Collider col in colliders)
      {
        // Without this undo, the enable/disable is not recorded.
        Undo.RecordObject(col, "Toggle Collider Enabled");
        // we add the mesh collider, so we want to disable it during collider selection.
        if (AddedComponents.Contains(col))
        {
          col.enabled = !colliderSelectionEnabled;
        }
        else
        {
          col.enabled = colliderSelectionEnabled;
        }
      }
    }

    /// <summary>
    /// Gets all colliders on parent + children if including children.
    /// </summary>
    /// <returns>Array of all colliders</returns>
    private Collider[] GetAllColliders()
    {
      if (SelectedGameObject != null)
      {
        return IncludeChildMeshes ? SelectedGameObject.GetComponentsInChildren<Collider>() : SelectedGameObject.GetComponents<Collider>();
      }
      return new Collider[0];
    }

    /// <summary>
    /// Removes all vertices that have index's greater than MeshFilter's count.
    /// </summary>
    private void CleanChildSelectedVertices()
    {
      // SelectedVertices.RemoveAll(vert => vert.MeshFilterIndex >= MeshFilters.Count);
      SelectedVertices.RemoveAll(vert => vert.T != SelectedGameObject.transform);
    }

    /// <summary>
    /// Inverts the currently selected vertices
    /// </summary>
    public void InvertSelectedVertices()
    {
      // list of new vertices (this doesn't have to be a list, since when inverting it'll mess up the order anyway)
      // but we can just use a list, and then union with the selected set at the end, since we're not doing any checking on it.
      List<EasyColliderVertex> inverted = new List<EasyColliderVertex>();
      // world positions for selected & invert, as we don't want to duplicate vertices due to triangles sharing vertices.
      HashSet<Vector3> selectedWorldPositions = new HashSet<Vector3>();
      HashSet<Vector3> invertedWorldPositions = new HashSet<Vector3>();
      // get selected vertices in worldspace
      foreach (EasyColliderVertex vert in SelectedVertices)
      {
        selectedWorldPositions.Add(vert.T.TransformPoint(vert.LocalPosition));
      }
      // Variables to hold values
      Vector3[] vertices;
      Transform transform;
      Vector3 transformedPosition;
      for (int i = 0; i < MeshFilters.Count; i++)
      {
        if (MeshFilters[i] != null)
        {
          // we only assign vertices array once per filter.
          transform = MeshFilters[i].transform;
          vertices = MeshFilters[i].sharedMesh.vertices;
          for (int j = 0; j < vertices.Length; j++)
          {
            transformedPosition = transform.TransformPoint(vertices[j]);
            // if it's currently not selected, and isn't already in the inverted positions
            // (multiple vertices share same world space because of triangles)
            if (!selectedWorldPositions.Contains(transformedPosition) && !invertedWorldPositions.Contains(transformedPosition))
            {
              invertedWorldPositions.Add(transformedPosition);
              inverted.Add(new EasyColliderVertex(transform, vertices[j]));
            }
          }
        }
      }
      SelectedVertices = inverted;
      // clear and union the selected set with the new inverted list.
      SelectedVerticesSet.Clear();
      SelectedVerticesSet.UnionWith(inverted);
    }

    /// <summary>
    /// Grows the list of vertices by shared triangles
    /// </summary>
    /// <param name="verticesToGrow">The list of vertices to expand out from</param>
    public void GrowVertices(HashSet<EasyColliderVertex> verticesToGrow)
    {
      HashSet<EasyColliderVertex> newSelectedVertices = new HashSet<EasyColliderVertex>();
      Transform t;
      Vector3[] vertices;
      int[] triangles;
      // Go through every filter & triangle, seems the fastest way to do it without storing the vertices & triangles of every mesh.
      foreach (MeshFilter filter in MeshFilters)
      {
        triangles = filter.sharedMesh.triangles;
        vertices = filter.sharedMesh.vertices;
        t = filter.transform;
        for (int i = 0; i < triangles.Length; i += 3)
        {
          EasyColliderVertex ecv1 = new EasyColliderVertex(t, vertices[triangles[i]]);
          EasyColliderVertex ecv2 = new EasyColliderVertex(t, vertices[triangles[i + 1]]);
          EasyColliderVertex ecv3 = new EasyColliderVertex(t, vertices[triangles[i + 2]]);
          if (verticesToGrow.Contains(ecv1) || verticesToGrow.Contains(ecv2) || verticesToGrow.Contains(ecv3))
          {
            newSelectedVertices.Add(ecv1);
            newSelectedVertices.Add(ecv2);
            newSelectedVertices.Add(ecv3);
          }
        }
      }
      // newly selected vertices are the ones where they are in the new set, but aren't currently in the selected set.
      List<EasyColliderVertex> newSelected = newSelectedVertices.Where(value => !SelectedVerticesSet.Contains(value)).ToList();
      // Add the new ones to the list.
      SelectedVertices.AddRange(newSelected);
      // clear the set, then union with the select vertices.
      SelectedVerticesSet.Clear();
      SelectedVerticesSet.UnionWith(SelectedVertices);
      // these aren't really the "last selected" as it contains the previous grown set as well, but its close enough that it doesn't really matter much.
      LastSelectedVertices = newSelected;
    }

    /// <summary>
    /// Grows selected vertices out from all selected vertices
    /// </summary>
    public void GrowAllSelectedVertices()
    {
      GrowVertices(SelectedVerticesSet);
    }

    /// <summary>
    /// Grows selected vertices out from the last selected vertex(s)
    /// </summary>
    public void GrowLastSelectedVertices()
    {
      HashSet<EasyColliderVertex> set = new HashSet<EasyColliderVertex>();
      set.UnionWith(LastSelectedVertices);
      GrowVertices(set);
    }

    //Serializing our hashsets.
    [SerializeField]
    private List<EasyColliderVertex> serializedSelectedVertexSet;
    public void OnBeforeSerialize()
    {
      // Serialize ours hashsets.
      if (serializedSelectedVertexSet == null)
      {
        serializedSelectedVertexSet = new List<EasyColliderVertex>();
      }
      serializedSelectedVertexSet = SelectedVerticesSet.ToList();
    }

    public void OnAfterDeserialize()
    {
      // Deserialize our hashsets.
      if (serializedSelectedVertexSet.Count > 0)
      {
        SelectedVerticesSet = new HashSet<EasyColliderVertex>(serializedSelectedVertexSet);
      }
      else
      {
        SelectedVerticesSet = new HashSet<EasyColliderVertex>();
      }
    }

    /// <summary>
    /// Rings around the last 2 selected vertices, selecting all the vertices in the ring.
    /// </summary>
    public void RingSelectVertices()
    {
      if (SelectedVertices.Count < 2)
      {
        Debug.LogWarning("Easy Collider Editor: Ring select requires 2 selected vertices.");
        return;
      }
      // last 2 selected vertices must come from the same transform, otherwise you can't really ring around a mesh..
      if (SelectedVertices[SelectedVertices.Count - 1].T != SelectedVertices[SelectedVertices.Count - 2].T)
      {
        Debug.LogWarning("Easy Collider Editor: Ring select from different transforms not allowed.");
        return;
      }
      // list of all the vertice's were going to add at the end
      List<EasyColliderVertex> newVerticesToAdd = new List<EasyColliderVertex>();
      // add the last 2 vertices initially so we know where to end.
      newVerticesToAdd.Add(SelectedVertices[SelectedVertices.Count - 1]);
      newVerticesToAdd.Add(SelectedVertices[SelectedVertices.Count - 2]);
      // get mesh's vertices & triangles.
      Vector3[] vertices = new Vector3[0];
      int[] triangles = new int[3];
      Transform t = SelectedVertices[SelectedVertices.Count - 1].T;
      foreach (MeshFilter filter in MeshFilters)
      {
        if (filter.transform == t)
        {
          vertices = filter.sharedMesh.vertices;
          triangles = filter.sharedMesh.triangles;
        }
      }
      // start vertex
      Vector3 currentVertex = SelectedVertices[SelectedVertices.Count - 1].LocalPosition;
      // previous vertex.
      Vector3 prevVertex = SelectedVertices[SelectedVertices.Count - 2].LocalPosition;
      // directon vector for first 2 points.
      Vector3 currentDirection = (currentVertex - prevVertex).normalized;
      // Directions for calculations
      Vector3 directionA, directionB = directionA = Vector3.zero;
      // points for calculations
      Vector3 pointA, pointB = pointA = Vector3.zero;
      // angle from calculations
      float angleA, angleB = angleA = 0.0f;

      // best point found in each iteration
      Vector3 bestPoint = Vector3.zero;
      // direction fot he best point (from current point)
      Vector3 bestDirection = Vector3.zero;
      // best angle from the best point (angle between current direction and best points direction from current point)
      float bestAngle = Mathf.Infinity;
      while (true)
      {
        // reset best angle for each iteration.
        bestAngle = Mathf.Infinity;
        // go through all the triangles.
        for (int i = 0; i < triangles.Length; i += 3)
        {
          // if the triangle doesn't contain both the current and previous vertex.
          // (as it's by the position, it allows cross edges that match position but not actual vertices' index)
          if ((vertices[triangles[i]] == currentVertex || vertices[triangles[i + 1]] == currentVertex || vertices[triangles[i + 2]] == currentVertex)
          && (vertices[triangles[i]] != prevVertex && vertices[triangles[i + 1]] != prevVertex && vertices[triangles[i + 2]] != prevVertex))
          {
            // if it's the first vertex.
            if (vertices[triangles[i]] == currentVertex)
            {
              // set the values for the pointA, pointB, directionA, and directionB to calculate.
              pointA = vertices[triangles[i + 1]];
              pointB = vertices[triangles[i + 2]];
              directionA = pointA - currentVertex;
              directionB = pointB - currentVertex;
            }
            else if (vertices[triangles[i + 1]] == currentVertex)
            {
              pointA = vertices[triangles[i]];
              pointB = vertices[triangles[i + 2]];
              directionA = pointA - currentVertex;
              directionB = pointB - currentVertex;
            }
            else if (vertices[triangles[i + 2]] == currentVertex)
            {
              pointA = vertices[triangles[i]];
              pointB = vertices[triangles[i + 1]];
              directionA = pointA - currentVertex;
              directionB = pointB - currentVertex;
            }
            // calculate angles between current direction and the direction to point A and point B.
            angleA = Vector3.Angle(currentDirection, directionA);
            angleB = Vector3.Angle(currentDirection, directionB);
            // if the angle is less than our current best angle, and less than the other triangles angle
            if (angleA < bestAngle && angleA < angleB)
            {
              // set our new best angle, best point, and best direction.
              bestAngle = angleA;
              bestPoint = pointA;
              bestDirection = directionA;
            }
            else if (angleB < bestAngle && angleB < angleA)
            {
              bestAngle = angleB;
              bestPoint = pointB;
              bestDirection = directionB;
            }
          }
        }
        currentDirection = bestDirection;
        prevVertex = currentVertex;
        currentVertex = bestPoint;
        EasyColliderVertex ecv = new EasyColliderVertex(t, bestPoint);
        if (newVerticesToAdd.Contains(ecv))
        {
          // reach some kind of end (newest point is already to be added.)
          break;
        }
        else
        {
          newVerticesToAdd.Add(ecv);
        }
      }
      SelectedVertices.AddRange(newVerticesToAdd);
      SelectedVerticesSet.UnionWith(newVerticesToAdd);
      LastSelectedVertices.Clear();
      LastSelectedVertices = newVerticesToAdd;
    }
  }
}
#endif