Shader "Custom/TestShader2"
{
    Properties
    {
        _Tex1 ("Texture1", 2D) = "white" {}
        _Tex2 ("Texture2", 2D) = "white" {}
        _MixValue("Mix Value", Range(0,1)) = 0.5
        _Color("Main Color", COLOR) = (1,1,1,1)

        _BendAmount("Bend Amount", Vector) = (1,0.05,1)
        _BendOrigin("Bend Origin", Vector) = (0,0,0)
        _BendFallOff("Bend Falloff", float) = 0
        _BendFallOffStr("Falloff strength", Range(0.00001,5)) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _Tex1;
            float4 _Tex1_ST;
            
            sampler2D _Tex2;
            float4 _Tex2_ST;

            float _MixValue;
            float4 _Color;

            float3 _BendAmount;
            float3 _BendOrigin;
            float _BendFallOff;
            float _BendFallOffStr;

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata_full v)
            {
                v2f result;
                float dist = length(v.vertex.xyz - _BendOrigin.xyz);
                dist = max(0, dist - _BendFallOff);
                dist = pow(dist, _BendFallOffStr);
                v.vertex.xyz += dist * _BendAmount;
                result.vertex = UnityObjectToClipPos(v.vertex);
                result.uv = TRANSFORM_TEX(v.texcoord, _Tex1);
                return result;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color;
                color = tex2D(_Tex1, i.uv) * _MixValue;
                color += tex2D(_Tex2, i.uv) * (1 - _MixValue);
                color = color * _Color;
                return color;
            }
            ENDCG
        }
    }
}
