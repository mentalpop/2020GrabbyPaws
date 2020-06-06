#if (UNITY_EDITOR)
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// If you have purchased this asset and have any other ideas for features, please contact me at pmurph.software@gmail.com
// I would love to hear what users of this asset would like added.

// Future Feature ideas:
// Better functionality & cleanup from prefab isolation mode once PrefabStage and PrefabStageUtility is out of Experimental in LTS.
// auto box / capsule mesh generation along bone chain using skinned mesh renderer & bone weights.
// capsule method of swept sphere of points + axis of points.
// vertex selection -> convex hull generation / mesh colliders

namespace ECE
{
  [System.Serializable]
  public class EasyColliderWindow : EditorWindow
  {
    /// <summary>
    /// Is the key to only Remove verts when dragging held down?
    /// </summary>
    private bool _BoxMinusHeld = false;

    /// <summary>
    /// Is the key to only select verts when dragging held down?
    /// </summary>
    private bool _BoxPlusHeld = false;

    /// <summary>
    /// Are we checking for keypress' to change point select keycode?
    /// </summary>
    private bool _CheckKeyPressPoint = false;

    /// <summary>
    /// Are we checking for keypress' to change vertex select keycode?
    /// </summary>
    private bool _CheckKeyPressVertex = false;

    /// <summary>
    /// Mouse position during the drag events
    /// </summary>
    private Vector2 _CurrentDragPosition;

    /// <summary>
    /// Current drawing collider that is selected
    /// </summary>
    private Collider _CurrentDrawingCollider;

    /// <summary>
    /// Local position of current hovered point (not a vertex)
    /// </summary>
    private Vector3 _CurrentHoveredPoint;

    /// <summary>
    /// Transform of current hovered point (not a vertex)
    /// </summary>
    private Transform _CurrentHoveredPointTransform;

    /// <summary>
    /// Local position of current hovered vertex
    /// </summary>
    private Vector3 _CurrentHoveredPosition;

    /// <summary>
    /// Transform of current hovered vertex
    /// </summary>
    private Transform _CurrentHoveredTransform;

    private HashSet<Vector3> _CurrentHoveredVertices;
    /// <summary>
    /// Set of hovered vertices in whip/box select, quicker to just use a hashset of vector3's
    /// </summary>
    private HashSet<Vector3> CurrentHoveredVertices
    {
      get
      {
        if (_CurrentHoveredVertices == null)
        {
          _CurrentHoveredVertices = new HashSet<Vector3>();
        }
        return _CurrentHoveredVertices;
      }
      set { _CurrentHoveredVertices = value; }
    }

    private HashSet<EasyColliderVertex> _CurrentSelectBoxVerts;
    /// <summary>
    /// Set of ECE vertices in whip/box select. These are sent to ECEditor to actually select vertices once the box select drag is done.
    /// </summary>
    private HashSet<EasyColliderVertex> CurrentSelectBoxVerts
    {
      get
      {
        if (_CurrentSelectBoxVerts == null)
        {
          _CurrentSelectBoxVerts = new HashSet<EasyColliderVertex>();
        }
        return _CurrentSelectBoxVerts;
      }
      set { _CurrentSelectBoxVerts = value; }
    }

    private List<string> _CurrentTips;
    /// <summary>
    /// List of current tips being displayed
    /// </summary>
    private List<string> CurrentTips
    {
      get
      {
        if (_CurrentTips == null)
        {
          _CurrentTips = new List<string>();
        }
        return _CurrentTips;
      }
      set
      {
        _CurrentTips = value;
      }
    }

    private EasyColliderEditor _ECEditor;
    /// <summary>
    /// EasyColliderEditor scriptable object.
    /// </summary>
    private EasyColliderEditor ECEditor
    {
      get
      {
        if (_ECEditor == null)
        {
          _ECEditor = ScriptableObject.CreateInstance<EasyColliderEditor>();
        }
        return _ECEditor;
      }
      set { _ECEditor = value; }
    }

    private EasyColliderPreferences _ECEPreferences;
    /// <summary>
    /// Preferences file
    /// </summary>
    private EasyColliderPreferences ECEPreferences
    {
      get
      {
        if (_ECEPreferences == null)
        {
          // load preferences by finding by name & type.
          string[] ecp = AssetDatabase.FindAssets("EasyColliderPreferences t:ScriptableObject");
          if (ecp.Length > 0)
          {
            string assetPath = AssetDatabase.GUIDToAssetPath(ecp[0]);
            if (ecp.Length > 1)
            {
              Debug.LogWarning("Easy Collider Editor has found multiple preferences files. Using the one located at " + assetPath);
            }
            ECEPreferences = AssetDatabase.LoadAssetAtPath(assetPath, typeof(EasyColliderPreferences)) as EasyColliderPreferences;
          }
          else
          {
            // Create a new preferences file.
            string path = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
            string prefPath = path.Remove(path.Length - 21) + "EasyColliderPreferences.asset";
            ECEPreferences = CreateInstance<EasyColliderPreferences>();
            ECEPreferences.SetDefaultValues();
            AssetDatabase.CreateAsset(ECEPreferences, prefPath);
            AssetDatabase.SaveAssets();
            Debug.LogWarning("Easy Collider Editor did not find a preferences file, new preferences file created at " + prefPath);
          }
        }
        return _ECEPreferences;
      }
      set
      {
        _ECEPreferences = value;
      }
    }

    /// <summary>
    /// bool for toggle for dropdown to edit preferences.
    /// </summary>
    private bool _EditPreferences;

    /// <summary>
    /// Are we in a mouse drag event
    /// </summary>
    private bool _IsMouseDrag = false;

    /// <summary>
    /// Did the mouse drag event just end and it needs to be processed?
    /// </summary>
    private bool _IsMouseDragEnd = false;

    /// <summary>
    /// Keeps track of when the last raycast was done when enabled, so that we aren't constantly raycasting / drag selecting
    /// </summary>
    private double _LastSelectionTime = 0.0f;

    /// <summary>
    /// Scroll position for editor window
    /// </summary>
    private Vector2 _ScrollPosition;

    /// <summary>
    /// Color of selection rectangle.
    /// </summary>
    private Color _SelectionRectColor = new Color(0, 255, 0, 0.2f);

    /// <summary>
    /// Display the collider remove tools foldout group?
    /// </summary>
    private bool _ShowColliderRemovalTools = false; // default to false.

    /// <summary>
    /// Disable the vertex selection tools foldout group?
    /// </summary>
    private bool _ShowVertexSelectionTools = true; // default to true.

    /// <summary>
    /// start mouse position of the drag
    /// </summary>
    private Vector2 _StartDragPosition = Vector2.zero;


    [MenuItem("Window/Easy Collider Editor")]
    static void Init()
    {
      EditorWindow ece = EditorWindow.GetWindow(typeof(EasyColliderWindow), false, "Easy Collider Editor");
      ece.Show();
      ece.autoRepaintOnSceneChange = true;
    }

    //NOTE: If something suddenly stops working on a new Unity update, it's likely the delegate names were changed as has occured in the past.
    void OnEnable()
    {
      ECEditor.SetValuesFromPreferences(ECEPreferences);
      // Register to scene updates so we can raycast to the mesh
      //EASY_COLLIDER_EDITOR_DELEGATES - Change the below delegates if something breaks! (and in OnDisable below)
      SceneView.onSceneGUIDelegate += OnSceneGUI;
      // Register to undo/redo to repaint immediately.
      Undo.undoRedoPerformed += OnUndoRedoPerformed;
    }

    void OnDisable()
    {
      ECEditor.SelectedGameObject = null;
      //EASY_COLLIDER_EDITOR_DELEGATES - Change the below delegates if something breaks! (and in OnEnable above)
      SceneView.onSceneGUIDelegate -= OnSceneGUI;
      // Unregister the repaint of window when undo's are performed.
      Undo.undoRedoPerformed -= OnUndoRedoPerformed;
    }

