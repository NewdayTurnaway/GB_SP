using UnityEngine;

public class ShaderTest : MonoBehaviour
{
    [field: Header("Test Materials")]
    [field: SerializeField] public MeshRenderer MeshRenderer1 { get; private set; }
    [field: SerializeField] public MeshRenderer MeshRenderer2 { get; private set; }
    [field: SerializeField] public MeshRenderer MeshRenderer3 { get; private set; }

    [field: Header("Shader 1 Test")]
    [field: SerializeField] public Color Color1 { get; private set; } = Color.white;
    [field: SerializeField, Range(0, 1)] public float MixValue1 { get; private set; } = 0.5f;

    [field: Header("Shader 2 Test")]
    [field: SerializeField] public Color Color2 { get; private set; } = Color.white;
    [field: SerializeField, Range(0, 1)] public float MixValue2 { get; private set; } = 0.5f;
    [field: SerializeField] public Vector3 BendAmount { get; private set; } = new(0, 0.1f, 0);
    [field: SerializeField] public Vector3 BendOrigin { get; private set; } = Vector3.zero;
    [field: SerializeField] public float BendFallOff { get; private set; } = 0;

    [field: Header("Shader 3 Test")]
    [field: SerializeField] public Color32 OutlineColor { get; private set; } = new(1, 155, 153, 64);

    private void Update()
    {
        Shader1();
        Shader2();
        Shader3();
    }

    private void Shader1()
    {
        MeshRenderer1.material.SetColor("_Color", Color1);
        MeshRenderer1.material.SetFloat("_MixValue", MixValue1);
        MeshRenderer1.material.SetFloat("_Height", Mathf.Lerp(-3.2f, 3.2f, Mathf.PingPong(Time.time/4, 1)));
    }

    private void Shader2()
    {
        MeshRenderer2.material.SetColor("_Color", Color2);
        MeshRenderer2.material.SetFloat("_MixValue", MixValue2);
        MeshRenderer2.material.SetVector("_BendAmount", (Vector4)BendAmount);
        MeshRenderer2.material.SetVector("_BendOrigin", (Vector4)BendOrigin);
        MeshRenderer2.material.SetFloat("_BendFallOff", BendFallOff);
        MeshRenderer2.material.SetFloat("_BendFallOffStr", Mathf.Lerp(0.00001f, 2, Mathf.PingPong(Time.time / 4, 1)));
    }
    private void Shader3()
    {
        MeshRenderer3.material.SetColor("_OutlineColor", OutlineColor);
        MeshRenderer3.material.SetFloat("_OutlineWidth", Mathf.Lerp(0.1f, 0.3f, Mathf.PingPong(Time.time / 4, 1)));
    }
}