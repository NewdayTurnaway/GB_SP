using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ColorPoint))]
public class ColorPointDrawer : PropertyDrawer 
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) * 2;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var oldIndentLevel = EditorGUI.indentLevel;
        label = EditorGUI.BeginProperty(position, label, property);
        EditorGUI.PrefixLabel(position, label);
        position.xMin = EditorGUIUtility.labelWidth/2;
        position.height = EditorGUIUtility.singleLineHeight;
        var contentRectPosition = EditorGUI.IndentedRect(position);
        contentRectPosition.width *= 0.8f;
        EditorGUI.PropertyField(contentRectPosition, property.FindPropertyRelative("position"), GUIContent.none);
        contentRectPosition.x += contentRectPosition.width;
        contentRectPosition.width /= 4f;
        EditorGUIUtility.labelWidth = EditorGUIUtility.singleLineHeight / 2;
        EditorGUI.PropertyField(contentRectPosition, property.FindPropertyRelative("color"), new GUIContent("C"));
        EditorGUI.EndProperty();
        EditorGUI.indentLevel = oldIndentLevel;
    }
}