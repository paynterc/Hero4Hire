using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AICondition))]
public class AIConditionDrawer : PropertyDrawer
{
    private static Type[] s_types;
    private static string[] s_names;

    static AIConditionDrawer()
    {
        s_types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => { try { return a.GetTypes(); } catch { return Type.EmptyTypes; } })
            .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(AICondition)))
            .OrderBy(t => t.Name)
            .ToArray();

        s_names = new[] { "None" }
            .Concat(s_types.Select(t => ObjectNames.NicifyVariableName(t.Name)))
            .ToArray();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        if (property.managedReferenceValue == null) return height;

        var child = property.Copy();
        var end = property.GetEndProperty();
        if (child.NextVisible(true))
        {
            while (!SerializedProperty.EqualContents(child, end))
            {
                height += EditorGUI.GetPropertyHeight(child) + EditorGUIUtility.standardVerticalSpacing;
                if (!child.NextVisible(false)) break;
            }
        }
        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var currentType = property.managedReferenceValue?.GetType();
        int currentIndex = currentType == null ? 0 : Array.IndexOf(s_types, currentType) + 1;

        Rect lineRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        int newIndex = EditorGUI.Popup(lineRect, "Condition", currentIndex, s_names);

        if (newIndex != currentIndex)
        {
            property.managedReferenceValue = newIndex == 0
                ? null
                : Activator.CreateInstance(s_types[newIndex - 1]);
        }

        if (property.managedReferenceValue == null) return;

        EditorGUI.indentLevel++;
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        var child = property.Copy();
        var end = property.GetEndProperty();
        if (child.NextVisible(true))
        {
            while (!SerializedProperty.EqualContents(child, end))
            {
                position.height = EditorGUI.GetPropertyHeight(child);
                EditorGUI.PropertyField(position, child, true);
                position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                if (!child.NextVisible(false)) break;
            }
        }
        EditorGUI.indentLevel--;
    }
}
