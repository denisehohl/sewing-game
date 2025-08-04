// This is a rip-off of the original file:

// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

// where we patched the DrawEventListener function to handle our EnumActionAttributes
// The idea comes from: https://forum.unity.com/threads/ability-to-add-enum-argument-to-button-functions.270817/
// Patch author: llamagod

// It works thanks to reflection and the usage of properties instead of trying to access private/internal members.
// However, it is risky: you need to update the file with every iteration of Unity that will modify this file.
// Follow: https://github.com/Unity-Technologies/UnityCsReference/blob/2018.3/Editor/Mono/Inspector/UnityEventDrawer.cs
// for changes.

// Check the discussion on the Unity forum to see if a more flexible solution appears, such as creating a type
// supported by Unity events, but displaying as an enum in the PropertyField.

// See usage in UnityEventDrawer.cs


#if UNITY_2020_2_OR_NEWER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEditorInternal;

[CustomPropertyDrawer(typeof(UnityEvent), true)]
public class UnityEventDrawer : PropertyDrawer
{
    private Dictionary<string, State> m_States = new Dictionary<string, State>();

    // Find internal methods with reflection
    private static MethodInfo findMethod = typeof(UnityEventBase).GetMethod("FindMethod", BindingFlags.NonPublic | BindingFlags.Instance, null,
        CallingConventions.Standard, new Type[] {typeof(string), typeof(Type), typeof(PersistentListenerMode), typeof(Type)}, null);

    private static MethodInfo temp = typeof(GUIContent).GetMethod("Temp", BindingFlags.NonPublic | BindingFlags.Static, null,
        CallingConventions.Standard, new Type[] {typeof(string)}, null);

    private static PropertyInfo mixedValueContent = typeof(EditorGUI).GetProperty("mixedValueContent", BindingFlags.NonPublic | BindingFlags.Static);
    private Styles m_Styles;
    private string m_Text;
    private UnityEventBase m_DummyEvent;
    private SerializedProperty m_Prop;
    private SerializedProperty m_ListenersArray;
    private ReorderableList m_ReorderableList;
    private int m_LastSelectedIndex;

    private static string GetEventParams(UnityEventBase evt)
    {
        var method = (MethodInfo) findMethod.Invoke(evt, new object[] {"Invoke", evt.GetType(), PersistentListenerMode.EventDefined, null});
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(" (");
        var array = ((IEnumerable<ParameterInfo>) method.GetParameters()).Select(x => x.ParameterType).ToArray();
        for (int index = 0; index < array.Length; ++index)
        {
            stringBuilder.Append(array[index].Name);
            if (index < array.Length - 1)
                stringBuilder.Append(", ");
        }

        stringBuilder.Append(")");
        return stringBuilder.ToString();
    }

    private State GetState(SerializedProperty prop)
    {
        string propertyPath = prop.propertyPath;
        State state;
        m_States.TryGetValue(propertyPath, out state);
        if (state == null)
        {
            state = new State();
            var propertyRelative = prop.FindPropertyRelative("m_PersistentCalls.m_Calls");
            state.m_ReorderableList = new ReorderableList(prop.serializedObject, propertyRelative, false, true, true, true);
            state.m_ReorderableList.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(DrawEventHeader);
            state.m_ReorderableList.drawElementCallback = new ReorderableList.ElementCallbackDelegate(DrawEventListener);
            state.m_ReorderableList.onSelectCallback = new ReorderableList.SelectCallbackDelegate(SelectEventListener);
            state.m_ReorderableList.onReorderCallback = new ReorderableList.ReorderCallbackDelegate(EndDragChild);
            state.m_ReorderableList.onAddCallback = new ReorderableList.AddCallbackDelegate(AddEventListener);
            state.m_ReorderableList.onRemoveCallback = new ReorderableList.RemoveCallbackDelegate(RemoveButton);
            state.m_ReorderableList.elementHeight = 43f;
            m_States[propertyPath] = state;
        }

        return state;
    }

    private State RestoreState(SerializedProperty property)
    {
        var state = GetState(property);
        m_ListenersArray = state.m_ReorderableList.serializedProperty;
        m_ReorderableList = state.m_ReorderableList;
        m_LastSelectedIndex = state.lastSelectedIndex;
        m_ReorderableList.index = m_LastSelectedIndex;
        return state;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        m_Prop = property;
        m_Text = label.text;
        var state = RestoreState(property);
        OnGUI(position);
        state.lastSelectedIndex = m_LastSelectedIndex;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        RestoreState(property);
        float num = 0.0f;
        if (m_ReorderableList != null)
            num = m_ReorderableList.GetHeight();
        return num;
    }

    public void OnGUI(Rect position)
    {
        if (m_ListenersArray == null || !m_ListenersArray.isArray)
            return;
        m_DummyEvent = GetDummyEvent(m_Prop);
        if (m_DummyEvent == null)
            return;
        if (m_Styles == null)
            m_Styles = new Styles();
        if (m_ReorderableList == null)
            return;
        int indentLevel = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        m_ReorderableList.DoList(position);
        EditorGUI.indentLevel = indentLevel;
    }

    protected virtual void DrawEventHeader(Rect headerRect)
    {
        headerRect.height = 16f;
        string text = (!string.IsNullOrEmpty(m_Text) ? m_Text : "Event") + GetEventParams(m_DummyEvent);
        GUI.Label(headerRect, text);
    }

    private static PersistentListenerMode GetMode(SerializedProperty mode)
    {
        return (PersistentListenerMode) mode.enumValueIndex;
    }

