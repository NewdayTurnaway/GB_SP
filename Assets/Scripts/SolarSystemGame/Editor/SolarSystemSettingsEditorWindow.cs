using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SolarSystemGame
{
    public class SolarSystemSettingsEditorWindow : EditorWindow
    {
        [SerializeField] private SolarSystemSettings _solarSystemSettings;
        [SerializeField] private List<SpaceObjectOrbitSettings> _spaceObjectOrbitSettings;

        private SpaceObjectOrbit[] _spaceObjectOrbits;
        private Vector2 _scrollPos;
        
        private Material _material;

        private void OnEnable()
        {
            _solarSystemSettings ??= FindObjectOfType<SolarSystemSettings>();

            if (_solarSystemSettings == null)
            {
                return;
            }

            _spaceObjectOrbitSettings = new();
            _spaceObjectOrbits = _solarSystemSettings.GetComponentsInChildren<SpaceObjectOrbit>();

            for (int i = 0; i < _spaceObjectOrbits.Length; i++)
            {
                _spaceObjectOrbitSettings.Add(_spaceObjectOrbits[i].Settings);
            }

            var shader = Shader.Find("Hidden/Internal-Colored");
            _material = new Material(shader);
        }

        private void OnDisable()
        {
            _solarSystemSettings = null;
            _spaceObjectOrbitSettings.Clear();
        }

        [MenuItem("SolarSystem/SolarSystemSettings")]
        public static void ShowWindow()
        {
            GetWindow(typeof(SolarSystemSettingsEditorWindow), true, nameof(SolarSystemSettings));
        }

        private void OnGUI()
        {
            //minSize = new(460, 600);

            if (_solarSystemSettings == null)
            {
                EditorGUILayout.HelpBox($"{nameof(SolarSystemSettings)} is is missing on scene!", MessageType.Warning);
                return;
            }

            var serializedObject = new SerializedObject(this);
            var spaceObjectOrbitSettingsProperty = serializedObject.FindProperty("_spaceObjectOrbitSettings");
            var offsetHeight = EditorGUIUtility.singleLineHeight * 2.5f;
            var height = position.height - offsetHeight;

            var maxPos = 0f;
            for (int i = 0; i < _spaceObjectOrbitSettings.Count; i++)
            {
                maxPos = Mathf.Max(maxPos, _spaceObjectOrbitSettings[i].xPosition);
            }
            var scaleFactor = maxPos / height;


            EditorGUILayout.ObjectField(_solarSystemSettings, typeof(SolarSystemSettings), true);
            EditorGUILayout.LabelField("Solar System Map", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            var rectWidth = 125;
            var rect = GUILayoutUtility.GetRect(rectWidth, height);
            if (Event.current.type == EventType.Repaint)
            {
                GUI.BeginClip(rect);

                GL.Clear(true, false, Color.black);
                
                GL.Begin(GL.LINES);
                _material.SetPass(0);

                GL.Color(Color.grey);
                GL.Vertex3(rectWidth / 2, 0, 0);
                GL.Vertex3(rectWidth / 2, height, 0);
                
                GL.End();

                var centers = new Vector3[_spaceObjectOrbitSettings.Count];
                for (int i = 0; i < _spaceObjectOrbitSettings.Count; i++)
                {
                    var radius = _spaceObjectOrbitSettings[i].scale / 2;
                    var xPos = _spaceObjectOrbitSettings[i].xPosition / scaleFactor;
                    centers[i] = new Vector3(rectWidth / 2, xPos, 0);

                    var isLast = i == _spaceObjectOrbitSettings.Count - 1;

                    if (i == 0 || isLast)
                    {
                        DrawGLCircle(radius, centers[i], true, isLast);
                        continue;
                    }

                    DrawGLCircle(radius, centers[i]);
                }

                for (int i = 0; i < _spaceObjectOrbitSettings.Count; i++)
                {
                    var name = _spaceObjectOrbitSettings[i].name;
                    
                    var lableWidth = name.Length * GUI.skin.font.fontSize;
                    var lableHeight = EditorGUIUtility.singleLineHeight;

                    var isLast = i == _spaceObjectOrbitSettings.Count - 1;

                    if (i == 0 || isLast)
                    {
                        var y = isLast ? centers[i].y - lableHeight : centers[i].y;
                        GUI.Label(new(centers[i].x - lableWidth / 3.5f, y, lableWidth, lableHeight), name);
                        continue;
                    }

                    GUI.Label(new(centers[i].x - lableWidth / 3.5f, centers[i].y - lableHeight / 2, lableWidth, lableHeight), name);
                }

                GUI.EndClip();
            }

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Height(height));
            EditorGUILayout.PropertyField(spaceObjectOrbitSettingsProperty);
            EditorGUILayout.EndScrollView();
            
            EditorGUILayout.EndHorizontal();

            UpdateSpaceObjectOrbitSettings();
        }

        private void DrawGLCircle(float radius, Vector3 center, bool isHalfCircle = false, bool isUpperHalf = false)
        {
            GL.Begin(GL.LINE_STRIP);
            GL.Color(new Color32(28,113,196,255));

            var sign = isUpperHalf ? -1 : 1;
            var circumference = isHalfCircle ? Mathf.PI : 2 * Mathf.PI;

            for (float theta = 0.0f; Mathf.Abs(theta) < circumference; theta += 0.125f * sign)
            {
                var ci = new Vector3(Mathf.Cos(theta) * radius + center.x, Mathf.Sin(theta) * radius + center.y, center.z);
                GL.Vertex3(ci.x, ci.y, ci.z);
            }

            GL.End();
        }

        private void UpdateSpaceObjectOrbitSettings()
        {
            for (int i = 0; i < _spaceObjectOrbits.Length; i++)
            {
                var pos = _spaceObjectOrbits[i].transform.position;
                pos = new(_spaceObjectOrbitSettings[i].xPosition, pos.y, pos.z);
                _spaceObjectOrbits[i].transform.position = pos;
                
                var scale = Vector3.one * _spaceObjectOrbitSettings[i].scale;
                _spaceObjectOrbits[i].transform.localScale = scale;
            }
        }
    }
}