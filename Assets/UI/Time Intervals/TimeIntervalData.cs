using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using PixelCrushers.DialogueSystem;

[CreateAssetMenu(fileName = "TimeInterval", menuName = "SneakDiary/Time Interval", order = 1)]
public class TimeIntervalData : ScriptableObject
{
    public QuestNames questName;
    [EnumFlags]
    public QuestState questState;
    public string title;
    [TextArea(3, 10)]
    public string description;
    public bool isMajorEvent;
}

/*
[CustomEditor(typeof(TimeIntervalData))]
public class TimeIntervalDataEditor : Editor 
{
    SerializedProperty description;
    
    void OnEnable()
    {
        description = serializedObject.FindProperty("lookAtPoint");
    }

    public override void OnInspectorGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.richText = true;
        serializedObject.Update();
        EditorGUILayout.LabelField("Some <color=yellow>RICH</color> text", style);
        EditorGUILayout.PropertyField(description);
        serializedObject.ApplyModifiedProperties();
    }
}
//*/