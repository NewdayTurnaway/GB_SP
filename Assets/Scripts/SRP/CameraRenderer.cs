using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

partial class CameraRenderer
{
    private const string BUFFER_NAME = nameof(CameraRenderer);

    private static readonly List<ShaderTagId> _drawingShaderTagIds = 
        new() { new("SRPDefaultUnlit"), new("SRPDefaultLit") };

    private ScriptableRenderContext _context;
    private Camera _camera;
    private CommandBuffer _commandBuffer;
    private CullingResults _cullingResults;

    private readonly Lighting _lighting = new();

    public void Render(ScriptableRenderContext context, Camera camera)
    {
        _camera = camera;
        _context = context;

        DrawUIInEditor();

        if (!Cull(out var parameters))
        {
            return;
        }

        Settings(parameters);

        _lighting.Setup(context, _cullingResults);

        DrawVisible();
        DrawUnsupportedShaders();
        DrawGizmos();
        Submit();
    }

    private bool Cull(out ScriptableCullingParameters parameters)
    {
        return _camera.TryGetCullingParameters(out parameters);
    }

    private void Settings(ScriptableCullingParameters parameters)
    {
        _commandBuffer = new() { name = _camera.name };
        _cullingResults = _context.Cull(ref parameters);
        _context.SetupCameraProperties(_camera);

        var clearFlags = _camera.clearFlags;
        var clearDepth = clearFlags <= CameraClearFlags.Depth;
        var clearColor = clearFlags == CameraClearFlags.Color;
        var solidColor = clearFlags == CameraClearFlags.Color ? 
            _camera.backgroundColor.linear : Color.clear;

        _commandBuffer.ClearRenderTarget(clearDepth, clearColor, solidColor);
        _commandBuffer.BeginSample(BUFFER_NAME);
        ExecuteCommandBuffer();
    }

    private void DrawVisible()
    {
        var drawingSettings = CreateDrawingSettings(_drawingShaderTagIds, 
            SortingCriteria.CommonOpaque,out var sortingSettings);
        drawingSettings.SetShaderPassName(1, _drawingShaderTagIds[1]);
        var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
        _context.DrawRenderers(_cullingResults, ref drawingSettings, ref filteringSettings);
        _context.DrawSkybox(_camera);

        sortingSettings.criteria = SortingCriteria.CommonTransparent;
        drawingSettings.sortingSettings = sortingSettings;
        filteringSettings.renderQueueRange = RenderQueueRange.transparent;
        _context.DrawRenderers(_cullingResults, ref drawingSettings, ref filteringSettings);
    }

    private DrawingSettings CreateDrawingSettings(List<ShaderTagId> shaderTags, 
        SortingCriteria sortingCriteria, out SortingSettings sortingSettings)
    {
        sortingSettings = new(_camera) { criteria = sortingCriteria };

        var drawingSettings = new DrawingSettings(shaderTags[0], sortingSettings);
        
        for (int i = 0; i < shaderTags.Count; i++)
        {
            drawingSettings.SetShaderPassName(i, shaderTags[i]);
        }

        return drawingSettings;
    }

    private void Submit()
    {
        _commandBuffer.EndSample(BUFFER_NAME);
        ExecuteCommandBuffer();
        _context.Submit();
    }

    private void ExecuteCommandBuffer()
    {
        _context.ExecuteCommandBuffer(_commandBuffer);
        _commandBuffer.Clear();
    }
}