    /// <summary>
    /// Repaints the editor window when undo/redo is done.
    /// </summary>
    void OnUndoRedoPerformed()
    {
      Repaint();
    }

    /// <summary>
    /// Checks if we need to update based on the selected vertex count, then updates the vertex display depending on if we're using gizmos, or shaders
    /// </summary>
    public void CheckUpdateVertexDisplays()
    {
      if (ECEditor.Gizmos != null && ECEditor.Gizmos.SelectedVertexPositions.Count != ECEditor.SelectedVertices.Count)
      {
        UpdateVertexDisplays();
      }
      if (ECEditor.Compute != null && ECEditor.Compute.PointCount != ECEditor.SelectedVertices.Count)
      {
        UpdateVertexDisplays();
      }
    }

    /// <summary>
    /// Updates the gizmos or shaders selected, hover, overlap vertices.
    /// </summary>
    public void UpdateVertexDisplays()
    {
      // Update Gizmos
      if (ECEditor.Gizmos != null)
      {
        ECEditor.Gizmos.SelectedVertexPositions = ECEditor.GetWorldVertices();
        ECEditor.Gizmos.HoveredVertexPositions = CurrentHoveredVertices;
      }
      // Update Compute / Shader script.
      if (ECEditor.Compute != null)
      {
        ECEditor.Compute.UpdateSelectedBuffer(ECEditor.GetWorldVertices());
        ECEditor.Compute.UpdateOverlapHoveredBuffer(CurrentHoveredVertices);
      }
    }

    /// <summary>
    /// Does raycasts and selection in the scene view updates.
    /// </summary>
    /// <param name="sceneView"></param>
    void OnSceneGUI(SceneView sceneView)
    {
      // Cleanup object if we're going into play mode.
      if (EditorApplication.isPlayingOrWillChangePlaymode)
      {
        ECEditor.CleanUpObject(ECEditor.SelectedGameObject, true);
      }

      // force focus scene if preference is set.
      if ((ECEditor.VertexSelectEnabled || ECEditor.ColliderSelectEnabled) && ECEPreferences.ForceFocusScene && ECEditor.SelectedGameObject != null)
      {
        if (EditorWindow.focusedWindow != SceneView.currentDrawingSceneView)
        {
          SceneView.currentDrawingSceneView.Focus();
        }
      }

      // Update tips.
      if (ECEPreferences.DisplayTips)
      {
        UpdateTips();
      }
      // Update vertex displays
      CheckUpdateVertexDisplays();


      // Only use the mouse drag events if vert select is enabled.
      if (ECEditor.VertexSelectEnabled
      && ECEditor.SelectedGameObject != null
      && SceneView.currentDrawingSceneView == EditorWindow.focusedWindow
      && Camera.current != null)
      {
        CheckMouseDrag();
        CheckMouseDragKeyHeld();
      }
      // reset the variables if we don thave a seleted object or vertex select is disabled.
      else if (_BoxMinusHeld || _BoxPlusHeld || _IsMouseDrag || _IsMouseDragEnd)
      {
        _BoxMinusHeld = false;
        _BoxPlusHeld = false;
        _IsMouseDrag = false;
        _IsMouseDragEnd = false;
      }

      // Selection box vertex selection.
      if (_IsMouseDrag
      && ECEditor.SelectedGameObject != null
      && SceneView.currentDrawingSceneView == EditorWindow.focusedWindow
      && Camera.current != null)
      {
        // Draw selection box.
        Handles.BeginGUI();
        Color original = GUI.color;
        EditorGUI.DrawRect(new Rect(_StartDragPosition, _CurrentDragPosition - _StartDragPosition), _SelectionRectColor);
        Handles.EndGUI();
        // we need to draw the UI rect every frame, but should only update the displayed dots occasionally.
        // but we also need to draw them constantly.
        if (EditorApplication.timeSinceStartup - _LastSelectionTime > ECEPreferences.RaycastDelayTime && Camera.current != null)
        {
          _LastSelectionTime = EditorApplication.timeSinceStartup;
          // mouse position has 0,0 at top left
          // world to screen has 0,0 at bottom left
          // so change mouse pos to 0,0 at bottom left.
          // Use camera.current.pixelheight instead of the screen.height
          // screen.height reports the window's height, which includes parts of the scene-view editor window that aren't in the world to screen calculation.
          Vector2 endDragM = _CurrentDragPosition;
          Vector2 startDragM = _StartDragPosition;
          endDragM.y = Camera.current.pixelHeight - _CurrentDragPosition.y;
          startDragM.y = Camera.current.pixelHeight - _StartDragPosition.y;
          Vector3[] verts = new Vector3[0];
          Vector3 currentVertexPos = Vector3.zero;
          Vector3 transformedPoint = Vector3.zero;
          Transform t = ECEditor.SelectedGameObject.transform;
          for (int i = 0; i < ECEditor.MeshFilters.Count; i++)
          {
            if (ECEditor.MeshFilters[i] == null) continue;
            verts = ECEditor.MeshFilters[i].sharedMesh.vertices;
            t = ECEditor.MeshFilters[i].transform;
            for (int j = 0; j < verts.Length; j++)
            {
              transformedPoint = t.TransformPoint(verts[j]);
              currentVertexPos = Camera.current.WorldToScreenPoint(transformedPoint);
              EasyColliderVertex ecv = new EasyColliderVertex(t, verts[j]);
              // if the vertex's screen pos is within the drag area
              if (
               ((currentVertexPos.x >= startDragM.x && currentVertexPos.x <= endDragM.x) || (currentVertexPos.x <= startDragM.x && currentVertexPos.x >= endDragM.x))
               && ((currentVertexPos.y >= startDragM.y && currentVertexPos.y <= endDragM.y) || (currentVertexPos.y <= startDragM.y && currentVertexPos.y >= endDragM.y))
              )
              {
                if (_BoxPlusHeld) // box plus is held
                {
                  if (!ECEditor.SelectedVerticesSet.Contains(ecv)) // if it's not already selected
                  {
                    if (!CurrentHoveredVertices.Contains(transformedPoint)) // and it's not in our hovered list.
                    {
                      CurrentHoveredVertices.Add(transformedPoint); // mark it as hovered.
                      CurrentSelectBoxVerts.Add(ecv);
                    }
                  }
                  else if (CurrentHoveredVertices.Contains(transformedPoint)) // otherwise, if its in the box and currently selected
                  {
                    CurrentHoveredVertices.Remove(transformedPoint); // try to remove it.
                    CurrentSelectBoxVerts.Remove(ecv);
                  }
                }
                else if (_BoxMinusHeld) // box minus is held
                {
                  if (ECEditor.SelectedVerticesSet.Contains(ecv)) // if it's selected
                  {
                    if (!CurrentHoveredVertices.Contains(transformedPoint)) // and it's not in our hovered list
                    {
                      CurrentHoveredVertices.Add(transformedPoint); // add it.
                      CurrentSelectBoxVerts.Add(ecv);
                    }
                  }
                  else if (CurrentHoveredVertices.Contains(transformedPoint)) //otherwise, if it's within the box, and not currently selected.
                  {
                    CurrentHoveredVertices.Remove(transformedPoint); // so try to remove it.
                    CurrentSelectBoxVerts.Remove(ecv);
                  }
                }
                else if (!CurrentHoveredVertices.Contains(transformedPoint)) // default functionality (not currently hovered, but in box -> mark it at hovered.)
                {
                  CurrentHoveredVertices.Add(transformedPoint);
                  CurrentSelectBoxVerts.Add(ecv);
                }
              }
              // remove it if no longer in the box, and in our lists.
              else if (CurrentHoveredVertices.Contains(transformedPoint))
              {
                CurrentSelectBoxVerts.Remove(new EasyColliderVertex(t, verts[j]));
                CurrentHoveredVertices.Remove(transformedPoint);
              }
            }
          }
          // force update selection displays while dragging a box
          UpdateVertexDisplays();
        }
      }
      else if (_IsMouseDragEnd)
      {
        // Done dragging, select everything in the box.
        Undo.RegisterCompleteObjectUndo(ECEditor, "Select Vertices");
        int group = Undo.GetCurrentGroup();
        ECEditor.SelectVertices(CurrentSelectBoxVerts);
        _IsMouseDragEnd = false;
        // Clear sets.
        CurrentHoveredVertices = new HashSet<Vector3>();
        CurrentSelectBoxVerts = new HashSet<EasyColliderVertex>();
        UpdateVertexDisplays();
        Undo.CollapseUndoOperations(group);
        // repaint so buttons appear for vertex selection
        this.Repaint();
      }

      // Vertex / Collider raycast selection.
      // Do vertex selection by raycast only occasionally, and if we are able to
      if (!_IsMouseDrag // not dragging
        && EditorApplication.timeSinceStartup - _LastSelectionTime > ECEPreferences.RaycastDelayTime // raycast occasionally
        && SceneView.currentDrawingSceneView == EditorWindow.focusedWindow // if we're focused on the scene view
        && (ECEditor.VertexSelectEnabled || ECEditor.ColliderSelectEnabled) // and selection is enabled
        && ECEditor.SelectedGameObject != null // and theres something selected
        && ECEditor.MeshFilters.Count > 0 // and there's mesh filters.
        && Camera.current != null & Event.current != null) // and there's a camera and an event to use.
      {
        _LastSelectionTime = EditorApplication.timeSinceStartup;
        RaycastSelect();
        if (ECEditor.VertexSelectEnabled)
        {
          // if we're not collider selecting, update the vertex display
          UpdateVertexDisplays();
        }
      }

      // Draw hovered vertex if it's enabled, and we're hovering a vertex.
      if (ECEditor.VertexSelectEnabled && Event.current.type == EventType.KeyUp && Event.current.isKey)
      {
        // Select the currently hovered vertex if we've pressed the key.
        if (Event.current.keyCode == ECEPreferences.VertSelectKeyCode && _CurrentHoveredTransform != null)
        {
          // Select the vertex then repaint.
          SelectVertex(_CurrentHoveredTransform, _CurrentHoveredPosition);
          this.Repaint();
        }
        // this selects abritrary points on the mesh collider that aren't vertices
        else if (Event.current.keyCode == ECEPreferences.PointSelectKeyCode && _CurrentHoveredPointTransform != null)
        {
          SelectVertex(_CurrentHoveredPointTransform, _CurrentHoveredPoint);
          this.Repaint();
        }
      }

      // Draw selected collider if it's enabled and we have one.
      if (ECEditor.ColliderSelectEnabled && ECEditor.SelectedCollider != null)
      {
        // Draw the collider
        EasyColliderDraw.DrawCollider(ECEditor.SelectedCollider, ECEPreferences.SelectedColliderColour);
        // Repaint the scene view & editor window if the current drawing collider is different.
        if (_CurrentDrawingCollider != ECEditor.SelectedCollider)
        {
          _CurrentDrawingCollider = ECEditor.SelectedCollider;
          SceneView.RepaintAll();
          this.Repaint();
        }
      }

      // Display mesh vertices
      if (ECEditor.SelectedGameObject != null && ECEditor.DisplayMeshVertices)
      {
        DrawAllVertices();
      }
    }

