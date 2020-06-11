#if (UNITY_EDITOR)
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
namespace ECE
{
  /// <summary>
  /// Not actually a compute shader. Just uses a regular shader with a structured buffer.
  /// "In regular graphics shaders the compute buffer support requires minimum shader model 4.5."
  /// </summary>
  [ExecuteInEditMode, System.Serializable]
  public class EasyColliderCompute : MonoBehaviour
  {
    // shader to create materials from
    public Shader GeometryShader;

    // hovered material for use with hovered buffer, color, and size.
    [SerializeField]
    Material _HoveredMaterial;
    [SerializeField]
    Material _SelectedMaterial;
    [SerializeField]
    Material _OverlapMaterial;
    [SerializeField]
    Material _DisplayAllMaterial;

    // bools to use to check if the buffer is valid (not empty), and if they are valid, to render the vertices/geometry
    [SerializeField]
    private bool _ValidOverlapBuffer = true;
    [SerializeField]
    private bool _ValidSelectedBuffer = true;
    [SerializeField]
    private bool _ValidHoverBuffer = true;
    [SerializeField]
    private bool _ValidDisplayAllBuffer = true;

    // Colors for selected, hovered, overlap materials.
    public Color SelectedColor = Color.red;
    public Color HoveredColor = Color.cyan;
    public Color OverlapColor = Color.red;
    public Color DisplayAllColor = Color.blue;

    // Size for selected hovered, overlap materials.
    public float SelectedSize = 0.01f;
    public float HoveredSize = 0.01f;
    public float OverlapSize = 0.015f;
    public float DisplayAllSize = 0.0075f;

    // Lists of points to be used in buffers.
    [SerializeField]
    private List<Vector3> _SelectedWorldPoints = new List<Vector3>();
    [SerializeField]
    private HashSet<Vector3> _SelectedWorldPointsSet = new HashSet<Vector3>();
    [SerializeField]
    private List<Vector3> _OverlappedPoints = new List<Vector3>();
    [SerializeField]
    private List<Vector3> _HoveredPoints = new List<Vector3>();
    [SerializeField]
    private List<Vector3> _DisplayAllPoints = new List<Vector3>();
    public int SelectedPointCount { get { return _SelectedWorldPoints.Count; } }
    public int HoveredPointCount { get { return _HoveredPoints.Count + _OverlappedPoints.Count; } }
    public int DisplayPointCount { get { return _DisplayAllPoints.Count; } }
    // Compute buffers
    [SerializeField]
    ComputeBuffer _SelectedBuffer;
    [SerializeField]
    ComputeBuffer _HoveredBuffer;
    [SerializeField]
    ComputeBuffer _OverlapBuffer;
    [SerializeField]
    ComputeBuffer _DisplayAllBuffer;

    [SerializeField]
    public bool DisplayAllVertices;

    void OnEnable()
    {
      // find the geometry shader
      if (GeometryShader == null)
      {
        string[] ecp = AssetDatabase.FindAssets("EasyColliderShader t:Shader");
        if (ecp.Length > 0)
        {
          string assetPath = AssetDatabase.GUIDToAssetPath(ecp[0]);
          GeometryShader = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Shader)) as Shader;
        }
        if (GeometryShader == null)
        {
          Debug.LogError("EasyColliderEditor was unable to find the shader needed for displaying vertices. If you deleted it because your system does not support it, be sure to set Render Vertex Method to Gizmos in preferences.");
          DestroyImmediate(this);
        }
      }

      // create materials
      _OverlapMaterial = new Material(GeometryShader);
      _HoveredMaterial = new Material(GeometryShader);
      _SelectedMaterial = new Material(GeometryShader);
      _DisplayAllMaterial = new Material(GeometryShader);
      // create a buffer
      _ValidOverlapBuffer = false;
      _ValidHoverBuffer = false;
      _ValidSelectedBuffer = false;
      _ValidDisplayAllBuffer = false;

