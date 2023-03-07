using UnityEditor;
using UnityEngine;
using System;

[Flags]
public enum EditorListOption 
{
    None = 0,
    ListSize = 1,
    ListLabel = 2,
    ElementLabels = 4,
    Buttons = 8,
    Default = ListSize | ListLabel | ElementLabels,
    NoElementLabels = ListSize | ListLabel,
    All = Default | Buttons
}

public static class EditorList 
{
    private static readonly GUIContent _moveButtonContent = new("\u21b4", "move down");
    private static readonly GUIContent _duplicateButtonContent = new("+", "duplicate");
    private static readonly GUIContent _deleteButtonContent = new("-", "delete");
    private static readonly GUIContent _addButtonContent = new("+", "add element");
    private static readonly GUILayoutOption _miniButtonWidth = GUILayout.Width(20f);

    public static void Show (SerializedProperty list, EditorListOption options = EditorListOption.Default) 
    {
        if (!list.isArray) 
        {
            EditorGUILayout.HelpBox(list.name + " is neither an array nor a list!", MessageType.Error);
            return;
        }

        var isShowListLabel = (options & EditorListOption.ListLabel) != 0;
        var isShowListSize = (options & EditorListOption.ListSize) != 0;

        if (isShowListLabel) 
        {
            EditorGUILayout.PropertyField(list);
            EditorGUI.indentLevel += 1;
        }

        if (!isShowListLabel || list.isExpanded) 
        {
            var sizeProperty = list.FindPropertyRelative("Array.size");

            if (isShowListSize) 
            {
                EditorGUILayout.PropertyField(sizeProperty);
            }

            if (sizeProperty.hasMultipleDifferentValues) 
            {
                EditorGUILayout.HelpBox("Not showing lists with different sizes.", MessageType.Info);
            }
            else 
            {
                ShowElements(list, options);
            }
        }

        if (isShowListLabel) 
        {
            EditorGUI.indentLevel -= 1;
        }
    }

    private static void ShowElements (SerializedProperty list, EditorListOption options) 
    {
        var oldIndentLevel = EditorGUI.indentLevel;
        EditorGUI.indentLevel += 1;
        bool isShowElementLabels = (options & EditorListOption.ElementLabels) != 0;
        bool isShowButtons = (options & EditorListOption.Buttons) != 0;

        for (int i = 0; i < list.arraySize; i++) 
        {
            if (isShowButtons) 
            {
                EditorGUILayout.BeginHorizontal();
            }

            if (isShowElementLabels) 
            {
                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
            }
            else 
            {
                EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), GUIContent.none);
            }

            if (isShowButtons) 
            {
                ShowButtons(list, i);
                EditorGUILayout.EndHorizontal();
            }
        }

        if (isShowButtons && list.arraySize == 0 && GUILayout.Button(_addButtonContent, EditorStyles.miniButton)) 
        {
            list.arraySize += 1;
        }

        EditorGUI.indentLevel = oldIndentLevel;
    }

    private static void ShowButtons (SerializedProperty list, int index) 
    {
        if (GUILayout.Button(_moveButtonContent, EditorStyles.miniButtonLeft, _miniButtonWidth)) 
        {
            list.MoveArrayElement(index, index + 1);
        }

        if (GUILayout.Button(_duplicateButtonContent, EditorStyles.miniButtonMid, _miniButtonWidth)) 
        {
            list.InsertArrayElementAtIndex(index);
        }

        if (GUILayout.Button(_deleteButtonContent, EditorStyles.miniButtonRight, _miniButtonWidth)) 
        {
            var oldSize = list.arraySize;
            list.DeleteArrayElementAtIndex(index);
            
            if (list.arraySize == oldSize) 
            {
                list.DeleteArrayElementAtIndex(index);
            }
        }
    }
}