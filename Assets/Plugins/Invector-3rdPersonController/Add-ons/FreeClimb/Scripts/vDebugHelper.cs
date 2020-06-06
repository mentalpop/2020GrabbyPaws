using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Invector
{
    public static class vDebug
    {
        public static void DrawLine(Vector3 origin, Vector3 destination, float lineWidth = 0.01f)
        {
#if UNITY_EDITOR
            try
            {
                Handles.DrawAAPolyLine(lineWidth, new Vector3[] { origin, destination });
            }
            catch { }
#endif
        }

    }
}
