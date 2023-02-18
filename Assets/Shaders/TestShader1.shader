Shader "Custom/TestShader1"
{
    Properties
    {
        _Tex1 ("Texture1", 2D) = "white" {}
        _Tex2 ("Texture2", 2D) = "white" {}
        _MixValue("Mix Value", Range(0,1)) = 0.5
        _Color("Main Color", COLOR) = (1,1,1,1)
        _Height("Height", Range(-3.15, 3.15)) = 1
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
            float _Height;

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
                v.vertex.xyz += v.normal * sin(v.texcoord.y * _Height);
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
