// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.DialogueSystem
{

    [CustomEditor(typeof(TextAnimatorSubtitlePanel), true)]
    public class TextAnimatorSubtitlePanelEditor : StandardUISubtitlePanelEditor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("clearTextOnOpen"), true);
            serializedObject.ApplyModifiedProperties();
        }

    }

}
