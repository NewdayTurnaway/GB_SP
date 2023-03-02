using UnityEngine;
using UnityEngine.Rendering;

public class SRPTest : MonoBehaviour
{
    [field: SerializeField] public RenderPipelineAsset RenderPipelineAsset { get; private set; }

    private void Awake()
    {
        GraphicsSettings.renderPipelineAsset = RenderPipelineAsset;
        Debug.Log("SRP Enable");
    }

    private void OnDestroy()
    {
        GraphicsSettings.renderPipelineAsset = null;
        Debug.Log("SRP Disable");
    }
}