      _DisplayAllBuffer = new ComputeBuffer(1, 12);
      _HoveredBuffer = new ComputeBuffer(1, 12);
      _SelectedBuffer = new ComputeBuffer(1, 12);
      _OverlapBuffer = new ComputeBuffer(1, 12);
    }

    void OnRenderObject()
    {
      // only need to re-update the selected buffer due to undo/redo operations.
      if (!_ValidSelectedBuffer && _SelectedWorldPoints.Count > 0)
      {
        UpdateSelectedBuffer(_SelectedWorldPoints);
      }

      if (_OverlapBuffer != null && _ValidOverlapBuffer)
      {
        _OverlapMaterial.SetColor("_Color", OverlapColor);
        _OverlapMaterial.SetFloat("_Size", OverlapSize > SelectedSize ? OverlapSize : SelectedSize + SelectedSize * 0.1f);
        _OverlapMaterial.SetPass(0);
        // draw the topology as points. the squares are drawn in the shader by triangles from the points passed in through the overlap buffer when it is updated.
        // Graphics.DrawProceduralNow(MeshTopology.Points, _OverlapBuffer.count);
        Graphics.DrawProceduralNow(MeshTopology.Points, _OverlapBuffer.count);
      }
      if (_HoveredBuffer != null && _ValidHoverBuffer)
      {
        _HoveredMaterial.SetColor("_Color", HoveredColor);
        _HoveredMaterial.SetFloat("_Size", HoveredSize);
        _HoveredMaterial.SetPass(0);
        // Graphics.DrawProceduralNow(MeshTopology.Points, _HoveredBuffer.count);
        Graphics.DrawProceduralNow(MeshTopology.Points, _HoveredBuffer.count);
      }
      if (_SelectedBuffer != null && _ValidSelectedBuffer)
      {
        _SelectedMaterial.SetColor("_Color", SelectedColor);
        _SelectedMaterial.SetFloat("_Size", SelectedSize);
        _SelectedMaterial.SetPass(0);
        // Graphics.DrawProceduralNow(MeshTopology.Points, _SelectedBuffer.count);
        Graphics.DrawProceduralNow(MeshTopology.Points, _SelectedBuffer.count);
      }
      if (_DisplayAllBuffer != null && _ValidDisplayAllBuffer)
      {
        _DisplayAllMaterial.SetColor("_Color", DisplayAllColor);
        _DisplayAllMaterial.SetFloat("_Size", DisplayAllSize);
        _DisplayAllMaterial.SetPass(0);
        // Graphics.DrawProceduralNow(MeshTopology.Points, _DisplayAllBuffer.count);
        Graphics.DrawProceduralNow(MeshTopology.Points, _DisplayAllBuffer.count);
      }
    }


    /// <summary>
    /// Updates the selected world points buffer.
    /// </summary>
    /// <param name="worldPoints">New list of world points</param>
    public void UpdateSelectedBuffer(List<Vector3> worldPoints)
    {
      _SelectedWorldPointsSet.Clear();
      _SelectedWorldPoints = worldPoints;
      _SelectedWorldPointsSet.UnionWith(worldPoints);
      worldPoints.ForEach(point => _SelectedWorldPointsSet.Add(point));
      if (_SelectedBuffer != null)
      {
        _SelectedBuffer.Release();
      }
      if (_SelectedWorldPoints.Count > 0)
      {
        _SelectedBuffer = new ComputeBuffer(_SelectedWorldPoints.Count, 12);
        // _SelectedBuffer.SetData(_SelectedWorldPoints);
        _SelectedBuffer.SetData(_SelectedWorldPoints.ToArray());
        _ValidSelectedBuffer = true;
      }
      else
      {
        _SelectedBuffer = new ComputeBuffer(1, 12);
        _SelectedBuffer.SetCounterValue(0);
        _ValidSelectedBuffer = false;
      }
      _SelectedMaterial.SetBuffer("worldPositions", _SelectedBuffer);
    }

    /// <summary>
    /// Updates both the overlap and hovered buffer
    /// </summary>
    /// <param name="worldPoints">All vertices highlighted for possible selection</param>
    public void UpdateOverlapHoveredBuffer(HashSet<Vector3> worldPoints)
    {
      _OverlappedPoints.Clear();
      _HoveredPoints.Clear();
      foreach (Vector3 p in _SelectedWorldPoints)
      {
        if (worldPoints.Contains(p))
        {
          _OverlappedPoints.Add(p);
        }
      }
      foreach (Vector3 p in worldPoints)
      {
        if (!_SelectedWorldPointsSet.Contains(p))
        {
          _HoveredPoints.Add(p);
        }
      }

      if (_OverlapBuffer != null)
      {
        _OverlapBuffer.Release();
      }
      if (_OverlappedPoints.Count > 0)
      {
        _OverlapBuffer = new ComputeBuffer(_OverlappedPoints.Count, 12);
        // _OverlapBuffer.SetData(_OverlappedPoints);
        _OverlapBuffer.SetData(_OverlappedPoints.ToArray());
        _ValidOverlapBuffer = true;
      }
      else
      {
        _OverlapBuffer = new ComputeBuffer(1, 12);
        _OverlapBuffer.SetCounterValue(0);
        _ValidOverlapBuffer = false;
      }
      _OverlapMaterial.SetBuffer("worldPositions", _OverlapBuffer);

      if (_HoveredBuffer != null)
      {
        _HoveredBuffer.Release();
      }
      if (_HoveredPoints.Count > 0)
      {
        _HoveredBuffer = new ComputeBuffer(_HoveredPoints.Count, 12);
        // _HoveredBuffer.SetData(_HoveredPoints);
        _HoveredBuffer.SetData(_HoveredPoints.ToArray());
        _ValidHoverBuffer = true;
      }
      else
      {
        _HoveredBuffer = new ComputeBuffer(1, 12);
        _HoveredBuffer.SetCounterValue(0);
        _ValidHoverBuffer = false;
      }
      _HoveredMaterial.SetBuffer("worldPositions", _HoveredBuffer);
    }

    public void SetDisplayAllBuffer(HashSet<Vector3> worldPoints)
    {
      _DisplayAllPoints.Clear();
      if (_DisplayAllBuffer != null)
      {
        _DisplayAllBuffer.Release();
      }
      _DisplayAllPoints = worldPoints.ToList();
      if (_DisplayAllPoints.Count > 0)
      {
        _DisplayAllBuffer = new ComputeBuffer(_DisplayAllPoints.Count, 12);
        // _DisplayAllBuffer.SetData(_DisplayAllPoints);
        _DisplayAllBuffer.SetData(_DisplayAllPoints.ToArray());
        _ValidDisplayAllBuffer = true;
      }
      else
      {
        _DisplayAllBuffer = new ComputeBuffer(1, 12);
        _DisplayAllBuffer.SetCounterValue(0);
        _ValidDisplayAllBuffer = false;
      }
      _DisplayAllMaterial.SetBuffer("worldPositions", _DisplayAllBuffer);
    }

    void OnDestroy()
    {
      if (_HoveredBuffer != null)
      {
        _HoveredBuffer.Release();
      }
      if (_SelectedBuffer != null)
      {
        _SelectedBuffer.Release();
      }
      if (_OverlapBuffer != null)
      {
        _OverlapBuffer.Release();
      }
      if (_DisplayAllBuffer != null)
      {
        _DisplayAllBuffer.Release();
      }
    }

    void OnDisable()
    {
      if (_HoveredBuffer != null)
      {
        _HoveredBuffer.Release();
      }
      if (_SelectedBuffer != null)
      {
        _SelectedBuffer.Release();
      }
      if (_OverlapBuffer != null)
      {
        _OverlapBuffer.Release();
      }
      if (_DisplayAllBuffer != null)
      {
        _DisplayAllBuffer.Release();
      }
    }
  }
}
#endif