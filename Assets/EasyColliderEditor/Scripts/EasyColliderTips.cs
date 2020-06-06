#if (UNITY_EDITOR)
namespace ECE
{
  public static class EasyColliderTips
  {
    public const string IN_PLAY_MODE = "Vertex selection is not enabled when in play mode.";
    public const string WRONG_FOCUSED_WINDOW = "Vertex selection only works when the scene view window is focused.";
    public const string FORCED_FOCUSED_WINDOW = "Forced window focus is enabled, you may not be able to edit values easily by typing. Disable vertex selection, or the force focus scene option in preferences.";
    public const string EDIT_PREFS_FORCED_FOCUSED = "You are editing preferences with forced window focus enabled. To make this easier, try disabling vertex selection, or the force focus scene option in preferences.";
    public const string NO_MESH_FILTER_FOUND = "No mesh filter is on the selected gameobject, try enabling include child meshes.";
    public const string CHECK_DOCUMENTATION_REMINDER = "Need help? Be sure to check out the included documentation.";
    public const string COMPUTE_SHADER_TIP = "You're system's shader model does not support shaders with a compute buffer. Be sure to use the gizmo display method instead of shader in the preferences.";
  }
}
#endif