    /// <summary>
    /// Checks and sets the _BoxPlusHeld and _BoxMinusHeld bools based on key events.
    /// </summary>
    private void CheckMouseDragKeyHeld()
    {
      if (Event.current != null)
      {
        if (!_BoxPlusHeld && !_BoxMinusHeld) // if no button is being held
        {
          Event e = Event.current;
          int controlID = GUIUtility.GetControlID(FocusType.Passive);
          EventType type = e.GetTypeForControl(controlID);
          // check to see if the current event is a keydown event.
          if (e.isKey && type == EventType.KeyDown)
          {
            // if its one of our shortcuts, set the bool to true.
            if (e.keyCode == ECEPreferences.BoxSelectPlusKey)
            {
              _BoxPlusHeld = true;
            }
            else if (e.keyCode == ECEPreferences.BoxSelectMinusKey)
            {
              _BoxMinusHeld = true;
            }
          }
        }
        // if one of the keys is held down.
        else if (_BoxPlusHeld || _BoxMinusHeld)
        {
          Event e = Event.current;
          int controlID = GUIUtility.GetControlID(FocusType.Passive);
          EventType type = e.GetTypeForControl(controlID);
          // check to see if the current event is a keyup event
          if (e.isKey && type == EventType.KeyUp)
          {
            // and if it matches one of ours shortcuts, set the bool to false.
            if (e.keyCode == ECEPreferences.BoxSelectPlusKey)
            {
              _BoxPlusHeld = false;
            }
            else if (e.keyCode == ECEPreferences.BoxSelectMinusKey)
            {
              _BoxMinusHeld = false;
            }
          }
        }
      }

    }

    /// <summary>
    /// Checks if left mouse was clicked, and uses the mouse events so that selection boxes can be drawn and used.
    /// </summary>
    private void CheckMouseDrag()
    {
      // reset drag end bool.
      if (_IsMouseDragEnd)
      {
        _IsMouseDragEnd = false;
      }
      // if we have a mouse event.
      if (Event.current != null && Event.current.isMouse)
      {
        Event e = Event.current;
        int controlID = GUIUtility.GetControlID(FocusType.Passive);
        // get event type for the current event.
        switch (e.GetTypeForControl(controlID))
        {
          case EventType.MouseDown:
            if (e.button == 0)
            {
              // get the start mouse position
              _StartDragPosition = e.mousePosition;
              _CurrentDragPosition = e.mousePosition;
              _IsMouseDrag = true;
              // set the control as hot
              GUIUtility.hotControl = controlID;
              // use the event.
              e.Use();
            }
            break;
          case EventType.MouseUp:
            if (e.button == 0)
            {
              // set our bools to track mouse drags, on mouse up the drag has just ended
              _IsMouseDrag = false;
              _IsMouseDragEnd = true;
              // set the control as hot
              GUIUtility.hotControl = 0;
              // use the event.
              e.Use();
            }
            break;
          case EventType.MouseDrag:
            if (e.button == 0)
            {
              // mouse is still dragging, continue to use the event & keep current mouse position.
              _CurrentDragPosition = e.mousePosition;
              // set the control as hot
              GUIUtility.hotControl = controlID;
              // use the event.
              e.Use();
            }
            else { _IsMouseDrag = false; }
            break;
        }
      }
    }

