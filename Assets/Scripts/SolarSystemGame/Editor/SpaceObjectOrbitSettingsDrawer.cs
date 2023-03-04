using UnityEditor;
using UnityEngine;

namespace SolarSystemGame
{
    [CustomPropertyDrawer(typeof(SpaceObjectOrbitSettings))]
    public class SpaceObjectOrbitSettingsDrawer : PropertyDrawer
    {
        private float _lastXPos;
        private float _lastScale;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) * 4 + 4 * 4;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();

            var nameProperty = property.FindPropertyRelative("name");
            var xPositionProperty = property.FindPropertyRelative("xPosition");
            var scaleProperty = property.FindPropertyRelative("scale");

            if (property.serializedObject.targetObject is SpaceObjectOrbit spaceObjectOrbit)
            {
                nameProperty.stringValue = property.serializedObject.targetObject.name.ToString();
                
                var pos = spaceObjectOrbit.transform.position;
                var scale = spaceObjectOrbit.transform.localScale;

                if (Mathf.Approximately(_lastXPos, pos.x))
                {
                    pos = new(xPositionProperty.floatValue, pos.y, pos.z);
                    spaceObjectOrbit.transform.position = pos;
                }
                else 
                {
                    xPositionProperty.floatValue = spaceObjectOrbit.transform.position.x;
                }
                
                if (Mathf.Approximately(_lastScale, scale.x))
                {
                    scale = Vector3.one * scaleProperty.floatValue;
                    spaceObjectOrbit.transform.localScale = scale;
                }
                else 
                {
                    scaleProperty.floatValue = spaceObjectOrbit.transform.localScale.x;
                }

                _lastXPos = spaceObjectOrbit.transform.position.x;
                _lastScale = spaceObjectOrbit.transform.localScale.x;
            }

            var circleInSecondProperty = property.FindPropertyRelative("circleInSecond");
            var offsetSinProperty = property.FindPropertyRelative("offsetSin");
            var offsetCosProperty = property.FindPropertyRelative("offsetCos");
            var rotationSpeedProperty = property.FindPropertyRelative("rotationSpeed");

            var rect = EditorGUI.IndentedRect(position);
            rect.width = EditorGUIUtility.singleLineHeight * 3;
            rect.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(rect, nameProperty.stringValue, EditorStyles.boldLabel);

            rect.width = EditorGUIUtility.singleLineHeight * 5 + 2;
            rect.x += rect.width;
            rect.width = 1;
            rect.height = rect.height * 4 + 4 * 4;
            EditorGUI.DrawRect(rect, Color.gray);
            rect = EditorGUI.IndentedRect(position);
            rect.height = EditorGUIUtility.singleLineHeight;

            rect.y += rect.height + 4;
            rect.width = EditorGUIUtility.singleLineHeight * 5;
            EditorGUIUtility.labelWidth = EditorGUIUtility.singleLineHeight * 3;
            EditorGUI.PropertyField(rect, xPositionProperty, new GUIContent("Position"));

            rect.y += rect.height + 4;
            rect.width = EditorGUIUtility.singleLineHeight * 5;
            EditorGUIUtility.labelWidth = EditorGUIUtility.singleLineHeight * 3;
            EditorGUI.PropertyField(rect, scaleProperty, new GUIContent("Scale"));

            rect = EditorGUI.IndentedRect(position);
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.width = EditorGUIUtility.singleLineHeight * 5 + 4;
            rect.x += rect.width;
            rect.width = EditorGUIUtility.singleLineHeight * 9;
            EditorGUIUtility.labelWidth = EditorGUIUtility.singleLineHeight * 5;
            EditorGUI.PropertyField(rect, circleInSecondProperty, new GUIContent("CircleInSec"));

            rect.y += rect.height + 4;
            rect.width = EditorGUIUtility.singleLineHeight * 9;
            EditorGUIUtility.labelWidth = EditorGUIUtility.singleLineHeight * 5;
            EditorGUI.PropertyField(rect, offsetSinProperty, new GUIContent("OffsetSin"));

            rect.y += rect.height + 4;
            rect.width = EditorGUIUtility.singleLineHeight * 9;
            EditorGUIUtility.labelWidth = EditorGUIUtility.singleLineHeight * 5;
            EditorGUI.PropertyField(rect, offsetCosProperty, new GUIContent("OffsetCos"));

            rect.y += rect.height + 4;
            rect.width = EditorGUIUtility.singleLineHeight * 9;
            EditorGUIUtility.labelWidth = EditorGUIUtility.singleLineHeight * 5;
            EditorGUI.PropertyField(rect, rotationSpeedProperty, new GUIContent("RotationSpeed"));

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}