    private void DrawEventListener(Rect rect, int index, bool isactive, bool isfocused)
    {
        var arrayElementAtIndex = m_ListenersArray.GetArrayElementAtIndex(index);
        ++rect.y;
        var rowRects = GetRowRects(rect);
        var position1 = rowRects[0];
        var position2 = rowRects[1];
        var rect1 = rowRects[2];
        var position3 = rowRects[3];
        var propertyRelative1 = arrayElementAtIndex.FindPropertyRelative("m_CallState");
        var propertyRelative2 = arrayElementAtIndex.FindPropertyRelative("m_Mode");
        var propertyRelative3 = arrayElementAtIndex.FindPropertyRelative("m_Arguments");
        var propertyRelative4 = arrayElementAtIndex.FindPropertyRelative("m_Target");
        var propertyRelative5 = arrayElementAtIndex.FindPropertyRelative("m_MethodName");
        var backgroundColor = GUI.backgroundColor;
        GUI.backgroundColor = Color.white;
        EditorGUI.PropertyField(position1, propertyRelative1, GUIContent.none);
        EditorGUI.BeginChangeCheck();
        GUI.Box(position2, GUIContent.none);
        EditorGUI.PropertyField(position2, propertyRelative4, GUIContent.none);
        if (EditorGUI.EndChangeCheck())
            propertyRelative5.stringValue = null;
        var persistentListenerMode = GetMode(propertyRelative2);
        if (propertyRelative4.objectReferenceValue == null || string.IsNullOrEmpty(propertyRelative5.stringValue))
            persistentListenerMode = PersistentListenerMode.Void;
        SerializedProperty propertyRelative6;
        switch (persistentListenerMode)
        {
            case PersistentListenerMode.Object:
                propertyRelative6 = propertyRelative3.FindPropertyRelative("m_ObjectArgument");
                break;
            case PersistentListenerMode.Int:
                propertyRelative6 = propertyRelative3.FindPropertyRelative("m_IntArgument");
                break;
            case PersistentListenerMode.Float:
                propertyRelative6 = propertyRelative3.FindPropertyRelative("m_FloatArgument");
                break;
            case PersistentListenerMode.String:
                propertyRelative6 = propertyRelative3.FindPropertyRelative("m_StringArgument");
                break;
            case PersistentListenerMode.Bool:
                propertyRelative6 = propertyRelative3.FindPropertyRelative("m_BoolArgument");
                break;
            default:
                propertyRelative6 = propertyRelative3.FindPropertyRelative("m_IntArgument");
                break;
        }

        string stringValue = propertyRelative3.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName").stringValue;
        var type = typeof(UnityEngine.Object);
        if (!string.IsNullOrEmpty(stringValue))
            type = Type.GetType(stringValue, false) ?? typeof(UnityEngine.Object);
        if (persistentListenerMode == PersistentListenerMode.Object)
        {
            EditorGUI.BeginChangeCheck();
            var @object = EditorGUI.ObjectField(position3, GUIContent.none, propertyRelative6.objectReferenceValue, type, true);
            if (EditorGUI.EndChangeCheck())
                propertyRelative6.objectReferenceValue = @object;
        }
        else if (persistentListenerMode != PersistentListenerMode.Void && persistentListenerMode != PersistentListenerMode.EventDefined &&
                 !propertyRelative6.serializedObject.isEditingMultipleObjects)
        {
            // Try to find Find the EnumActionAttribute
            var method = GetMethod(m_DummyEvent, propertyRelative5.stringValue, propertyRelative4.objectReferenceValue, GetMode(propertyRelative2),
                type);
            object[] attributes = null;
            if (method != null)
                attributes = method.GetCustomAttributes(typeof(EnumActionAttribute), true);
            if (attributes != null && attributes.Length > 0)
            {
                // Make an enum popup
                var enumType = ((EnumActionAttribute) attributes[0]).enumType;
                var value = (Enum) Enum.ToObject(enumType, propertyRelative6.intValue);
                propertyRelative6.intValue = Convert.ToInt32(EditorGUI.EnumPopup(position3, value));
            }
            else
                EditorGUI.PropertyField(position3, propertyRelative6, GUIContent.none);
        }

        EditorGUI.BeginDisabledGroup(propertyRelative4.objectReferenceValue == null);
        EditorGUI.BeginProperty(rect1, GUIContent.none, propertyRelative5);
        GUIContent content;
        if (EditorGUI.showMixedValue)
        {
            content = (GUIContent) mixedValueContent.GetValue(null, null);
        }
        else
        {
            var stringBuilder = new StringBuilder();
            if (propertyRelative4.objectReferenceValue == null || string.IsNullOrEmpty(propertyRelative5.stringValue))
                stringBuilder.Append("No Function");
            else if (!IsPersistantListenerValid(m_DummyEvent, propertyRelative5.stringValue, propertyRelative4.objectReferenceValue,
                GetMode(propertyRelative2), type))
            {
                var str = "UnknownComponent";
                UnityEngine.Object objectReferenceValue = propertyRelative4.objectReferenceValue;
                if (objectReferenceValue != null)
                    str = objectReferenceValue.GetType().Name;
                stringBuilder.Append(string.Format("<Missing {0}.{1}>", str, propertyRelative5.stringValue));
            }
            else
            {
                stringBuilder.Append(propertyRelative4.objectReferenceValue.GetType().Name);
                if (!string.IsNullOrEmpty(propertyRelative5.stringValue))
                {
                    stringBuilder.Append(".");
                    if (propertyRelative5.stringValue.StartsWith("set_"))
                        stringBuilder.Append(propertyRelative5.stringValue.Substring(4));
                    else
                        stringBuilder.Append(propertyRelative5.stringValue);
                }
            }

            content = (GUIContent) temp.Invoke(null, new object[] {stringBuilder.ToString()});
        }

        if (GUI.Button(rect1, content, EditorStyles.popup))
            BuildPopupList(propertyRelative4.objectReferenceValue, m_DummyEvent, arrayElementAtIndex).DropDown(rect1);
        EditorGUI.EndProperty();
        EditorGUI.EndDisabledGroup();
        GUI.backgroundColor = backgroundColor;
    }

    private Rect[] GetRowRects(Rect rect)
    {
        var rectArray = new Rect[4];
        rect.height = 16f;
        rect.y += 2f;
        var rect1 = rect;
        rect1.width *= 0.3f;
        var rect2 = rect1;
        rect2.y += EditorGUIUtility.singleLineHeight + 2f;
        var rect3 = rect;
        rect3.xMin = rect2.xMax + 5f;
        var rect4 = rect3;
        rect4.y += EditorGUIUtility.singleLineHeight + 2f;
        rectArray[0] = rect1;
        rectArray[1] = rect2;
        rectArray[2] = rect3;
        rectArray[3] = rect4;
        return rectArray;
    }

    private void RemoveButton(ReorderableList list)
    {
        ReorderableList.defaultBehaviours.DoRemoveButton(list);
        m_LastSelectedIndex = list.index;
    }

    private void AddEventListener(ReorderableList list)
    {
        if (m_ListenersArray.hasMultipleDifferentValues)
        {
            foreach (UnityEngine.Object targetObject in m_ListenersArray.serializedObject.targetObjects)
            {
                var serializedObject = new SerializedObject(targetObject);
                ++serializedObject.FindProperty(m_ListenersArray.propertyPath).arraySize;
                serializedObject.ApplyModifiedProperties();
            }

            m_ListenersArray.serializedObject.SetIsDifferentCacheDirty();
            m_ListenersArray.serializedObject.Update();
            list.index = list.serializedProperty.arraySize - 1;
        }
        else
            ReorderableList.defaultBehaviours.DoAddButton(list);

        m_LastSelectedIndex = list.index;
        var arrayElementAtIndex = m_ListenersArray.GetArrayElementAtIndex(list.index);
        var propertyRelative1 = arrayElementAtIndex.FindPropertyRelative("m_CallState");
        var propertyRelative2 = arrayElementAtIndex.FindPropertyRelative("m_Target");
        var propertyRelative3 = arrayElementAtIndex.FindPropertyRelative("m_MethodName");
        var propertyRelative4 = arrayElementAtIndex.FindPropertyRelative("m_Mode");
        var propertyRelative5 = arrayElementAtIndex.FindPropertyRelative("m_Arguments");
        propertyRelative1.enumValueIndex = 2;
        propertyRelative2.objectReferenceValue = null;
        propertyRelative3.stringValue = null;
        propertyRelative4.enumValueIndex = 1;
        propertyRelative5.FindPropertyRelative("m_FloatArgument").floatValue = 0.0f;
        propertyRelative5.FindPropertyRelative("m_IntArgument").intValue = 0;
        propertyRelative5.FindPropertyRelative("m_ObjectArgument").objectReferenceValue = null;
        propertyRelative5.FindPropertyRelative("m_StringArgument").stringValue = null;
        propertyRelative5.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName").stringValue = null;
    }

