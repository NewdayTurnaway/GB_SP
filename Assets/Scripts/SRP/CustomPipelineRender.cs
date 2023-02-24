using UnityEngine;
using UnityEngine.Rendering;

public class CustomPipelineRender : RenderPipeline
{
    CameraRenderer _cameraRenderer;

    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        _cameraRenderer = new();
        CamerasRenderer(context, cameras);
    }

    private void CamerasRenderer(ScriptableRenderContext context, Camera[] cameras)
    {
        foreach (var camera in cameras)
        {
            _cameraRenderer.Render(context, camera);
        }
    }
}
