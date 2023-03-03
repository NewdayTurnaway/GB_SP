using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Star)), CanEditMultipleObjects]
public class StarInspector : Editor 
{
    private static Vector3 _pointSnap = Vector3.one * 0.1f;

    public override void OnInspectorGUI () 
    {
        var pointsProperty = serializedObject.FindProperty("points");
        var frequencyProperty = serializedObject.FindProperty("frequency");

        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("center"));
        EditorList.Show(pointsProperty, EditorListOption.Buttons | EditorListOption.ListSize | EditorListOption.ElementLabels);
        EditorGUILayout.IntSlider(frequencyProperty, 1, 20);

        if (!serializedObject.isEditingMultipleObjects) 
        {
            var totalPoints = frequencyProperty.intValue * pointsProperty.arraySize;

            if (totalPoints < 3) 
            {
                EditorGUILayout.HelpBox("At least three points are needed.", MessageType.Warning);
            }
            else 
            {
                EditorGUILayout.HelpBox(totalPoints + " points in total.", MessageType.Info);
            }
        }

        if (serializedObject.ApplyModifiedProperties() 
            || (Event.current.type == EventType.ValidateCommand 
            && Event.current.commandName == "UndoRedoPerformed"))
        {
            foreach (Star s in targets.Cast<Star>()) 
            {
                if (PrefabUtility.GetPrefabAssetType(s) != PrefabAssetType.Regular) 
                {
                    s.UpdateMesh();
                }
            }
        }
    }

    void OnSceneGUI () 
    {
        var star = target as Star;
        var starTransform = star.transform;

        var angle = -360f / (star.frequency * star.points.Length);
        for (int i = 0; i < star.points.Length; i++) 
        {
            var rotation = Quaternion.Euler(0f, 0f, angle * i);
            var oldPoint = starTransform.TransformPoint(rotation * star.points[i].position);
            var newPoint = Handles.FreeMoveHandle(oldPoint, Quaternion.identity, 0.02f, _pointSnap, Handles.DotHandleCap);

            if (oldPoint != newPoint) 
            {
                Undo.RecordObject(star, "Move");
                star.points[i].position = Quaternion.Inverse(rotation) * starTransform.InverseTransformPoint(newPoint);
                star.UpdateMesh();
            }
        }
    }
}