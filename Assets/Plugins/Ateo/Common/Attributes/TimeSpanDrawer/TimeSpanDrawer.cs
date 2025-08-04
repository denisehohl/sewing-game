#if UNITY_EDITOR

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using System;
    using Utilities.Editor;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// TimeSpan property drawer.
    /// </summary>
    public sealed class TimeSpanDrawer : OdinValueDrawer<TimeSpan>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            GUILayout.BeginHorizontal();
            {
                if (label != null)
                {
                    //Renders this field's prefix label.
                    //Note that since this uses EditorGUI, we need to add some GUILayout.Space to take up the Rect space in the layouting system.
                    EditorGUI.PrefixLabel(GUIHelper.GetCurrentLayoutRect(), label);
                    
                    //Subtract 8 from the space, to line up with the rest of the inspector control column.
                    //Without this, label text will be indented a bit too much.
                    GUILayout.Space(EditorGUIUtility.labelWidth - 8);
                }

                //Make sure all of our labels have a static width of 42 pixels, as that's all we'll need.
                GUIHelper.PushLabelWidth(42f);
                
                //We don't want any indent, so make sure we don't use any.
                GUIHelper.PushIndentLevel(0);
                
                //We'll stack our IntFields into two rows, so start a vertical layout here.
                GUILayout.BeginVertical();
                {
                    int days, hours, minutes, seconds;
                    
                    EditorGUI.BeginChangeCheck();
                    {
                        GUILayout.BeginHorizontal();

                        {
                            days = SirenixEditorFields.IntField(GUIHelper.TempContent("Days"), ValueEntry.SmartValue.Days);
                            hours = SirenixEditorFields.IntField(GUIHelper.TempContent("Hours"), ValueEntry.SmartValue.Hours);
                        }

                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();

                        {
                            minutes = SirenixEditorFields.IntField(GUIHelper.TempContent("Mins"), ValueEntry.SmartValue.Minutes);
                            seconds = SirenixEditorFields.IntField(GUIHelper.TempContent("Secs"), ValueEntry.SmartValue.Seconds);
                        }

                        GUILayout.EndHorizontal();
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        //If EditorGUI detected any changes, we'll set our SmartValue to a TimeSpan containing our updated values.
                        ValueEntry.SmartValue = new TimeSpan(days, hours, minutes, seconds);
                    }
                }

                GUILayout.EndVertical();
                
                //Clean up and pop our indent and label width.
                GUIHelper.PopIndentLevel();
                GUIHelper.PopLabelWidth();
            }

            GUILayout.EndHorizontal();
        }
    }
}
#endif