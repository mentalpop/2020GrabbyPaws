using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using System.Collections.Generic;

namespace PixelCrushers.LoveHate
{

    /// <summary>
    /// This is a custom editor for DeedTemplateLibrary.
    /// </summary>
    [CustomEditor(typeof(DeedTemplateLibrary), true)]
    public class DeedTemplateLibraryEditor : Editor
    {

        private DeedTemplateLibrary m_deedTemplateLibrary;
        private ReorderableList m_deedTemplateList;
        private ReorderableList m_deedTemplateFieldList;
        private SerializedProperty m_deedTemplate;

        private void OnEnable()
        {
            m_deedTemplateLibrary = target as DeedTemplateLibrary;
            SetupDeedTemplateList();
        }

        #region Inspector GUI

        public override void OnInspectorGUI()
        {
            //--- For comparison with default inspector:
            //base.OnInspectorGUI();
            //EditorGUILayout.Separator();

            Undo.RecordObject(target, "DeedTemplateLibrary");
            DrawCustomGUI();
        }

        private void DrawCustomGUI()
        {
            serializedObject.Update();
            DrawLooseProperties();
            if (m_deedTemplateLibrary.factionDatabase == null)
            {
                EditorGUILayout.HelpBox("Assign a faction database.", MessageType.None);
            }
            else
            {
                DrawDeedTemplatesSection();
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawLooseProperties()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("factionDatabase"));
        }

        #endregion

        #region Deed Templates

        private void SetupDeedTemplateList()
        {
            m_deedTemplateList = new ReorderableList(
                serializedObject, serializedObject.FindProperty("deedTemplates"),
                true, true, true, true);
            m_deedTemplateList.drawHeaderCallback = OnDrawDeedTemplateListHeader;
            m_deedTemplateList.drawElementCallback = OnDrawDeedTemplateListElement;
            m_deedTemplateList.onAddCallback = OnAddDeedTemplate;
            m_deedTemplateList.onRemoveCallback = OnRemoveDeedTemplate;
            m_deedTemplateList.onSelectCallback = OnSelectDeedTemplate;
            m_deedTemplateFieldList = null;
            m_deedTemplate = null;
        }

        private void DrawDeedTemplatesSection()
        {
            m_deedTemplateList.DoLayoutList();
            if (m_deedTemplateFieldList != null)
            {
                DrawDeedTemplateSection();
            }
            else
            {
                EditorGUILayout.HelpBox("Click the double bars to the left of a template's name to edit its attributes.", MessageType.None);
            }
        }

        private void OnDrawDeedTemplateListHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Deed Templates");
        }

        private void OnDrawDeedTemplateListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = m_deedTemplateList.serializedProperty.GetArrayElementAtIndex(index);
            DrawTagDescriptionListElement(rect, index, isActive, isFocused, element);
        }

        private void DrawTagDescriptionListElement(Rect rect, int index, bool isActive, bool isFocused, SerializedProperty element)
        {
            rect.y += 2;
            var nameWidth = GetDefaultNameWidth(rect);
            var descriptionWidth = rect.width - nameWidth - 4;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, nameWidth, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("tag"), GUIContent.none);
            EditorGUI.PropertyField(
                new Rect(rect.x + rect.width - descriptionWidth, rect.y, descriptionWidth, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("description"), GUIContent.none);
        }

        private float GetDefaultNameWidth(Rect rect)
        {
            return Mathf.Clamp(rect.width / 4, 80, 200);
        }

        private void OnAddDeedTemplate(ReorderableList list)
        {
            var index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            list.index = index;
            SetupDeedTemplateFieldList(list.serializedProperty.GetArrayElementAtIndex(list.index));
        }

        private void OnRemoveDeedTemplate(ReorderableList list)
        {
            var tag = list.serializedProperty.GetArrayElementAtIndex(list.index).FindPropertyRelative("tag").stringValue;
            if (EditorUtility.DisplayDialog("Delete selected deed template?", tag, "Delete", "Cancel"))
            {
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
                m_deedTemplateFieldList = null;
            }
        }

        private void OnSelectDeedTemplate(ReorderableList list)
        {
            if (list != null && 0 <= list.index && list.index < list.serializedProperty.arraySize)
            {
                SetupDeedTemplateFieldList(list.serializedProperty.GetArrayElementAtIndex(list.index));
            }
        }

        #endregion

        #region Deed Template

        private const int NumConcreteFields = 5;

        private void SetupDeedTemplateFieldList(SerializedProperty actionTemplate)
        {
            m_deedTemplate = actionTemplate;
            m_deedTemplate.FindPropertyRelative("traits").arraySize = m_deedTemplateLibrary.factionDatabase.personalityTraitDefinitions.Length;
            var numFields = NumConcreteFields + m_deedTemplateLibrary.factionDatabase.personalityTraitDefinitions.Length;
            m_deedTemplateFieldList = new ReorderableList(
                new bool[numFields], typeof(bool),
                false, true, true, false);
            m_deedTemplateFieldList.drawHeaderCallback = OnDrawDeedTemplateFieldListHeader;
            m_deedTemplateFieldList.drawElementCallback = OnDrawDeedTemplateFieldListElement;
            m_deedTemplateFieldList.onAddDropdownCallback = OnDeedTemplatePresetsDropdown;
        }

        private void DrawDeedTemplateSection()
        {
            m_deedTemplateFieldList.DoLayoutList();
        }

        private void OnDrawDeedTemplateFieldListHeader(Rect rect)
        {
            var actionTag = m_deedTemplateList.serializedProperty.GetArrayElementAtIndex(m_deedTemplateList.index).FindPropertyRelative("tag").stringValue;
            EditorGUI.LabelField(rect, "Deed Template: " + actionTag);
        }

        private void OnDrawDeedTemplateFieldListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            rect.width -= 16;
            rect.x += 16;
            rect.y += 2;
            var nameWidth = GetDefaultNameWidth(rect);
            var valueWidth = rect.width - nameWidth - 4;
            var labelRect = new Rect(rect.x, rect.y, nameWidth, EditorGUIUtility.singleLineHeight);
            var fieldRect = new Rect(rect.x + rect.width - valueWidth, rect.y, valueWidth, EditorGUIUtility.singleLineHeight);
            switch (index)
            {
                case 0:
                    DrawDeedTemplateField(labelRect, "Requires Sight", fieldRect, m_deedTemplate.FindPropertyRelative("requiresSight"), false);
                    break;
                case 1:
                    var leftRect = new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight);
                    var rightRect = new Rect(rect.x + rect.width - rect.width / 2, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight);
                    DrawDeedTemplateField(leftRect, "Radius (0=global)", rightRect, m_deedTemplate.FindPropertyRelative("radius"), false);
                    break;
                case 2:
                    DrawDeedTemplateField(labelRect, "Evaluate By", fieldRect, m_deedTemplate.FindPropertyRelative("permittedEvaluators"), false);
                    break;
                case 3:
                    DrawDeedTemplateField(labelRect, "Impact", fieldRect, m_deedTemplate.FindPropertyRelative("impact"), false);
                    break;
                case 4:
                    DrawDeedTemplateField(labelRect, "Aggression", fieldRect, m_deedTemplate.FindPropertyRelative("aggression"), false);
                    break;
                default:
                    var traitIndex = index - NumConcreteFields;
                    var traitName = m_deedTemplateLibrary.factionDatabase.personalityTraitDefinitions[traitIndex].name;
                    DrawDeedTemplateField(labelRect, traitName, fieldRect, m_deedTemplate.FindPropertyRelative("traits").GetArrayElementAtIndex(traitIndex), true);
                    break;
            }
        }

        private void DrawDeedTemplateField(Rect labelRect, string labelText, Rect fieldRect, SerializedProperty element, bool slider)
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.TextField(labelRect, labelText);
            EditorGUI.EndDisabledGroup();
            if (slider)
            {
                EditorGUI.Slider(fieldRect, element, -100, 100, GUIContent.none);
            }
            else
            {
                EditorGUI.PropertyField(fieldRect, element, GUIContent.none);
            }
        }

        private void OnDeedTemplatePresetsDropdown(Rect rect, ReorderableList list)
        {
            var menu = new GenericMenu();
            for (int i = 0; i < m_deedTemplateLibrary.factionDatabase.presets.Length; i++)
            {
                var preset = m_deedTemplateLibrary.factionDatabase.presets[i];
                menu.AddItem(
                    new GUIContent(preset.name),
                    false, OnSelectDeedTemplatePresetMenuItem, preset);
            }
            menu.ShowAsContext();
        }

        private void OnSelectDeedTemplatePresetMenuItem(object presetObject)
        {
            var preset = presetObject as Preset;
            var actionTemplateValues = m_deedTemplateList.serializedProperty.GetArrayElementAtIndex(m_deedTemplateList.index).FindPropertyRelative("values");
            for (int i = 0; i < preset.traits.Length; i++)
            {
                actionTemplateValues.GetArrayElementAtIndex(i).floatValue = preset.traits[i];
            }
            serializedObject.ApplyModifiedProperties();
        }

        #endregion

    }

}