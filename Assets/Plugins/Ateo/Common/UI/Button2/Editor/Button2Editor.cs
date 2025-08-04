using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(Button2), true), CanEditMultipleObjects]
    public class Button2Editor : ButtonEditor
    {
        private SerializedProperty _buttonClickModeProperty;
        private SerializedProperty _delayProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            _buttonClickModeProperty = serializedObject.FindProperty("_buttonClickMode");
            _delayProperty = serializedObject.FindProperty("_delay");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_buttonClickModeProperty);
            EditorGUILayout.PropertyField(_delayProperty);
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Button", EditorStyles.boldLabel);

            base.OnInspectorGUI();
        }
    }
}