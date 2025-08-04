using UnityEngine;
using Ateo.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(UI_Toggle), true)]
    [CanEditMultipleObjects]
    public class UI_ToggleEditor : ToggleEditor
    {
        SerializedProperty m_InteractableProperty;
        SerializedProperty m_IsOnProperty;
        // SerializedProperty m_AnimationModeProperty;
        SerializedProperty m_AnimationDurationProperty;

        // SerializedProperty m_CanvasGroupProperty;
	    SerializedProperty m_HighlightedProperty;
	    SerializedProperty m_PressedProperty;
        SerializedProperty m_ToggleOnProperty;
        SerializedProperty m_ToggleOffProperty;
        SerializedProperty m_CrossFadeTimeProperty;

        SerializedProperty m_ToogleTextProperty;
        SerializedProperty m_TextOnProperty;
        SerializedProperty m_TextOffProperty;

        SerializedProperty m_OnValueChangedProperty;

        UI_Toggle toggle;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_InteractableProperty = serializedObject.FindProperty("m_Interactable");
            m_IsOnProperty = serializedObject.FindProperty("m_IsOn");
            // m_AnimationModeProperty = serializedObject.FindProperty("m_AnimationMode");
            m_AnimationDurationProperty = serializedObject.FindProperty("m_AnimationDuration");
            // m_CanvasGroupProperty = serializedObject.FindProperty("m_CanvasGroup");
            m_HighlightedProperty = serializedObject.FindProperty("m_Highlighted");
            m_PressedProperty = serializedObject.FindProperty("m_Pressed");
            m_ToggleOnProperty = serializedObject.FindProperty("m_ToggleOn");
            m_ToggleOffProperty = serializedObject.FindProperty("m_ToggleOff");
            m_CrossFadeTimeProperty = serializedObject.FindProperty("m_CrossFadeTime");
            m_ToogleTextProperty = serializedObject.FindProperty("m_ToogleText");
            m_TextOnProperty = serializedObject.FindProperty("m_TextOn");
            m_TextOffProperty = serializedObject.FindProperty("m_TextOff");
            m_OnValueChangedProperty = serializedObject.FindProperty("onValueChanged");

            toggle = target as UI_Toggle;
        }

        public override void OnInspectorGUI()
        {
            // base.OnInspectorGUI();
            // EditorGUILayout.Space();

            serializedObject.Update();

            EditorGUILayout.LabelField("State", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_InteractableProperty);
            EditorGUILayout.PropertyField(m_IsOnProperty);
            EditorGUILayout.Space();

            // EditorGUILayout.PropertyField(m_AnimationModeProperty);
            // EditorGUILayout.PropertyField(m_CanvasGroupProperty);

            EditorGUILayout.LabelField("Button", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_HighlightedProperty);
            EditorGUILayout.PropertyField(m_PressedProperty);
            EditorGUILayout.PropertyField(m_AnimationDurationProperty);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Toggle", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_ToggleOnProperty);
            EditorGUILayout.PropertyField(m_ToggleOffProperty);
            EditorGUILayout.PropertyField(m_CrossFadeTimeProperty);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Text", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_ToogleTextProperty);
            EditorGUILayout.PropertyField(m_TextOnProperty);
            EditorGUILayout.PropertyField(m_TextOffProperty);
            EditorGUILayout.Space();

/*             EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Setup Graphics", GUILayout.MaxHeight(20)))
            {
                toggle.SetupGraphics();
                serializedObject.ApplyModifiedProperties();
                OnInspectorGUI();
                return;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(); */
            EditorGUILayout.LabelField("Event", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(m_OnValueChangedProperty);

            if (GUI.changed)
            {
                // EditorUtility.SetDirty(target);  
                serializedObject.ApplyModifiedProperties ();
            }
        }
    }
}
