using System;

#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using UnityEngine;
    using UnityEditor;

    // Draw for tag field attributes on string members
    public class TagSelectorAttributeDrawer : OdinAttributeDrawer<TagSelectorAttribute, string>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            ValueEntry.SmartValue = label != null
                ? EditorGUILayout.TagField(label, ValueEntry.SmartValue)
                : EditorGUILayout.TagField(ValueEntry.SmartValue);
        }
    }
}
#endif