    /// <summary>
    /// Usings a raycast and highlights whatever vertex is the closest.
    /// Sets the current hovered filter and current hovered vertex
    /// Also selects collider
    /// </summary>
    private void RaycastSelect()
    {
      // clear current hovered vertices
      CurrentHoveredVertices.Clear();
      // Use physics scene for the current scene to allow for proper raycasting in the prefab editing scene.
      // PhysicsScene physicsScene = PhysicsSceneExtensions.GetPhysicsScene(ECEditor.SelectedGameObject.scene);
      Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
      RaycastHit hit;
      if (Physics.Raycast(ray, out hit, Mathf.Infinity))
      {
        if (ECEditor.VertexSelectEnabled)
        {
          // Vertex selection.
          float minDistance = Mathf.Infinity;
          Transform closestTransform = ECEditor.SelectedGameObject.transform;
          Vector3 closestLocalPosition = Vector3.zero;
          foreach (MeshFilter meshFilter in ECEditor.MeshFilters)
          {
            if (meshFilter == null) continue;
            // Get transform and verts of each mesh to make things a little quicker.
            Transform t = meshFilter.transform;
            Vector3[] vertices = meshFilter.sharedMesh.vertices;
            // Get the closest by checking the distance.
            // convert world hit point to local hit point for each meshfilter's transform.
            Vector3 localHit = t.InverseTransformPoint(hit.point);
            for (int i = 0; i < vertices.Length; i++)
            {
              float distance = Vector3.Distance(vertices[i], localHit);
              if (distance < minDistance)
              {
                minDistance = distance;
                closestTransform = t;
                closestLocalPosition = vertices[i];
              }
            }
          }
          // if the closest changed from the one we already have.
          if (closestTransform != null)
          {
            CurrentHoveredVertices.Add(closestTransform.TransformPoint(closestLocalPosition));
            _CurrentHoveredPosition = closestLocalPosition;
            _CurrentHoveredTransform = closestTransform;
          }
          _CurrentHoveredPointTransform = hit.transform;
          if (_CurrentHoveredPointTransform != null)
          {
            // with point selection, you can more easily select points that aren't on the selected or child meshes
            _CurrentHoveredPoint = _CurrentHoveredPointTransform.InverseTransformPoint(hit.point);
            CurrentHoveredVertices.Add(hit.point);
            // Point selection requires more repaints to display better, so we'll repaint every time we raycast.
            // since we don't raycast constantly, it's not a huge deal.
            SceneView.RepaintAll();
          }

        }
        else if (ECEditor.ColliderSelectEnabled)
        {
          if (hit.collider != ECEditor.SelectedCollider)
          {
            ECEditor.SelectedCollider = hit.collider;
            // so we display the remove button immediately.
            this.Repaint();
          }
        }
      }
      else if (ECEditor.VertexSelectEnabled && _CurrentHoveredTransform != null)
      {
        // clear hovered display if we're not over anything.
        CurrentHoveredVertices.Remove(_CurrentHoveredTransform.TransformPoint(_CurrentHoveredPosition));
        _CurrentHoveredTransform = null;
        SceneView.RepaintAll();
      }
    }

    /// <summary>
    /// Draws all vertices in Editor's mesh filter list
    /// /// </summary>
    private void DrawAllVertices()
    {
      // get all world mesh vertices is super slow.
      if (ECEditor.Gizmos != null)
      {
        ECEditor.Gizmos.DisplayVertexPositions = ECEditor.GetAllWorldMeshVertices();
      }
      else if (ECEditor.Compute != null)
      {
        ECEditor.Compute.SetDisplayAllBuffer(ECEditor.GetAllWorldMeshVertices());
      }
    }


    private Color DisabledButtonColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    private Color TempGUIColor;
    /// <summary>
    /// Creates a button that displays different if it is enabled or disabled. Button always returns false if disabled.
    /// <param name="title">text of button</param>
    /// <param name="enabledTooltip">tool tip for button when enabled</param>
    /// <param name="disabledTooltip">tool tip for box when disabled</param>
    /// <param name="isEnabled">is the button enabled?</param>
    /// <returns>false if disabled, true if enabled and button is clicked</returns>
    bool GUIButton(string text, string enabledTooltip, string disabledTooltip, bool isEnabled)
    {
      // only display the button as a button if it's actually enabled
      if (isEnabled)
      {
        if (GUILayout.Button(new GUIContent(text, enabledTooltip)))
        {
          return true;
        }
      }
      else
      {
        // create the style to take up the space space as an enabled buttons default sizing.
        GUIStyle box = new GUIStyle(GUI.skin.box);

        box.padding = GUI.skin.button.padding;
        box.margin = GUI.skin.button.margin;
        TempGUIColor = GUI.color;
        GUI.color = DisabledButtonColor;
        GUILayout.Box(new GUIContent(text, disabledTooltip), box, GUILayout.ExpandWidth(true));
        GUI.color = TempGUIColor;
      }
      // always return false, like a normal button, unless the actual enabled button is pressed.
      return false;
    }

    private Color disabledToggleColor = new Color(1, 1, 1, 0.33f);
    /// <summary>
    /// Creates a left toggle if the toggle is enabled that functions normally,
    /// otherwise creates a style toggle that is not toggleable and grayed-out.
    /// </summary>
    /// <param name="text">Text to show beside the toggle</param>
    /// <param name="enabledTooltip">Tool tip when toggle is enabled</param>
    /// <param name="disabledTooltip">Tool tip when toggle is disabled</param>
    /// <param name="isEnabled">Is the toggle enabled</param>
    /// <param name="toggle">Bool the toggle controls</param>
    /// <returns>Value of toggle</returns>
    bool GUIToggleLeft(string text, string enabledTooltip, string disabledTooltip, bool isEnabled, bool toggle)
    {
      if (isEnabled)
      {
        bool toggleValue = EditorGUILayout.ToggleLeft(new GUIContent(text, enabledTooltip), toggle);
        return toggleValue;
      }
      else
      {
        Color color = GUI.backgroundColor;
        GUI.backgroundColor = disabledToggleColor;
        EditorGUILayout.ToggleLeft(new GUIContent(text, disabledTooltip), toggle);
        GUI.backgroundColor = color;
      }
      return toggle;
    }

    /// <summary>
    /// Draws the GUI
    /// </summary>
    void OnGUI()
    {
      // Clear editor window's lists if we've deselected the objects.
      if (!ECEditor.VertexSelectEnabled || ECEditor.SelectedGameObject == null || ECEditor.MeshFilters.Count == 0)
      {
        CurrentHoveredVertices = new HashSet<Vector3>();
        CurrentSelectBoxVerts = new HashSet<EasyColliderVertex>();
      }

      // scrollable window.
      _ScrollPosition = EditorGUILayout.BeginScrollView(_ScrollPosition);
      // Selected Gameobject field.
      EditorGUI.BeginChangeCheck();
      GameObject selected = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Selected GameObject", "Selected GameObject is usually the gameobject with the mesh, or it's parent."), ECEditor.SelectedGameObject, typeof(GameObject), true);
      if (EditorGUI.EndChangeCheck())
      {
        // NOTE that using a name and registering a group and collapsing doesn't actually work, undo's will only display the last undo name
        // when when adding components is add component.
        // I will leave the code in places where undo's should be grouped and named correctly in the hopes that one day it works as expected
        // even though this bug is listed as will not fix in unity's bug reports
        Undo.RegisterCompleteObjectUndo(ECEditor, "Change Selected Object");
        int group = Undo.GetCurrentGroup();
        ECEditor.SelectedGameObject = selected;
        SetGizmoPreferences();
        SetShaderPreferences();
        Undo.CollapseUndoOperations(group);
      }

      // Attach to gameobject field.
      EditorGUI.BeginChangeCheck();
      GameObject attachTo = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Attach Collider To", "Gameobject to attach the collider to, usually the selected gameobject."), ECEditor.AttachToObject, typeof(GameObject), true);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RegisterCompleteObjectUndo(ECEditor, "Change AttachTo GameObject");
        ECEditor.AttachToObject = attachTo;
      }

      EditorGUI.BeginChangeCheck();
      bool vertexToggle = GUIToggleLeft("Enable Vertex Selection", "Allows selection of vertices and points by raycast in the sceneview", "Select a gameobject with a mesh, or enable child meshes.", ECEditor.SelectedGameObject != null && ECEditor.MeshFilters.Count > 0, ECEditor.VertexSelectEnabled);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RegisterCompleteObjectUndo(ECEditor, "Toggle Vertex Selection");
        int group = Undo.GetCurrentGroup();
        ECEditor.VertexSelectEnabled = vertexToggle;
        if (vertexToggle)
        {
          ECEditor.ColliderSelectEnabled = false;
        }
        Undo.CollapseUndoOperations(group);
      }