    private void SelectEventListener(ReorderableList list)
    {
        m_LastSelectedIndex = list.index;
    }

    private void EndDragChild(ReorderableList list)
    {
        m_LastSelectedIndex = list.index;
    }

    private static UnityEventBase GetDummyEvent(SerializedProperty prop)
    {
        return new UnityEvent();

        #region Original Code

#pragma warning disable CS0162
        var type = Type.GetType(prop.FindPropertyRelative("m_TypeName").stringValue, false);
        if (type == null)
            return new UnityEvent();
        return Activator.CreateInstance(type) as UnityEventBase;
#pragma warning restore CS0162

        #endregion
    }

    private static IEnumerable<ValidMethodMap> CalculateMethodMap(UnityEngine.Object target, Type[] t, bool allowSubclasses)
    {
        var validMethodMapList = new List<ValidMethodMap>();
        if (target == null || t == null)
            return validMethodMapList;
        var type = target.GetType();
        var list = ((IEnumerable<MethodInfo>) type.GetMethods()).Where(x => !x.IsSpecialName).ToList();
        var source = ((IEnumerable<PropertyInfo>) type.GetProperties()).AsEnumerable().Where(x =>
        {
            if (x.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length == 0)
                return x.GetSetMethod() != null;
            return false;
        });
        list.AddRange(source.Select(x => x.GetSetMethod()));
        using (List<MethodInfo>.Enumerator enumerator = list.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                var parameters = current.GetParameters();
                if (parameters.Length == t.Length && current.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length <= 0 &&
                    current.ReturnType == typeof(void))
                {
                    bool flag = true;
                    for (int index = 0; index < t.Length; ++index)
                    {
                        if (!parameters[index].ParameterType.IsAssignableFrom(t[index]))
                            flag = false;
                        if (allowSubclasses && t[index].IsAssignableFrom(parameters[index].ParameterType))
                            flag = true;
                    }

                    if (flag)
                        validMethodMapList.Add(new ValidMethodMap()
                        {
                            target = target,
                            methodInfo = current
                        });
                }
            }
        }

