#if UNITY_EDITOR

using Ateo.Common.UI.UnityEngine.UI.Extensions;
using UnityEditor;
using UnityEditor.UI;

namespace Ateo.Common.UI.Editor
{
    [CustomEditor(typeof(ScrollDeltaScrollRect))]
    public class ScrollDeltaScrollRectEditor : ScrollRectEditor
    {
        SerializedProperty _scrollSpeedMouse;
        SerializedProperty _scrollSpeedTrackpad;

        protected override void OnEnable()
        {
            base.OnEnable();

            _scrollSpeedMouse = serializedObject.FindProperty("_scrollSpeedMouse");
            _scrollSpeedTrackpad = serializedObject.FindProperty("_scrollSpeedTrackpad");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_scrollSpeedMouse);
            EditorGUILayout.PropertyField(_scrollSpeedTrackpad);
            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}

#endif