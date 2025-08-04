using UnityEngine;
using Ateo.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(UI_Slider), true), CanEditMultipleObjects]
    public class UI_SliderEditor : SliderEditor
    {
        SerializedProperty m_OnValueStartProperty;
        SerializedProperty m_OnValueEndProperty;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            m_OnValueStartProperty = serializedObject.FindProperty("m_OnValueStart");
            m_OnValueEndProperty = serializedObject.FindProperty("m_OnValueEnd");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_OnValueStartProperty);
            EditorGUILayout.PropertyField(m_OnValueEndProperty);
            
            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}