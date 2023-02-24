using UnityEngine;
using UnityEngine.Rendering;

public class Lighting {

    private const string BUFFER_NAME = nameof(Lighting);
    private const int MAX_DIR_LIGHT_COUNT = 4;

    private static readonly int _dirLightCountId = Shader.PropertyToID("_DirectionalLightCount");
    private static readonly int _dirLightColorsId = Shader.PropertyToID("_DirectionalLightColors");
    private static readonly int _dirLightDirectionsId = Shader.PropertyToID("_DirectionalLightDirections");

    private static readonly Vector4[] _dirLightColors = new Vector4[MAX_DIR_LIGHT_COUNT];
    private static readonly Vector4[] _dirLightDirections = new Vector4[MAX_DIR_LIGHT_COUNT];

    private readonly CommandBuffer _commandBuffer = new() { name = BUFFER_NAME };

    private CullingResults _cullingResults;

    public void Setup (ScriptableRenderContext context, CullingResults cullingResults) 
    {
        _cullingResults = cullingResults;
        _commandBuffer.BeginSample(BUFFER_NAME);
        SetupLights();
        _commandBuffer.EndSample(BUFFER_NAME);
        context.ExecuteCommandBuffer(_commandBuffer);
        _commandBuffer.Clear();
    }

    private void SetupLights() 
    {
        var visibleLights = _cullingResults.visibleLights;
        var dirLightCount = 0;

        for (int i = 0; i < visibleLights.Length; i++) 
        {
            var visibleLight = visibleLights[i];

            if (visibleLight.lightType == LightType.Directional) 
            {
                SetupDirectionalLight(dirLightCount++, ref visibleLight);
                
                if (dirLightCount >= MAX_DIR_LIGHT_COUNT) 
                {
                    break;
                }
            }
        }

        _commandBuffer.SetGlobalInt(_dirLightCountId, dirLightCount);
        _commandBuffer.SetGlobalVectorArray(_dirLightColorsId, _dirLightColors);
        _commandBuffer.SetGlobalVectorArray(_dirLightDirectionsId, _dirLightDirections);
    }

    private void SetupDirectionalLight(int index, ref VisibleLight visibleLight) 
    {
        _dirLightColors[index] = visibleLight.finalColor;
        _dirLightDirections[index] = -visibleLight.localToWorldMatrix.GetColumn(2);
    }
}