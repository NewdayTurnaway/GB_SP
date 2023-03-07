using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

partial class CameraRenderer
{
    partial void DrawUIInEditor();
    partial void DrawUnsupportedShaders();
    partial void DrawGizmos();

#if UNITY_EDITOR

    private static readonly ShaderTagId[] _legacyShaderTagIds =
    {
        new("Always"),
        new("ForwardBase"),
        new("PrepassBase"),
        new("Vertex"),
        new("VertexLMRGBM"),
        new("VertexLM")
    };

    private static readonly Material _errorMaterial = new(Shader.Find("Hidden/InternalErrorShader"));

    partial void DrawUIInEditor()
    {
        if (_camera.cameraType == CameraType.SceneView)
        {
            ScriptableRenderContext.EmitWorldGeometryForSceneView(_camera);
        }
    }

    partial void DrawUnsupportedShaders()
    {
        var drawingSettings = new DrawingSettings
            (_legacyShaderTagIds[0], new(_camera)) { overrideMaterial = _errorMaterial };

        for (var i = 1; i < _legacyShaderTagIds.Length; i++)
        {
            drawingSettings.SetShaderPassName(i, _legacyShaderTagIds[i]);
        }

        var filteringSettings = FilteringSettings.defaultValue;
        _context.DrawRenderers(_cullingResults, ref drawingSettings, ref filteringSettings);
    }

    partial void DrawGizmos()
    {
        if (!Handles.ShouldRenderGizmos())
        {
            return;
        }

        _context.DrawGizmos(_camera, GizmoSubset.PreImageEffects);
        _context.DrawGizmos(_camera, GizmoSubset.PostImageEffects);
    }

#endif
}