        return validMethodMapList;
    }

    public static bool IsPersistantListenerValid(UnityEventBase dummyEvent, string methodName, UnityEngine.Object uObject,
        PersistentListenerMode modeEnum, Type argumentType)
    {
        if (uObject == null || string.IsNullOrEmpty(methodName))
            return false;
        return GetMethod(dummyEvent, methodName, uObject, modeEnum, argumentType) != null;
    }

    private static MethodInfo GetMethod(UnityEventBase dummyEvent, string methodName, UnityEngine.Object uObject, PersistentListenerMode modeEnum,
        Type argumentType)
    {
        return (MethodInfo) findMethod.Invoke(dummyEvent, new object[] {methodName, uObject.GetType(), modeEnum, argumentType});
    }

    private static GenericMenu BuildPopupList(UnityEngine.Object target, UnityEventBase dummyEvent, SerializedProperty listener)
    {
        var target1 = target;
        if (target1 is Component)
            target1 = (target as Component).gameObject;
        var propertyRelative = listener.FindPropertyRelative("m_MethodName");
        var menu = new GenericMenu();
        menu.AddItem(new GUIContent("No Function"), string.IsNullOrEmpty(propertyRelative.stringValue),
            new GenericMenu.MenuFunction2(ClearEventFunction), new UnityEventFunction(listener, null, null, PersistentListenerMode.EventDefined));
        if (target1 == null)
            return menu;
        menu.AddSeparator(string.Empty);
        var array = ((IEnumerable<ParameterInfo>) dummyEvent.GetType().GetMethod("Invoke").GetParameters()).Select(x => x.ParameterType).ToArray();
        GeneratePopUpForType(menu, target1, false, listener, array);
        if (target1 is GameObject)
        {
            var components = (target1 as GameObject).GetComponents<Component>();
            var list = ((IEnumerable<Component>) components).Where(c => c != null).Select(c => c.GetType().Name).GroupBy(x => x)
                .Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            foreach (var component in components)
            {
                if (!(component == null))
                    GeneratePopUpForType(menu, component, list.Contains(component.GetType().Name), listener, array);
            }
        }

        return menu;
    }

    private static void GeneratePopUpForType(GenericMenu menu, UnityEngine.Object target, bool useFullTargetName, SerializedProperty listener,
        Type[] delegateArgumentsTypes)
    {
        var methods = new List<ValidMethodMap>();
        string targetName = !useFullTargetName ? target.GetType().Name : target.GetType().FullName;
        bool flag = false;
        if (delegateArgumentsTypes.Length != 0)
        {
            GetMethodsForTargetAndMode(target, delegateArgumentsTypes, methods, PersistentListenerMode.EventDefined);
            if (methods.Count > 0)
            {
                menu.AddDisabledItem(new GUIContent(targetName + "/Dynamic " +
                                                    string.Join(", ",
                                                        ((IEnumerable<Type>) delegateArgumentsTypes).Select(e => GetTypeName(e)).ToArray())));
                AddMethodsToMenu(menu, listener, methods, targetName);
                flag = true;
            }
        }

        methods.Clear();
        GetMethodsForTargetAndMode(target, new Type[1] {typeof(float)}, methods, PersistentListenerMode.Float);
        GetMethodsForTargetAndMode(target, new Type[1] {typeof(int)}, methods, PersistentListenerMode.Int);
        GetMethodsForTargetAndMode(target, new Type[1] {typeof(string)}, methods, PersistentListenerMode.String);
        GetMethodsForTargetAndMode(target, new Type[1] {typeof(bool)}, methods, PersistentListenerMode.Bool);
        GetMethodsForTargetAndMode(target, new Type[1] {typeof(UnityEngine.Object)}, methods, PersistentListenerMode.Object, true);
        GetMethodsForTargetAndMode(target, new Type[0], methods, PersistentListenerMode.Void);
        if (methods.Count <= 0)
            return;
        if (flag)
            menu.AddItem(new GUIContent(targetName + "/ "), false, null);
        if (delegateArgumentsTypes.Length != 0)
            menu.AddDisabledItem(new GUIContent(targetName + "/Static Parameters"));
        AddMethodsToMenu(menu, listener, methods, targetName);
    }

    private static void AddMethodsToMenu(GenericMenu menu, SerializedProperty listener, List<ValidMethodMap> methods, string targetName)
    {
        foreach (var method in methods.OrderBy(e => e.methodInfo.Name.StartsWith("set_") ? 0 : 1).ThenBy(e => e.methodInfo.Name))
            AddFunctionsForScript(menu, listener, method, targetName);
    }

    private static void GetMethodsForTargetAndMode(UnityEngine.Object target, Type[] delegateArgumentsTypes, List<ValidMethodMap> methods,
        PersistentListenerMode mode, bool allowSubclasses = false)
    {
        var methodMaps = CalculateMethodMap(target, delegateArgumentsTypes, allowSubclasses).ToArray();
        for (int i = 0; i < methodMaps.Length; i++)
        {
            methodMaps[i].mode = mode;
            methods.Add(methodMaps[i]);
        }
    }

    private static void AddFunctionsForScript(GenericMenu menu, SerializedProperty listener, ValidMethodMap method, string targetName)
    {
        var mode1 = method.mode;
        var objectReferenceValue = listener.FindPropertyRelative("m_Target").objectReferenceValue;
        var stringValue = listener.FindPropertyRelative("m_MethodName").stringValue;
        var mode2 = GetMode(listener.FindPropertyRelative("m_Mode"));
        var propertyRelative = listener.FindPropertyRelative("m_Arguments").FindPropertyRelative("m_ObjectArgumentAssemblyTypeName");
        var stringBuilder = new StringBuilder();
        int length = method.methodInfo.GetParameters().Length;
        for (int index = 0; index < length; ++index)
        {
            var parameter = method.methodInfo.GetParameters()[index];
            stringBuilder.Append(string.Format("{0}", GetTypeName(parameter.ParameterType)));
            if (index < length - 1)
                stringBuilder.Append(", ");
        }

        var on = objectReferenceValue == method.target && stringValue == method.methodInfo.Name && mode1 == mode2;
        if (on && mode1 == PersistentListenerMode.Object && method.methodInfo.GetParameters().Length == 1)
            on &= method.methodInfo.GetParameters()[0].ParameterType.AssemblyQualifiedName == propertyRelative.stringValue;
        var formattedMethodName = GetFormattedMethodName(targetName, method.methodInfo.Name, stringBuilder.ToString(),
            mode1 == PersistentListenerMode.EventDefined);
        menu.AddItem(new GUIContent(formattedMethodName), on, new GenericMenu.MenuFunction2(SetEventFunction),
            new UnityEventFunction(listener, method.target, method.methodInfo, mode1));
    }

    private static string GetTypeName(Type t)
    {
        if (t == typeof(int))
            return "int";
        if (t == typeof(float))
            return "float";
        if (t == typeof(string))
            return "string";
        if (t == typeof(bool))
            return "bool";
        return t.Name;
    }

    private static string GetFormattedMethodName(string targetName, string methodName, string args, bool dynamic)
    {
        if (dynamic)
        {
            if (methodName.StartsWith("set_"))
                return string.Format("{0}/{1}", targetName, methodName.Substring(4));
            return string.Format("{0}/{1}", targetName, methodName);
        }

        if (methodName.StartsWith("set_"))
            return string.Format("{0}/{2} {1}", targetName, methodName.Substring(4), args);
        return string.Format("{0}/{1} ({2})", targetName, methodName, args);
    }

    private static void SetEventFunction(object source)
    {
        ((UnityEventFunction) source).Assign();
    }

    private static void ClearEventFunction(object source)
    {
        ((UnityEventFunction) source).Clear();
    }

    protected class State
    {
        internal ReorderableList m_ReorderableList;
        public int lastSelectedIndex;
    }

    private class Styles
    {
        public readonly GUIContent iconToolbarMinus = EditorGUIUtility.IconContent("Toolbar Minus");
        public readonly GUIStyle genericFieldStyle = EditorStyles.label;
        public readonly GUIStyle removeButton = "InvisibleButton";
    }

    private struct ValidMethodMap
    {
        public UnityEngine.Object target;
        public MethodInfo methodInfo;
        public PersistentListenerMode mode;
    }

    private struct UnityEventFunction
    {
        private readonly SerializedProperty m_Listener;
        private readonly UnityEngine.Object m_Target;
        private readonly MethodInfo m_Method;
        private readonly PersistentListenerMode m_Mode;

        public UnityEventFunction(SerializedProperty listener, UnityEngine.Object target, MethodInfo method, PersistentListenerMode mode)
        {
            m_Listener = listener;
            m_Target = target;
            m_Method = method;
            m_Mode = mode;
        }

        public void Assign()
        {
            var propertyRelative1 = m_Listener.FindPropertyRelative("m_Target");
            var propertyRelative2 = m_Listener.FindPropertyRelative("m_MethodName");
            var propertyRelative3 = m_Listener.FindPropertyRelative("m_Mode");
            var propertyRelative4 = m_Listener.FindPropertyRelative("m_Arguments");
            propertyRelative1.objectReferenceValue = m_Target;
            propertyRelative2.stringValue = m_Method.Name;
            propertyRelative3.enumValueIndex = (int) m_Mode;
            if (m_Mode == PersistentListenerMode.Object)
            {
                var propertyRelative5 = propertyRelative4.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName");
                var parameters = m_Method.GetParameters();
                propertyRelative5.stringValue = parameters.Length != 1 || !typeof(UnityEngine.Object).IsAssignableFrom(parameters[0].ParameterType)
                    ? typeof(UnityEngine.Object).AssemblyQualifiedName
                    : parameters[0].ParameterType.AssemblyQualifiedName;
            }

            ValidateObjectParamater(propertyRelative4, m_Mode);
            m_Listener.serializedObject.ApplyModifiedProperties();
        }

        private void ValidateObjectParamater(SerializedProperty arguments, PersistentListenerMode mode)
        {
            var propertyRelative1 = arguments.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName");
            var propertyRelative2 = arguments.FindPropertyRelative("m_ObjectArgument");
            var objectReferenceValue = propertyRelative2.objectReferenceValue;
            if (mode != PersistentListenerMode.Object)
            {
                propertyRelative1.stringValue = typeof(UnityEngine.Object).AssemblyQualifiedName;
                propertyRelative2.objectReferenceValue = null;
            }
            else
            {
                if (objectReferenceValue == null)
                    return;
                var type = Type.GetType(propertyRelative1.stringValue, false);
                if (typeof(UnityEngine.Object).IsAssignableFrom(type) && type.IsInstanceOfType(objectReferenceValue))
                    return;
                propertyRelative2.objectReferenceValue = null;
            }
        }

        public void Clear()
        {
            m_Listener.FindPropertyRelative("m_MethodName").stringValue = null;
            m_Listener.FindPropertyRelative("m_Mode").enumValueIndex = 1;
            m_Listener.serializedObject.ApplyModifiedProperties();
        }
    }
}
#else
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using Object = UnityEngine.Object;
using UnityEditorInternal;

[CustomPropertyDrawer(typeof(UnityEvent), true)]
public class UnityEventDrawer : PropertyDrawer
{
    protected class State
    {
        internal ReorderableList m_ReorderableList;
        public int lastSelectedIndex;
    }

    private const string kNoFunctionString = "No Function";

    //Persistent Listener Paths
    private const string kInstancePath = "m_Target";
    private const string kCallStatePath = "m_CallState";
    private const string kArgumentsPath = "m_Arguments";
    private const string kModePath = "m_Mode";
    private const string kMethodNamePath = "m_MethodName";

    //ArgumentCache paths
    private const string kFloatArgument = "m_FloatArgument";
    private const string kIntArgument = "m_IntArgument";
    private const string kObjectArgument = "m_ObjectArgument";
    private const string kStringArgument = "m_StringArgument";
    private const string kBoolArgument = "m_BoolArgument";
    private const string kObjectArgumentAssemblyTypeName = "m_ObjectArgumentAssemblyTypeName";