      // Collider selection
      EditorGUI.BeginChangeCheck();
      bool colliderSelectEnabled = GUIToggleLeft("Enable Collider Selection", "Allows selection of colliders by raycast in the sceneview.", "Select a GameObject.", ECEditor.SelectedGameObject != null, ECEditor.ColliderSelectEnabled);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RegisterCompleteObjectUndo(ECEditor, "Toggle Collider Selection");
        ECEditor.ColliderSelectEnabled = colliderSelectEnabled;
        if (ECEditor.ColliderSelectEnabled)
        {
          ECEditor.VertexSelectEnabled = false;
        }
      }

      // Display all vertices toggle
      EditorGUI.BeginChangeCheck();
      bool displayMeshVertices = GUIToggleLeft("Display Mesh Vertices", "Helps make sure everything is properly set up, as it will display all the vertices that are able to be selected.", "Select a GameObject.",
        ECEditor.SelectedGameObject != null,
        ECEditor.DisplayMeshVertices);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RegisterCompleteObjectUndo(ECEditor, "Display all vertices");
        int group = Undo.GetCurrentGroup();
        ECEditor.DisplayMeshVertices = displayMeshVertices;
        Undo.CollapseUndoOperations(group);
        // Repaint so it gets updated immediately.
        SceneView.RepaintAll();
      }


      // Include child meshes.
      EditorGUI.BeginChangeCheck();
      bool includeChildMeshes = EditorGUILayout.ToggleLeft(new GUIContent("Include Child Meshes", "Enables child mesh vertices in vertex selection."), ECEditor.IncludeChildMeshes);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RegisterCompleteObjectUndo(ECEditor, "ECE: " + (includeChildMeshes ? "Enable " : "Disable ") + " include child meshes.");
        int group = Undo.GetCurrentGroup();
        ECEditor.IncludeChildMeshes = includeChildMeshes;
        Undo.CollapseUndoOperations(group);
      }

      // create as trigger.
      EditorGUI.BeginChangeCheck();
      bool createAsTrigger = EditorGUILayout.ToggleLeft(new GUIContent("Create as Trigger", "Creates the colliders as triggers"), ECEditor.IsTrigger);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RegisterCompleteObjectUndo(ECEditor, "Toggle Create As Trigger");
        ECEditor.IsTrigger = createAsTrigger;
      }

      // Rotated Collider Layer
      if (!ECEPreferences.RotatedOnSelectedLayer)
      {
        EditorGUI.BeginChangeCheck();
        int rotatedColliderLayer = EditorGUILayout.LayerField(new GUIContent("Rotated Collider Layer:", "The layer to set on the rotated collider's gameobject/transform on creation."), ECEditor.RotatedColliderLayer);
        if (EditorGUI.EndChangeCheck())
        {
          ECEditor.RotatedColliderLayer = rotatedColliderLayer;
        }
      }

      // Physic material
      EditorGUI.BeginChangeCheck();
      PhysicMaterial physicMaterial = (PhysicMaterial)EditorGUILayout.ObjectField(new GUIContent("Add PhysicMaterial", "PhysicMaterial to set on collider upon creation."), ECEditor.PhysicMaterial, typeof(PhysicMaterial), false);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RegisterCompleteObjectUndo(ECEditor, "Set PhysicMaterial");
        ECEditor.PhysicMaterial = physicMaterial;
      }

      // Vertex selection tools are a foldout that starts opened by default.
      _ShowVertexSelectionTools = EditorGUILayout.Foldout(_ShowVertexSelectionTools, "Vertex Selection Tools");
      if (_ShowVertexSelectionTools)
      {
        // deselect all
        if (GUIButton("Deselect All Vertices",
          "Deselects all currently selected points.",
          "No points are currently selected.",
            ECEditor.SelectedVertices.Count > 0))
        {
          Undo.RegisterCompleteObjectUndo(ECEditor, "Deselect All Vertices");
          int group = Undo.GetCurrentGroup();
          ECEditor.ClearSelectedVertices();
          Undo.CollapseUndoOperations(group);
          // repaint to update quickly, as clicking the editor window de-focuses the scene which stops the visual update.
          SceneView.RepaintAll();
        }

        // invert selected
        if (GUIButton("Invert Selected Vertices",
        "Deselects all currently selected vertices and points, and selects all unselected vertices.",
        "No gameobject is currently selected.",
        ECEditor.SelectedGameObject != null
        ))
        {
          Undo.RegisterCompleteObjectUndo(ECEditor, "Invert vertices selection");
          int group = Undo.GetCurrentGroup();
          ECEditor.InvertSelectedVertices();
          Undo.CollapseUndoOperations(group);
          // need to update displays as inverting can result in same # of vertices in the lists, so the check update won't detect a change.
          UpdateVertexDisplays();
          SceneView.RepaintAll();
        }
        // grow all vertices
        if (GUIButton("Grow Selected Vertices",
        "Grows the selection of vertices to all the connected vertices.",
        "No vertices are current selected.",
        ECEditor.SelectedVertices.Count > 0))
        {
          Undo.RegisterCompleteObjectUndo(ECEditor, "Grow selected vertices");
          int group = Undo.GetCurrentGroup();
          ECEditor.GrowAllSelectedVertices();
          Undo.CollapseUndoOperations(group);
          SceneView.RepaintAll();
        }
        // grow last select vertices
        if (GUIButton("Grow Last Selected Vertices",
        "Grows the selection of vertices from the last selected vertices",
        "No vertices are currently selected.",
        ECEditor.SelectedVertices.Count > 0))
        {
          Undo.RegisterCompleteObjectUndo(ECEditor, "Grow selected vertices");
          int group = Undo.GetCurrentGroup();
          ECEditor.GrowLastSelectedVertices();
          Undo.CollapseUndoOperations(group);
          SceneView.RepaintAll();
        }
        if (GUIButton("Ring Select",
        "Attempts to do a ring select from the last 2 vertices around the object.",
        "At least 2 vertices must be selected to do a ring select.",
        ECEditor.SelectedVertices.Count >= 2))
        {
          Undo.RegisterCompleteObjectUndo(ECEditor, "Ring select vertices");
          int group = Undo.GetCurrentGroup();
          ECEditor.RingSelectVertices();
          Undo.CollapseUndoOperations(group);
          SceneView.RepaintAll();
        }
      }
      // EditorGUILayout.EndFoldoutHeaderGroup();

      //space between tools and creation buttons.
      EditorGUILayout.Space();

      // create a box collider button
      if (GUIButton("Create Box Collider",
        "Creates a box collider from the selected points.",
        "At least 2 points must be selected to create a box collider.",
        ECEditor.SelectedVertices.Count >= 2))
      {
        CreateCollider(CREATE_COLLIDER_TYPE.BOX, "Create Box Collider");
      }
      // rotated box button
      if (GUIButton("Create Rotated Box Collider",
        "Creates a rotated box collider from the selected points.",
        "At least 3 points must be selected to create a rotated box collider.",
        ECEditor.SelectedVertices.Count >= 3))
      {
        CreateCollider(CREATE_COLLIDER_TYPE.ROTATED_BOX, "Create Rotated Box Collider");
      }
      // sphere collider button
      if (GUIButton("Create Sphere Collider",
        "Creates a sphere collider from the selected points using the Sphere Method selected.",
        "At least 2 points must be selected to create a sphere collider.",
        ECEditor.SelectedVertices.Count >= 2))
      {
        CreateCollider(CREATE_COLLIDER_TYPE.SPHERE, "Create Sphere Collider");
      }
      // sphere method enum
      ECEPreferences.SphereColliderMethod = (SPHERE_COLLIDER_METHOD)EditorGUILayout.EnumPopup(new GUIContent("Sphere Method:", "Algorithm to use during sphere creation."), ECEPreferences.SphereColliderMethod);
      // capsule collider button
      if (GUIButton("Create Capsule Collider",
      "Creates a capsule collider from the points selected using the Capsule Method selected.",
        ECEPreferences.CapsuleColliderMethod == CAPSULE_COLLIDER_METHOD.BestFit ?
        "At least 3 points must be selected to use the Best Fit Capsule Method." :
        "At least 2 points must be selected to use the Min Max Capsule Method.",
        ECEPreferences.CapsuleColliderMethod == CAPSULE_COLLIDER_METHOD.BestFit ?
        ECEditor.SelectedVertices.Count >= 3 :
        ECEditor.SelectedVertices.Count >= 2
        ))
      {
        CreateCollider(CREATE_COLLIDER_TYPE.CAPSULE, "Create Capsule Collider");
      }
      // rotated capsule collider
      if (GUIButton("Create Rotated Capsule Collider",
      "Creates a rotated capsule collider from the points selected using the Capsule Method selected.",
      "At least 3 points must be selected to create a rotated capsule collider.",
      ECEditor.SelectedVertices.Count >= 3))
      {
        CreateCollider(CREATE_COLLIDER_TYPE.ROTATED_CAPSULE, "Create Rotated Capsule Collider");
      }
      // capsule method enum
      ECEPreferences.CapsuleColliderMethod = (CAPSULE_COLLIDER_METHOD)EditorGUILayout.EnumPopup(new GUIContent("Capsule Method:", "Algorithm to use during capsule collider creation."), ECEPreferences.CapsuleColliderMethod);

      // space between create colliders & remove colliders.
      EditorGUILayout.Space();

      // make the removal tools a foldout group
      _ShowColliderRemovalTools = EditorGUILayout.Foldout(_ShowColliderRemovalTools, "Collider Removal Tools");
      if (_ShowColliderRemovalTools)
      {

        if (GUIButton("Remove Selected Collider",
        "Removes the collider that is currently selected, this collider is drawn by the color set in the preferences.",
        "No collider is currently selected.",
        colliderSelectEnabled && ECEditor.SelectedCollider != null))
        {
          Undo.RegisterCompleteObjectUndo(ECEditor, "Remove Collider");
          int group = Undo.GetCurrentGroup();
          ECEditor.RemoveSelectedCollider();
          Undo.CollapseUndoOperations(group);
        }

        if (GUIButton("Remove All Colliders",
         "Removes all colliders on the selected gameobject AND children if include child meshes is enabled.",
         "No gameobject is currently selected.",
         ECEditor.SelectedGameObject != null
        ))
        {
          Undo.RegisterCompleteObjectUndo(ECEditor, "Remove All Colliders");
          int group = Undo.GetCurrentGroup();
          ECEditor.RemoveAllColliders();
          Undo.CollapseUndoOperations(group);
        }
      }

      // space between removal tools & finish.
      EditorGUILayout.Space();
      if (GUIButton("Finish Currently Selected GameObject", "Cleans up the currently selected gameobject and deselects it.", "No GameObject is currently selected.", ECEditor.SelectedGameObject != null))
      {
        Undo.RegisterCompleteObjectUndo(ECEditor.SelectedGameObject, "Finish Currently Selected GameObject");
        int group = Undo.GetCurrentGroup();
        Undo.RegisterCompleteObjectUndo(ECEditor, "Finish Currently Selected GameObject");
        ECEditor.CleanUpObject(ECEditor.SelectedGameObject, false);
        ECEditor.SelectedGameObject = null;
        Undo.CollapseUndoOperations(group);
      }

      // space between finish & preferences.
      EditorGUILayout.Space();

      // Draw preferences in foldout menu
      _EditPreferences = EditorGUILayout.Foldout(_EditPreferences, new GUIContent("Edit Preferences", "Allows you to edit preferences for various settings."));
      if (_EditPreferences)
      {
        DrawPreferences();
      }

      // Add a flexible space, so tips are displayed at the bottom.
      GUILayout.FlexibleSpace();

      // Draw tips if set in preferences.
      if (ECEPreferences.DisplayTips)
      {
        UpdateTips();
        if (CurrentTips.Count > 0)
        {
          GUIStyle tipStyle = new GUIStyle(GUI.skin.label);
          tipStyle.fontStyle = FontStyle.Bold;
          tipStyle.alignment = TextAnchor.UpperCenter;
          GUILayout.Label("Tips", tipStyle);
          tipStyle.wordWrap = true;
          tipStyle.alignment = TextAnchor.UpperLeft;
          tipStyle.fontStyle = FontStyle.Normal;
          foreach (string tip in CurrentTips)
          {
            EditorGUILayout.LabelField("- " + tip, tipStyle);
          }
        }
      }

      // End of gui
      // end scroll view.
      EditorGUILayout.EndScrollView();
    }

    private Color _ColorField = Color.white;
    /// <summary>
    /// Creates an undoable color field
    /// </summary>
    /// <param name="obj">Object to record the undo on</param>
    /// <param name="content">GUI Content for the auto-layout field</param>
    /// <param name="undoString">String to use for undos</param>
    /// <param name="value">Value of the color</param>
    private void ColorFieldUndoable(UnityEngine.Object obj, GUIContent content, string undoString, ref Color value)
    {
      _ColorField = value;
      EditorGUI.BeginChangeCheck();
      _ColorField = EditorGUILayout.ColorField(content, _ColorField);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RegisterCompleteObjectUndo(obj, undoString);
        value = _ColorField;
      }
    }

    private float _FloatField = 0.0f;
    /// <summary>
    /// Creates an undoable float field.
    /// </summary>
    /// <param name="obj">Object to record the undo on</param>
    /// <param name="content">GUI Content for the auto-layout field</param>
    /// <param name="undoString">String to use for undos</param>
    /// <param name="value">Value of the float</param>
    private void FloatFieldUndoable(UnityEngine.Object obj, GUIContent content, string undoString, ref float value)
    {
      _FloatField = value;
      EditorGUI.BeginChangeCheck();
      _FloatField = EditorGUILayout.FloatField(content, _FloatField);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RegisterCompleteObjectUndo(obj, undoString);
        value = _FloatField;
      }
    }

    private bool _ToggleField = false;
    /// <summary>
    /// Creates an undoable toggle field.
    /// </summary>
    /// <param name="obj">Object to record the undo on</param>
    /// <param name="content">GUI Content for the auto-layout field</param>
    /// <param name="undoString">String to use for undos</param>
    /// <param name="value">Value of the toggle</param>
    private void ToggleLeftUndoable(UnityEngine.Object obj, GUIContent content, string undoString, ref bool value)
    {
      _ToggleField = value;
      EditorGUI.BeginChangeCheck();
      _ToggleField = EditorGUILayout.ToggleLeft(content, _ToggleField);
      if (EditorGUI.EndChangeCheck())
      {
        // again record only works in some cases, and complete works significantly better.
        // ie can't record changing DrawGizmos without the complete object undo.
        Undo.RegisterCompleteObjectUndo(obj, undoString);
        value = _ToggleField;
      }
    }

    private bool _CheckKeyBoxPlus = false;
    private bool _CheckKeyBoxMinus = false;

    /// <summary>
    /// Draws the Preferences UI.
    /// </summary>
    private void DrawPreferences()
    {
      // Vert and point select keys, box + and minus keys first, as they have their own undo thing.
      ChangeButtonKeyCodeUndoable("Vertex Select Key:", "Key used to select vertices on the mesh.", ref ECEPreferences.VertSelectKeyCode, ref _CheckKeyPressVertex);
      ChangeButtonKeyCodeUndoable("Point Select Key:", "Key used to select points on the mesh.", ref ECEPreferences.PointSelectKeyCode, ref _CheckKeyPressPoint);
      ChangeButtonKeyCodeUndoable("Box Select+ Key:", "Key held before box select to only add points from a box select.", ref ECEPreferences.BoxSelectPlusKey, ref _CheckKeyBoxPlus);
      ChangeButtonKeyCodeUndoable("Box Select- Key:", "Key held before box select to only remove points from a box select", ref ECEPreferences.BoxSelectMinusKey, ref _CheckKeyBoxMinus);

      // over-all changecheck used to check undo's for all prefs.
      EditorGUI.BeginChangeCheck();

      // all the toggles.
      ToggleLeftUndoable(ECEPreferences, new GUIContent("Force Focus Scene", "Forces sceneview focus when vertex or collider selection is enabled. The scene view constantly being refocused can prevent editing of any fields when selections are enabled."), "Toggle force focus scene", ref ECEPreferences.ForceFocusScene);
      ToggleLeftUndoable(ECEPreferences, new GUIContent("Display Tips", "Disable to stop helpful tips from displaying at the bottom of this window."), "Toggle display tips", ref ECEPreferences.DisplayTips);
      ToggleLeftUndoable(ECEPreferences, new GUIContent("Rotated Collider on Selected GameObject Layer", "When enabled uses the selected gameobject's layer when creating rotated colliders. When disabled lets you choose the layer from a dropdown menu."), "Toggle rotated on selected layer", ref ECEPreferences.RotatedOnSelectedLayer);
      ToggleLeftUndoable(ECEPreferences, new GUIContent("Include Child Skinned Meshes", "Automatically includes skinned meshes when include child meshes is enabled."), "Toggle auto include child skinned meshes", ref ECEPreferences.AutoIncludeChildSkinnedMeshes);
      ToggleLeftUndoable(ECEPreferences, new GUIContent("Temporarily disable created colliders", "Created colliders get disabled upon creation, then enabled when done with that gameobject. Makes vertex selection easier when creating multiple colliders."), "Toggle create colliders disabled", ref ECEPreferences.CreatedColliderDisabled);

      // Colors & sizes of displayed vertices.
      ColorFieldUndoable(ECEPreferences, new GUIContent("Selected Vertex Color:", "Color of selected vertices gizmo."), "Change selected vertices color", ref ECEPreferences.SelectedVertColour);
      FloatFieldUndoable(ECEPreferences, new GUIContent("Selected Vertex Scaling:", "Size of selected vertices gizmo."), "Change selected vertices scale", ref ECEPreferences.SelectedVertScaling);

      ColorFieldUndoable(ECEPreferences, new GUIContent("Hover Vertex Color:", "Color of hovered vertices gizmo."), "Change hovered vertices color", ref ECEPreferences.HoverVertColour);
      FloatFieldUndoable(ECEPreferences, new GUIContent("Hover Vertex Scaling:", "Size of hovered vertices gizmo."), "Change hovered vertices scale", ref ECEPreferences.HoverVertScaling);

      ColorFieldUndoable(ECEPreferences, new GUIContent("Overlap Vertex Color:", "Color of overlapped (already selected and current hovered) vertices gizmo."), "Change overlapped vertices color", ref ECEPreferences.OverlapSelectedVertColour);
      FloatFieldUndoable(ECEPreferences, new GUIContent("Overlap Vertex Scaling:", "Size of overlapped (already selected and current hovered) vertices gizmo."), "Change overlapped vertices scale", ref ECEPreferences.OverlapSelectedVertScale);

      ColorFieldUndoable(ECEPreferences, new GUIContent("Display Vertex Color:", "Color of display vertices gizmo."), "Change displayed vertices color", ref ECEPreferences.DisplayVerticesColour);
      FloatFieldUndoable(ECEPreferences, new GUIContent("Display Vertex Scaling:", "Size of display vertices gizmo."), "Change displayed vertices scale", ref ECEPreferences.DisplayVerticesScaling);

      ColorFieldUndoable(ECEPreferences, new GUIContent("Selected Collider Color:", "Color used to draw the currently selected collider."), "Change selected collider color", ref ECEPreferences.SelectedColliderColour);

      // Raycast delay time.
      FloatFieldUndoable(ECEPreferences, new GUIContent("Raycast Delay:", "How often to do a raycast to select a vertex / collider."), "Change raycast delay time", ref ECEPreferences.RaycastDelayTime);

      // shader vs gizmo for rendering all points
      EditorGUI.BeginChangeCheck();
      RENDER_POINT_TYPE render_type = (RENDER_POINT_TYPE)EditorGUILayout.EnumPopup(new GUIContent("Render Vertex Method:", "Gizmos are usable by everyone, but slow when large amount of vertices are selected. The shader uses a compute buffer which requires at least shader model 4.5, but is significantly faster."), ECEPreferences.RenderPointType);
      if (EditorGUI.EndChangeCheck())
      {
        Undo.RecordObject(ECEPreferences, "Change Render Method");
        ECEPreferences.RenderPointType = render_type;
      }

      // if using gizmos:
      if (ECEPreferences.RenderPointType == RENDER_POINT_TYPE.GIZMOS)
      {
        if (ECEditor.Gizmos != null)
        {
          ToggleLeftUndoable(ECEditor.Gizmos, new GUIContent("Draw Gizmos", "Drawing gizmo can be slow with a significant number of vertices enabled."), "Toggle Draw Gizmos", ref ECEditor.Gizmos.DrawGizmos);
        }
        EditorGUI.BeginChangeCheck();
        GIZMO_TYPE gizmo_type = (GIZMO_TYPE)EditorGUILayout.EnumPopup(new GUIContent("Gizmo Type:", "Type of gizmos to draw for selected/hovered/displayed vertices"), ECEPreferences.GizmoType);
        if (EditorGUI.EndChangeCheck())
        {
          Undo.RecordObject(ECEPreferences, "Change gizmo type");
          ECEPreferences.GizmoType = gizmo_type;
        }
        ToggleLeftUndoable(ECEPreferences, new GUIContent("Use Fixed Gizmo Scale", "If true uses a fixed screen size for hovered, selected, and displayed vertices regardless of world position."), "Toggle use fixed gizmo scale", ref ECEPreferences.UseFixedGizmoScale);
      }

      // reset preferences to all default values.
      if (GUILayout.Button(new GUIContent("Reset Preferences to Default", "Resets preferences to their default settings.")) && EditorUtility.DisplayDialog("Reset Preferences", "Are you sure you want to reset Easy Collider Editor's preferences to the default values?", "Yes", "Cancel"))
      {
        ECEPreferences.SetDefaultValues();
        _CheckKeyBoxMinus = false;
        _CheckKeyBoxPlus = false;
        _CheckKeyPressPoint = false;
        _CheckKeyPressVertex = false;
      }

      // if any of the preferences changed, set the things that need preferences.
      if (EditorGUI.EndChangeCheck())
      {
        EditorUtility.SetDirty(ECEPreferences);
        Undo.RegisterCompleteObjectUndo(ECEditor, "Change preferences");
        int group = Undo.GetCurrentGroup();
        ECEditor.SetValuesFromPreferences(ECEPreferences);
        SetGizmoPreferences();
        SetShaderPreferences();
        Undo.CollapseUndoOperations(group);
      }
    }

    /// <summary>
    /// Creates a button that allows undoable changing of a keycode value.
    /// Button displays current keycode, then press any key when pressed and listens for a keypress, then updates the keycode.
    /// </summary>
    /// <param name="label">Label to display beside button</param>
    /// <param name="key">KeyCode to change. Should be unique for each button.</param>
    /// <param name="isChanging">Bool representing whether it should be listening to key presses. Should be unique for each button.</param>
    private void ChangeButtonKeyCodeUndoable(string label, string labelTooltip, ref KeyCode key, ref bool isChanging)
    {
      GUIStyle pressedButtonStyle = new GUIStyle(GUI.skin.box);
      pressedButtonStyle.fontStyle = FontStyle.Bold;
      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.LabelField(new GUIContent(label, labelTooltip), GUILayout.ExpandWidth(false));
      string buttonTitle = isChanging ? "Press a key" : key.ToString();
      if (GUILayout.Button(new GUIContent(buttonTitle, "Click then press a key to change."), isChanging ? pressedButtonStyle : GUI.skin.button, GUILayout.ExpandWidth(true)))
      {
        isChanging = true;
      }
      EditorGUILayout.EndHorizontal();
      if (isChanging)
      {
        if (CheckKeypressChangeUndoable(ECEPreferences, ref key))
        {
          isChanging = false;
          this.Repaint();
        }
      }
    }

    /// <summary>
    /// Changes the KeyCode of keyCode through an undoable action when a key is pressed down.
    /// </summary>
    /// <param name="obj">Object to record undo on.</param>
    /// <param name="keyCode">KeyCode to change to new key</param>
    /// <returns>true if key was changed.</returns>
    private bool CheckKeypressChangeUndoable(Object obj, ref KeyCode keyCode)
    {
      if (Event.current != null // have an event.
        && Event.current.type == EventType.KeyDown // a key down event.
        && Event.current.keyCode != KeyCode.None) // a key down event that wasn't None.
      {
        Undo.RecordObject(obj, "Change keycode");
        keyCode = Event.current.keyCode;
        return true;
      }
      return false;
    }

    /// <summary>
    /// Sets Gizmo Colors and Scaling if the gizmo component exists.
    /// </summary>
    private void SetGizmoPreferences()
    {
      if (ECEditor.Gizmos != null)
      {
        Undo.RegisterCompleteObjectUndo(ECEditor.Gizmos, "Change Preferences");
        int group = Undo.GetCurrentGroup();
        ECEditor.Gizmos.SelectedVertexColor = ECEPreferences.SelectedVertColour;
        ECEditor.Gizmos.SelectedVertexScale = ECEPreferences.SelectedVertScaling;

        ECEditor.Gizmos.HoveredVertexColor = ECEPreferences.HoverVertColour;
        ECEditor.Gizmos.HoveredVertexScale = ECEPreferences.HoverVertScaling;

        ECEditor.Gizmos.OverlapVertexColor = ECEPreferences.OverlapSelectedVertColour;
        ECEditor.Gizmos.OverlapVertexScale = ECEPreferences.OverlapSelectedVertScale;

        ECEditor.Gizmos.DisplayVertexColor = ECEPreferences.DisplayVerticesColour;
        ECEditor.Gizmos.DisplayVertexScale = ECEPreferences.DisplayVerticesScaling;

        ECEditor.Gizmos.UseFixedGizmoScale = ECEPreferences.UseFixedGizmoScale;

        ECEditor.Gizmos.GizmoType = ECEPreferences.GizmoType;
        Undo.CollapseUndoOperations(group);
      }
    }

    /// <summary>
    /// Sets the values on the shader based on the preferences.
    /// </summary>
    private void SetShaderPreferences()
    {
      if (ECEditor.Compute != null)
      {
        Undo.RegisterCompleteObjectUndo(ECEditor.Compute, "Change Preferences");
        int group = Undo.GetCurrentGroup();
        // adjust scaling value to shader to fit gizmos size.
        ECEditor.Compute.SelectedSize = ECEPreferences.SelectedVertScaling / 10;
        ECEditor.Compute.SelectedColor = ECEPreferences.SelectedVertColour;

        ECEditor.Compute.HoveredSize = ECEPreferences.HoverVertScaling / 10;
        ECEditor.Compute.HoveredColor = ECEPreferences.HoverVertColour;

        ECEditor.Compute.OverlapSize = ECEPreferences.OverlapSelectedVertScale / 10;
        ECEditor.Compute.OverlapColor = ECEPreferences.OverlapSelectedVertColour;

        ECEditor.Compute.DisplayAllSize = ECEPreferences.DisplayVerticesScaling / 10;
        ECEditor.Compute.DisplayAllColor = ECEPreferences.DisplayVerticesColour;
        Undo.CollapseUndoOperations(group);
      }
    }

    /// <summary>
    /// Registers an undo and selects a vertex.
    /// </summary>
    /// <param name="transform">transform of vertex' mesh filter to select</param>
    /// <param name="localPosition">local position of vertex</param>
    private void SelectVertex(Transform transform, Vector3 localPosition)
    {
      Undo.RegisterCompleteObjectUndo(ECEditor, "Select Vertex");
      ECEditor.SelectVertex(new EasyColliderVertex(transform, localPosition));
    }

    /// <summary>
    /// Updates all the tips in to display using CurrentTips list.
    /// </summary>
    private void UpdateTips()
    {
      int preUpdateCount = CurrentTips.Count;
      if (ECEditor.SelectedGameObject != null)
      {

        UpdateTip(ECEditor.SelectedGameObject != null && ECEditor.MeshFilters.Count == 0, EasyColliderTips.NO_MESH_FILTER_FOUND);
        if (SceneView.currentDrawingSceneView != null)
        {
          UpdateTip(ECEditor.VertexSelectEnabled && EditorWindow.focusedWindow != SceneView.currentDrawingSceneView, EasyColliderTips.WRONG_FOCUSED_WINDOW);
        }
        UpdateTip(ECEditor.VertexSelectEnabled && EditorApplication.isPlayingOrWillChangePlaymode, EasyColliderTips.IN_PLAY_MODE);
        UpdateTip(ECEditor.VertexSelectEnabled && ECEPreferences.ForceFocusScene, EasyColliderTips.FORCED_FOCUSED_WINDOW);
        UpdateTip(ECEditor.VertexSelectEnabled && _EditPreferences && ECEPreferences.ForceFocusScene, EasyColliderTips.EDIT_PREFS_FORCED_FOCUSED);
        // https://docs.unity3d.com/Manual/SL-ShaderCompileTargets.html, 4.5+ has compute shaders.
        UpdateTip(SystemInfo.graphicsShaderLevel < 45, EasyColliderTips.COMPUTE_SHADER_TIP);
      }
      else if (CurrentTips.Count > 0)
      {
        // Clear tips if we dont have anything selected.
        CurrentTips = new List<string>();
      }
      // Always display the check documentation tip.
      UpdateTip(true, EasyColliderTips.CHECK_DOCUMENTATION_REMINDER);
      // Repaint the Editor window if tips have changed.
      if (preUpdateCount != CurrentTips.Count)
      {
        Repaint();
      }
    }

    /// <summary>
    /// Adds or Removes tips from CurrentTips based on whether it should be displayed or not.
    /// </summary>
    /// <param name="displayTip">Should this tip be displayed?</param>
    /// <param name="tip">String of tip to display.</param>
    /// <returns></returns>
    private bool UpdateTip(bool displayTip, string tip)
    {
      if (displayTip)
      {
        if (!CurrentTips.Contains(tip))
        {
          CurrentTips.Add(tip);
          return true;
        }
        return false;
      }
      else
      {
        return CurrentTips.Remove(tip);
      }
    }

    /// <summary>
    /// Creates a collider of collider type, with the undo string being displayed.
    /// </summary>
    /// <param name="collider_type">Type of collider to create</param>
    /// <param name="undoString">Undo string to be displayed.</param>
    private void CreateCollider(CREATE_COLLIDER_TYPE collider_type, string undoString)
    {
      Undo.RegisterCompleteObjectUndo(ECEditor.AttachToObject, undoString);
      int group = Undo.GetCurrentGroup();
      Undo.RegisterCompleteObjectUndo(ECEditor, undoString);
      switch (collider_type)
      {
        case CREATE_COLLIDER_TYPE.BOX:
          ECEditor.CreateBoxCollider();
          break;
        case CREATE_COLLIDER_TYPE.ROTATED_BOX:
          ECEditor.CreateBoxCollider(COLLIDER_ORIENTATION.ROTATED);
          break;
        case CREATE_COLLIDER_TYPE.SPHERE:
          ECEditor.CreateSphereCollider(ECEPreferences.SphereColliderMethod);
          break;
        case CREATE_COLLIDER_TYPE.CAPSULE:
          ECEditor.CreateCapsuleCollider(ECEPreferences.CapsuleColliderMethod);
          break;
        case CREATE_COLLIDER_TYPE.ROTATED_CAPSULE:
          ECEditor.CreateCapsuleCollider(ECEPreferences.CapsuleColliderMethod, COLLIDER_ORIENTATION.ROTATED);
          break;
      }
      Undo.CollapseUndoOperations(group);
    }
  }
}
#endif