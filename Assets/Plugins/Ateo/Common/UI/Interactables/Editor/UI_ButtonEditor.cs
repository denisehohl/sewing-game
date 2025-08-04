using UnityEngine;
using Ateo.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(UI_Button), true), CanEditMultipleObjects]
    public class UI_ButtonEditor : SelectableEditor
    {
        SerializedProperty m_InteractableProperty;
        SerializedProperty m_OnClickProperty;
        SerializedProperty m_ButtonClickModeProperty;
        SerializedProperty m_ClickDelayProperty;
        SerializedProperty m_AnimationDurationProperty;
        private SerializedProperty m_DisabledAlphaProperty;

        SerializedProperty m_HighlightedProperty;
        SerializedProperty m_PressedProperty;
        SerializedProperty m_SelectedProperty;

        SerializedProperty m_NavigationProperty;

        UI_Button button;

        protected bool m_DrawSetupGraphics = true;
        protected bool m_DrawOnClickEvent = true;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_InteractableProperty = serializedObject.FindProperty("m_Interactable");
            m_OnClickProperty = serializedObject.FindProperty("m_OnClick");
            m_ButtonClickModeProperty = serializedObject.FindProperty("m_ButtonClickMode");
            m_ClickDelayProperty = serializedObject.FindProperty("m_ClickDelay");
            m_AnimationDurationProperty = serializedObject.FindProperty("m_AnimationDuration");
            m_DisabledAlphaProperty = serializedObject.FindProperty("m_DisabledAlpha");
            m_HighlightedProperty = serializedObject.FindProperty("m_Highlighted");
            m_PressedProperty = serializedObject.FindProperty("m_Pressed");
            m_SelectedProperty = serializedObject.FindProperty("m_Selected");
            m_NavigationProperty = serializedObject.FindProperty("m_Navigation");

            button = target as UI_Button;
        }

        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();
            // EditorGUILayout.Space();

            serializedObject.Update();
            EditorGUILayout.LabelField("State", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_InteractableProperty);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_ButtonClickModeProperty);

            if (button.m_ButtonClickMode == UI_Button.ButtonClickMode.OnDelay)
                EditorGUILayout.PropertyField(m_ClickDelayProperty);

            EditorGUILayout.Space();
            // EditorGUILayout.PropertyField(m_CanvasGroupProperty);
            EditorGUILayout.LabelField("Button", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_HighlightedProperty);
            EditorGUILayout.PropertyField(m_PressedProperty);
            EditorGUILayout.PropertyField(m_SelectedProperty);
            EditorGUILayout.PropertyField(m_AnimationDurationProperty);
            EditorGUILayout.PropertyField(m_DisabledAlphaProperty);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_NavigationProperty);

            m_DrawSetupGraphics = button.m_Highlighted == null || button.m_Pressed == null || button.m_Selected == null;
            
            if (m_DrawSetupGraphics)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Setup Graphics", GUILayout.MaxHeight(20)))
                {
                    button.SetupGraphics();
                    serializedObject.ApplyModifiedProperties();
                    OnInspectorGUI();
                    return;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (m_DrawOnClickEvent)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Event", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(m_OnClickProperty);
            }

            if (GUI.changed)
            {
                // EditorUtility.SetDirty(target);  
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}