    // Find internal methods with reflection
    private static MethodInfo findMethod =
 typeof(UnityEventBase).GetMethod("FindMethod", BindingFlags.NonPublic | BindingFlags.Instance, null, CallingConventions.Standard, new Type[] { typeof(string), typeof(object), typeof(PersistentListenerMode), typeof(Type) }, null);
    private static MethodInfo temp =
 typeof(GUIContent).GetMethod("Temp", BindingFlags.NonPublic | BindingFlags.Static, null, CallingConventions.Standard, new Type[] { typeof(string) }, null);
    private static PropertyInfo mixedValueContent = typeof(EditorGUI).GetProperty("mixedValueContent", BindingFlags.NonPublic | BindingFlags.Static);

    string m_Text;
    UnityEventBase m_DummyEvent;
    SerializedProperty m_Prop;
    SerializedProperty m_ListenersArray;

    const int kExtraSpacing = 9;

    //State:
    ReorderableList m_ReorderableList;
    int m_LastSelectedIndex;
    Dictionary<string, State> m_States = new Dictionary<string, State>();

    static string GetEventParams(UnityEventBase evt)
    {
        // Reflection for:
        // var methodInfo = evt.FindMethod("Invoke", evt, PersistentListenerMode.EventDefined, null);
        GetMethod(evt, "Invoke", evt, PersistentListenerMode.EventDefined, null);
        var methodInfo = (MethodInfo)findMethod.Invoke(evt, new object[] { "Invoke", evt, PersistentListenerMode.EventDefined, null });

        var sb = new StringBuilder();
        sb.Append(" (");

        var types = methodInfo.GetParameters().Select(x => x.ParameterType).ToArray();
        for (int i = 0; i < types.Length; i++)
        {
            sb.Append(types[i].Name);
            if (i < types.Length - 1)
            {
                sb.Append(", ");
            }
        }
        sb.Append(")");
        return sb.ToString();
    }

    private State GetState(SerializedProperty prop)
    {
        State state;
        string key = prop.propertyPath;
        m_States.TryGetValue(key, out state);
        // ensure the cached SerializedProperty is synchronized (case 974069)
        if (state == null || state.m_ReorderableList.serializedProperty.serializedObject != prop.serializedObject)
        {
            if (state == null)
                state = new State();

            SerializedProperty listenersArray = prop.FindPropertyRelative("m_PersistentCalls.m_Calls");
            state.m_ReorderableList = new ReorderableList(prop.serializedObject, listenersArray, false, true, true, true);
            state.m_ReorderableList.drawHeaderCallback = DrawEventHeader;
            state.m_ReorderableList.drawElementCallback = DrawEventListener;
            state.m_ReorderableList.onSelectCallback = SelectEventListener;
            state.m_ReorderableList.onReorderCallback = EndDragChild;
            state.m_ReorderableList.onAddCallback = AddEventListener;
            state.m_ReorderableList.onRemoveCallback = RemoveButton;
            // Two standard lines with standard spacing between and extra spacing below to better separate items visually.
            // Reflection for:
            // state.m_ReorderableList.elementHeight = EditorGUI.kSingleLineHeight * 2 + EditorGUI.kControlVerticalSpacing + kExtraSpacing;
            state.m_ReorderableList.elementHeight = 43f;

            m_States[key] = state;
        }
        return state;
    }

    private State RestoreState(SerializedProperty property)
    {
        State state = GetState(property);

        m_ListenersArray = state.m_ReorderableList.serializedProperty;
        m_ReorderableList = state.m_ReorderableList;
        m_LastSelectedIndex = state.lastSelectedIndex;
        m_ReorderableList.index = m_LastSelectedIndex;

        return state;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        m_Prop = property;
        m_Text = label.text;

        State state = RestoreState(property);

        OnGUI(position);
        state.lastSelectedIndex = m_LastSelectedIndex;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        //TODO: Also we need to have a constructor or initializer called for this property Drawer, before OnGUI or GetPropertyHeight
        //otherwise, we get Restore the State twice, once here and agai in OnGUI. Maybe we should only do it here?
        RestoreState(property);

        float height = 0f;
        if (m_ReorderableList != null)
        {
            height = m_ReorderableList.GetHeight();
        }
        return height;
    }

    public void OnGUI(Rect position)
    {
        if (m_ListenersArray == null || !m_ListenersArray.isArray)
            return;

        m_DummyEvent = GetDummyEvent(m_Prop);
        if (m_DummyEvent == null)
            return;

        if (m_ReorderableList != null)
        {
            var oldIdentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            m_ReorderableList.DoList(position);
            EditorGUI.indentLevel = oldIdentLevel;
        }
    }

    protected virtual void DrawEventHeader(Rect headerRect)
    {
        headerRect.height = 16;

        string text = (string.IsNullOrEmpty(m_Text) ? "Event" : m_Text) + GetEventParams(m_DummyEvent);
        GUI.Label(headerRect, text);
    }

    static PersistentListenerMode GetMode(SerializedProperty mode)
    {
        return (PersistentListenerMode)mode.enumValueIndex;
    }

