using UnityEngine;
using UnityEditor;

namespace Ateo.ViewManagement
{
    [CustomEditor(typeof(UIManager), true)]
    public class UIManagerEditor : Editor
    {
        private SerializedProperty m_OnlyTopHierarchyProperty;
        private SerializedProperty m_HideAllOnEnableProperty;
        private SerializedProperty m_HideManagerOnEnableProperty;
        private SerializedProperty m_EnableAnimationProperty;
        private SerializedProperty m_DefaultViewProperty;

        private UIManager manager;

        private void OnEnable()
        {
            manager = target as UIManager;
            m_OnlyTopHierarchyProperty = serializedObject.FindProperty("m_OnlyTopHierarchy");
            m_HideAllOnEnableProperty = serializedObject.FindProperty("m_HideAllOnEnable");
            m_HideManagerOnEnableProperty = serializedObject.FindProperty("m_HideManagerOnEnable");
            m_EnableAnimationProperty = serializedObject.FindProperty("m_EnableAnimation");
            m_DefaultViewProperty = serializedObject.FindProperty("m_DefaultView");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Properties", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_OnlyTopHierarchyProperty);
            EditorGUILayout.PropertyField(m_HideAllOnEnableProperty);
            EditorGUILayout.PropertyField(m_HideManagerOnEnableProperty);
            EditorGUILayout.PropertyField(m_EnableAnimationProperty);
            EditorGUILayout.PropertyField(m_DefaultViewProperty);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Editor Controlls", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Update Views", GUILayout.MaxHeight(20)))
            {
                manager.GetViewsAndElements();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Show ViewManager", GUILayout.MaxHeight(20)))
            {
                manager.ShowViewManager();
            }

            if (GUILayout.Button("Hide ViewManager", GUILayout.MaxHeight(20)))
            {
                manager.HideViewManager();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Show all views", GUILayout.MaxHeight(20)))
            {
                manager.ShowAllViews();
            }

            if (GUILayout.Button("Hide all views", GUILayout.MaxHeight(20)))
            {
                manager.HideAllViews();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            foreach (var v in manager.Views)
            {
                if (v != null)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField(v.name, GUILayout.MaxWidth(120));

                    if (GUILayout.Button("Show", GUILayout.MaxHeight(20)))
                    {
                        manager.ShowView(v.name, Application.isPlaying);
                    }

                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    DebugDev.LogWarning("ViewManager: Null Reference - Updating Views...");
                    manager.GetViewsAndElements();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}