    void DrawEventListener(Rect rect, int index, bool isactive, bool isfocused)
    {
        var pListener = m_ListenersArray.GetArrayElementAtIndex(index);

        rect.y++;
        Rect[] subRects = GetRowRects(rect);
        Rect enabledRect = subRects[0];
        Rect goRect = subRects[1];
        Rect functionRect = subRects[2];
        Rect argRect = subRects[3];

        // find the current event target...
        var callState = pListener.FindPropertyRelative(kCallStatePath);
        var mode = pListener.FindPropertyRelative(kModePath);
        var arguments = pListener.FindPropertyRelative(kArgumentsPath);
        var listenerTarget = pListener.FindPropertyRelative(kInstancePath);
        var methodName = pListener.FindPropertyRelative(kMethodNamePath);

        Color c = GUI.backgroundColor;
        GUI.backgroundColor = Color.white;

        EditorGUI.PropertyField(enabledRect, callState, GUIContent.none);

        EditorGUI.BeginChangeCheck();
        {
            GUI.Box(goRect, GUIContent.none);
            EditorGUI.PropertyField(goRect, listenerTarget, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
                methodName.stringValue = null;
        }

        SerializedProperty argument;
        var modeEnum = GetMode(mode);
        //only allow argument if we have a valid target / method
        if (listenerTarget.objectReferenceValue == null || string.IsNullOrEmpty(methodName.stringValue))
            modeEnum = PersistentListenerMode.Void;

        switch (modeEnum)
        {
            case PersistentListenerMode.Float:
                argument = arguments.FindPropertyRelative(kFloatArgument);
                break;
            case PersistentListenerMode.Int:
                argument = arguments.FindPropertyRelative(kIntArgument);
                break;
            case PersistentListenerMode.Object:
                argument = arguments.FindPropertyRelative(kObjectArgument);
                break;
            case PersistentListenerMode.String:
                argument = arguments.FindPropertyRelative(kStringArgument);
                break;
            case PersistentListenerMode.Bool:
                argument = arguments.FindPropertyRelative(kBoolArgument);
                break;
            default:
                argument = arguments.FindPropertyRelative(kIntArgument);
                break;
        }

        var desiredArgTypeName = arguments.FindPropertyRelative(kObjectArgumentAssemblyTypeName).stringValue;
        var desiredType = typeof(Object);
        if (!string.IsNullOrEmpty(desiredArgTypeName))
            desiredType = Type.GetType(desiredArgTypeName, false) ?? typeof(Object);

        if (modeEnum == PersistentListenerMode.Object)
        {
            EditorGUI.BeginChangeCheck();
            var result = EditorGUI.ObjectField(argRect, GUIContent.none, argument.objectReferenceValue, desiredType, true);
            if (EditorGUI.EndChangeCheck())
                argument.objectReferenceValue = result;
        }
        else if (modeEnum != PersistentListenerMode.Void && modeEnum != PersistentListenerMode.EventDefined)
        {
            // MODIFIED: support EnumAction attribute
            // This patch should be applied to any new version of UnityEventDrawer

            // Note that llamagod was also checking for !argument.serializedObject.isEditingMultipleObjects
            // as it wasn't supported. Right now changing multiple values work, but mixed values will not show as "-".

            // If you have issues with multiple edit on EnumAction callbacks, add a check on this, but not in the else if
            // above (which would also prevent showing a normal PropertyField), rather inside this block, falling back to
            // normal PropertyField when editing multiple objects.

            // Try to find Find the EnumActionAttribute
            var method = GetMethod(m_DummyEvent, methodName.stringValue, listenerTarget.objectReferenceValue, GetMode(mode), desiredType);
            object[] attributes = null;
            if (method != null)
                attributes = method.GetCustomAttributes(typeof(EnumActionAttribute), true);
            if (attributes != null && attributes.Length > 0)
            {
                // Make an enum popup
                var enumType = ((EnumActionAttribute)attributes[0]).enumType;
                var value = (Enum)Enum.ToObject(enumType, argument.intValue);
                argument.intValue = Convert.ToInt32(EditorGUI.EnumPopup(argRect, value));
            }
            else
            {
                EditorGUI.PropertyField(argRect, argument, GUIContent.none);
            }
        }

        using (new EditorGUI.DisabledScope(listenerTarget.objectReferenceValue == null))
        {
            EditorGUI.BeginProperty(functionRect, GUIContent.none, methodName);
            {
                GUIContent buttonContent;
                if (EditorGUI.showMixedValue)
                {
                    // Reflection for:
                    // buttonContent = EditorGUI.mixedValueContent;
                    buttonContent = (GUIContent)mixedValueContent.GetValue(null, null);
                }
                else
                {
                    var buttonLabel = new StringBuilder();
                    if (listenerTarget.objectReferenceValue == null || string.IsNullOrEmpty(methodName.stringValue))
                    {
                        buttonLabel.Append(kNoFunctionString);
                    }
                    else if (!IsPersistantListenerValid(m_DummyEvent, methodName.stringValue, listenerTarget.objectReferenceValue, GetMode(mode), desiredType))
                    {
                        var instanceString = "UnknownComponent";
                        var instance = listenerTarget.objectReferenceValue;
                        if (instance != null)
                            instanceString = instance.GetType().Name;

                        buttonLabel.Append(string.Format("<Missing {0}.{1}>", instanceString, methodName.stringValue));
                    }
                    else
                    {
                        buttonLabel.Append(listenerTarget.objectReferenceValue.GetType().Name);

                        if (!string.IsNullOrEmpty(methodName.stringValue))
                        {
                            buttonLabel.Append(".");
                            if (methodName.stringValue.StartsWith("set_"))
                                buttonLabel.Append(methodName.stringValue.Substring(4));
                            else
                                buttonLabel.Append(methodName.stringValue);
                        }
                    }
                    // Reflection for:
                    // buttonContent = GUIContent.Temp(buttonLabel.ToString());
                    buttonContent = (GUIContent)temp.Invoke(null, new object[] { buttonLabel.ToString() });
                }

                if (GUI.Button(functionRect, buttonContent, EditorStyles.popup))
                    BuildPopupList(listenerTarget.objectReferenceValue, m_DummyEvent, pListener).DropDown(functionRect);
            }
            EditorGUI.EndProperty();
        }
        GUI.backgroundColor = c;
    }

    Rect[] GetRowRects(Rect rect)
    {
        Rect[] rects = new Rect[4];

        // Reflection for:
        // rect.height = EditorGUI.kSingleLineHeight;
        rect.height = 16f;
        rect.y += 2;

        Rect enabledRect = rect;
        enabledRect.width *= 0.3f;

        Rect goRect = enabledRect;
        // Reflection for:
        // goRect.y += EditorGUIUtility.singleLineHeight + EditorGUI.kControlVerticalSpacing;
        goRect.y += 18f;

        Rect functionRect = rect;
        // Reflection for:
        // functionRect.xMin = goRect.xMax + EditorGUI.kSpacing;
        functionRect.xMin = goRect.xMax + 5f;

        Rect argRect = functionRect;
        // Reflection for:
        // argRect.y += EditorGUIUtility.singleLineHeight + EditorGUI.kControlVerticalSpacing;
        argRect.y += 18f;

        rects[0] = enabledRect;
        rects[1] = goRect;
        rects[2] = functionRect;
        rects[3] = argRect;
        return rects;
    }

    void RemoveButton(ReorderableList list)
    {
        ReorderableList.defaultBehaviours.DoRemoveButton(list);
        m_LastSelectedIndex = list.index;
    }

    private void AddEventListener(ReorderableList list)
    {
        if (m_ListenersArray.hasMultipleDifferentValues)
        {
            //When increasing a multi-selection array using Serialized Property
            //Data can be overwritten if there is mixed values.
            //The Serialization system applies the Serialized data of one object, to all other objects in the selection.
            //We handle this case here, by creating a SerializedObject for each object.
            //Case 639025.
            foreach (var targetObject in m_ListenersArray.serializedObject.targetObjects)
            {
                var temSerialziedObject = new SerializedObject(targetObject);
                var listenerArrayProperty = temSerialziedObject.FindProperty(m_ListenersArray.propertyPath);
                listenerArrayProperty.arraySize += 1;
                temSerialziedObject.ApplyModifiedProperties();
            }
            m_ListenersArray.serializedObject.SetIsDifferentCacheDirty();
            m_ListenersArray.serializedObject.Update();
            list.index = list.serializedProperty.arraySize - 1;
        }
        else
        {
            ReorderableList.defaultBehaviours.DoAddButton(list);
        }

        m_LastSelectedIndex = list.index;
        var pListener = m_ListenersArray.GetArrayElementAtIndex(list.index);

        var callState = pListener.FindPropertyRelative(kCallStatePath);
        var listenerTarget = pListener.FindPropertyRelative(kInstancePath);
        var methodName = pListener.FindPropertyRelative(kMethodNamePath);
        var mode = pListener.FindPropertyRelative(kModePath);
        var arguments = pListener.FindPropertyRelative(kArgumentsPath);

        callState.enumValueIndex = (int)UnityEventCallState.RuntimeOnly;
        listenerTarget.objectReferenceValue = null;
        methodName.stringValue = null;
        mode.enumValueIndex = (int)PersistentListenerMode.Void;
        arguments.FindPropertyRelative(kFloatArgument).floatValue = 0;
        arguments.FindPropertyRelative(kIntArgument).intValue = 0;
        arguments.FindPropertyRelative(kObjectArgument).objectReferenceValue = null;
        arguments.FindPropertyRelative(kStringArgument).stringValue = null;
        arguments.FindPropertyRelative(kObjectArgumentAssemblyTypeName).stringValue = null;
    }

    void SelectEventListener(ReorderableList list)
    {
        m_LastSelectedIndex = list.index;
    }

    void EndDragChild(ReorderableList list)
    {
        m_LastSelectedIndex = list.index;
    }

    static UnityEventBase GetDummyEvent(SerializedProperty prop)
    {
        // Create dummy instance of this type... we need it for function validation ect
        string typeName = prop.FindPropertyRelative("m_TypeName")?.stringValue;
        Type type = null;

        if (!string.IsNullOrEmpty(typeName))
        {
            type = Type.GetType(typeName, false);
        }

        if (type == null)
        {
            return new UnityEvent();
        }
        else
        {
            return Activator.CreateInstance(type) as UnityEventBase;
        }
    }

    struct ValidMethodMap
    {
        public Object target;
        public MethodInfo methodInfo;
        public PersistentListenerMode mode;
    }

    static IEnumerable<ValidMethodMap> CalculateMethodMap(Object target, Type[] t, bool allowSubclasses)
    {
        var validMethods = new List<ValidMethodMap>();
        if (target == null || t == null)
            return validMethods;

        // find the methods on the behaviour that match the signature
        Type componentType = target.GetType();
        var componentMethods = componentType.GetMethods().Where(x => !x.IsSpecialName).ToList();

        var wantedProperties = componentType.GetProperties().AsEnumerable();
        wantedProperties =
 wantedProperties.Where(x => x.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length == 0 && x.GetSetMethod() != null);
        componentMethods.AddRange(wantedProperties.Select(x => x.GetSetMethod()));

        foreach (var componentMethod in componentMethods)
        {
            //Debug.Log ("Method: " + componentMethod);
            // if the argument length is not the same, no match
            var componentParamaters = componentMethod.GetParameters();
            if (componentParamaters.Length != t.Length)
                continue;

            // Don't show obsolete methods.
            if (componentMethod.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length > 0)
                continue;

            if (componentMethod.ReturnType != typeof(void))
                continue;

            // if the argument types do not match, no match
            bool paramatersMatch = true;
            for (int i = 0; i < t.Length; i++)
            {
                if (!componentParamaters[i].ParameterType.IsAssignableFrom(t[i]))
                    paramatersMatch = false;

                if (allowSubclasses && t[i].IsAssignableFrom(componentParamaters[i].ParameterType))
                    paramatersMatch = true;
            }

            // valid method
            if (paramatersMatch)
            {
                var vmm = new ValidMethodMap();
                vmm.target = target;
                vmm.methodInfo = componentMethod;
                validMethods.Add(vmm);
            }
        }
        return validMethods;
    }

    public static bool IsPersistantListenerValid(UnityEventBase dummyEvent, string methodName, Object uObject, PersistentListenerMode modeEnum, Type argumentType)
    {
        if (uObject == null || string.IsNullOrEmpty(methodName))
            return false;

        // Reflection for:
        // return dummyEvent.FindMethod(methodName, uObject, modeEnum, argumentType) != null;
        return GetMethod(dummyEvent, methodName, uObject, modeEnum, argumentType) != null;
    }

    // Reflection-based method added for convenience
    // hsandt: modified parameter type Object -> object to allow reusing that method in GetEventParams
    private static MethodInfo GetMethod(UnityEventBase dummyEvent, string methodName, object obj, PersistentListenerMode modeEnum, Type argumentType)
    {
        return (MethodInfo)findMethod.Invoke(dummyEvent, new object[] { methodName, obj, modeEnum, argumentType });
    }

    static GenericMenu BuildPopupList(Object target, UnityEventBase dummyEvent, SerializedProperty listener)
    {
        //special case for components... we want all the game objects targets there!
        var targetToUse = target;
        if (targetToUse is Component)
            targetToUse = (target as Component).gameObject;

        // find the current event target...
        var methodName = listener.FindPropertyRelative(kMethodNamePath);

        var menu = new GenericMenu();
        menu.AddItem(new GUIContent(kNoFunctionString),
            string.IsNullOrEmpty(methodName.stringValue),
            ClearEventFunction,
            new UnityEventFunction(listener, null, null, PersistentListenerMode.EventDefined));

        if (targetToUse == null)
            return menu;

        menu.AddSeparator("");

        // figure out the signature of this delegate...
        // The property at this stage points to the 'container' and has the field name
        Type delegateType = dummyEvent.GetType();

        // check out the signature of invoke as this is the callback!
        MethodInfo delegateMethod = delegateType.GetMethod("Invoke");
        var delegateArgumentsTypes = delegateMethod.GetParameters().Select(x => x.ParameterType).ToArray();

        GeneratePopUpForType(menu, targetToUse, false, listener, delegateArgumentsTypes);
        if (targetToUse is GameObject)
        {
            Component[] comps = (targetToUse as GameObject).GetComponents<Component>();
            var duplicateNames =
 comps.Where(c => c != null).Select(c => c.GetType().Name).GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            foreach (Component comp in comps)
            {
                if (comp == null)
                    continue;

                GeneratePopUpForType(menu, comp, duplicateNames.Contains(comp.GetType().Name), listener, delegateArgumentsTypes);
            }
        }

        return menu;
    }

    private static void GeneratePopUpForType(GenericMenu menu, Object target, bool useFullTargetName, SerializedProperty listener, Type[] delegateArgumentsTypes)
    {
        var methods = new List<ValidMethodMap>();
        string targetName = useFullTargetName ? target.GetType().FullName : target.GetType().Name;

        bool didAddDynamic = false;

        // skip 'void' event defined on the GUI as we have a void prebuilt type!
        if (delegateArgumentsTypes.Length != 0)
        {
            GetMethodsForTargetAndMode(target, delegateArgumentsTypes, methods, PersistentListenerMode.EventDefined);
            if (methods.Count > 0)
            {
                menu.AddDisabledItem(new GUIContent(targetName + "/Dynamic " + string.Join(", ", delegateArgumentsTypes.Select(e => GetTypeName(e)).ToArray())));
                AddMethodsToMenu(menu, listener, methods, targetName);
                didAddDynamic = true;
            }
        }

        methods.Clear();
        GetMethodsForTargetAndMode(target, new[] { typeof(float) }, methods, PersistentListenerMode.Float);
        GetMethodsForTargetAndMode(target, new[] { typeof(int) }, methods, PersistentListenerMode.Int);
        GetMethodsForTargetAndMode(target, new[] { typeof(string) }, methods, PersistentListenerMode.String);
        GetMethodsForTargetAndMode(target, new[] { typeof(bool) }, methods, PersistentListenerMode.Bool);
        GetMethodsForTargetAndMode(target, new[] { typeof(Object) }, methods, PersistentListenerMode.Object);
        GetMethodsForTargetAndMode(target, new Type[] { }, methods, PersistentListenerMode.Void);
        if (methods.Count > 0)
        {
            if (didAddDynamic)
                // AddSeperator doesn't seem to work for sub-menus, so we have to use this workaround instead of a proper separator for now.
                menu.AddItem(new GUIContent(targetName + "/ "), false, null);
            if (delegateArgumentsTypes.Length != 0)
                menu.AddDisabledItem(new GUIContent(targetName + "/Static Parameters"));
            AddMethodsToMenu(menu, listener, methods, targetName);
        }
    }

    private static void AddMethodsToMenu(GenericMenu menu, SerializedProperty listener, List<ValidMethodMap> methods, string targetName)
    {
        // Note: sorting by a bool in OrderBy doesn't seem to work for some reason, so using numbers explicitly.
        IEnumerable<ValidMethodMap> orderedMethods =
 methods.OrderBy(e => e.methodInfo.Name.StartsWith("set_") ? 0 : 1).ThenBy(e => e.methodInfo.Name);
        foreach (var validMethod in orderedMethods)
            AddFunctionsForScript(menu, listener, validMethod, targetName);
    }

    private static void GetMethodsForTargetAndMode(Object target, Type[] delegateArgumentsTypes, List<ValidMethodMap> methods, PersistentListenerMode mode)
    {
        IEnumerable<ValidMethodMap> newMethods = CalculateMethodMap(target, delegateArgumentsTypes, mode == PersistentListenerMode.Object);
        foreach (var m in newMethods)
        {
            var method = m;
            method.mode = mode;
            methods.Add(method);
        }
    }

    static void AddFunctionsForScript(GenericMenu menu, SerializedProperty listener, ValidMethodMap method, string targetName)
    {
        PersistentListenerMode mode = method.mode;

        // find the current event target...
        var listenerTarget = listener.FindPropertyRelative(kInstancePath).objectReferenceValue;
        var methodName = listener.FindPropertyRelative(kMethodNamePath).stringValue;
        var setMode = GetMode(listener.FindPropertyRelative(kModePath));
        var typeName = listener.FindPropertyRelative(kArgumentsPath).FindPropertyRelative(kObjectArgumentAssemblyTypeName);

        var args = new StringBuilder();
        var count = method.methodInfo.GetParameters().Length;
        for (int index = 0; index < count; index++)
        {
            var methodArg = method.methodInfo.GetParameters()[index];
            args.Append(string.Format("{0}", GetTypeName(methodArg.ParameterType)));

            if (index < count - 1)
                args.Append(", ");
        }

        var isCurrentlySet = listenerTarget == method.target
            && methodName == method.methodInfo.Name
            && mode == setMode;

        if (isCurrentlySet && mode == PersistentListenerMode.Object && method.methodInfo.GetParameters().Length == 1)
        {
            isCurrentlySet &= (method.methodInfo.GetParameters()[0].ParameterType.AssemblyQualifiedName == typeName.stringValue);
        }

        string path = GetFormattedMethodName(targetName, method.methodInfo.Name, args.ToString(), mode == PersistentListenerMode.EventDefined);
        menu.AddItem(new GUIContent(path),
            isCurrentlySet,
            SetEventFunction,
            new UnityEventFunction(listener, method.target, method.methodInfo, mode));
    }

    private static string GetTypeName(Type t)
    {
        if (t == typeof(int))
            return "int";
        if (t == typeof(float))
            return "float";
        if (t == typeof(string))
            return "string";
        if (t == typeof(bool))
            return "bool";
        return t.Name;
    }

    static string GetFormattedMethodName(string targetName, string methodName, string args, bool dynamic)
    {
        if (dynamic)
        {
            if (methodName.StartsWith("set_"))
                return string.Format("{0}/{1}", targetName, methodName.Substring(4));
            else
                return string.Format("{0}/{1}", targetName, methodName);
        }
        else
        {
            if (methodName.StartsWith("set_"))
                return string.Format("{0}/{2} {1}", targetName, methodName.Substring(4), args);
            else
                return string.Format("{0}/{1} ({2})", targetName, methodName, args);
        }
    }

    static void SetEventFunction(object source)
    {
        ((UnityEventFunction)source).Assign();
    }

    static void ClearEventFunction(object source)
    {
        ((UnityEventFunction)source).Clear();
    }

    struct UnityEventFunction
    {
        readonly SerializedProperty m_Listener;
        readonly Object m_Target;
        readonly MethodInfo m_Method;
        readonly PersistentListenerMode m_Mode;

        public UnityEventFunction(SerializedProperty listener, Object target, MethodInfo method, PersistentListenerMode mode)
        {
            m_Listener = listener;
            m_Target = target;
            m_Method = method;
            m_Mode = mode;
        }

        public void Assign()
        {
            // find the current event target...
            var listenerTarget = m_Listener.FindPropertyRelative(kInstancePath);
            var methodName = m_Listener.FindPropertyRelative(kMethodNamePath);
            var mode = m_Listener.FindPropertyRelative(kModePath);
            var arguments = m_Listener.FindPropertyRelative(kArgumentsPath);

            listenerTarget.objectReferenceValue = m_Target;
            methodName.stringValue = m_Method.Name;
            mode.enumValueIndex = (int)m_Mode;

            if (m_Mode == PersistentListenerMode.Object)
            {
                var fullArgumentType = arguments.FindPropertyRelative(kObjectArgumentAssemblyTypeName);
                var argParams = m_Method.GetParameters();
                if (argParams.Length == 1 && typeof(Object).IsAssignableFrom(argParams[0].ParameterType))
                    fullArgumentType.stringValue = argParams[0].ParameterType.AssemblyQualifiedName;
                else
                    fullArgumentType.stringValue = typeof(Object).AssemblyQualifiedName;
            }

            ValidateObjectParamater(arguments, m_Mode);

            // Reflection for:
            // m_Listener.m_SerializedObject.ApplyModifiedProperties();
            m_Listener.serializedObject.ApplyModifiedProperties();
        }

        private void ValidateObjectParamater(SerializedProperty arguments, PersistentListenerMode mode)
        {
            var fullArgumentType = arguments.FindPropertyRelative(kObjectArgumentAssemblyTypeName);
            var argument = arguments.FindPropertyRelative(kObjectArgument);
            var argumentObj = argument.objectReferenceValue;

            if (mode != PersistentListenerMode.Object)
            {
                fullArgumentType.stringValue = typeof(Object).AssemblyQualifiedName;
                argument.objectReferenceValue = null;
                return;
            }

            if (argumentObj == null)
                return;

            Type t = Type.GetType(fullArgumentType.stringValue, false);
            if (!typeof(Object).IsAssignableFrom(t) || !t.IsInstanceOfType(argumentObj))
                argument.objectReferenceValue = null;
        }

        public void Clear()
        {
            // find the current event target...
            var methodName = m_Listener.FindPropertyRelative(kMethodNamePath);
            methodName.stringValue = null;

            var mode = m_Listener.FindPropertyRelative(kModePath);
            mode.enumValueIndex = (int)PersistentListenerMode.Void;

            // Reflection for:
            // m_Listener.m_SerializedObject.ApplyModifiedProperties();
            m_Listener.serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif