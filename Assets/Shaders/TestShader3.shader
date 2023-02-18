Shader "Custom/TestShader3"
{
    Properties
    {
        _MainTex("Albedo", 2D) = "white" {}
        _Color("Main Color", COLOR) = (1,1,1,1)
        _OutlineWidth("Outline Width", Range(0, 0.3)) = 0.1
        _OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags 
        {
            "Queue" = "Transparent+110"
            "RenderType" = "Transparent" 
        }

        LOD 100
        Cull off

        Pass
        {
            ZWrite off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _OutlineColor;
            float _OutlineWidth;
            
            struct v2f
            {
                float4 position : SV_POSITION;
                fixed4 color : COLOR;
            };

            float4 outline(float4 vertexPos, float outlineValue)
            {
                float4x4 scale = float4x4
                (
                    1 + outlineValue, 0, 0, 0,
                    0, 1 + outlineValue, 0, 0,
                    0, 0, 1 + outlineValue, 0,
                    0, 0, 0, 1 + outlineValue
                );
                return mul(scale, vertexPos);
            }

            v2f vert(appdata_base v)
            {
                v2f result;
                float4 vertexPos = outline(v.vertex, _OutlineWidth);
                result.position = UnityObjectToClipPos(vertexPos);
                result.color = _OutlineColor;
                return result;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return i.color;
            }

            ENDCG
        }

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata_base v)
            {
                v2f result;
                result.vertex = UnityObjectToClipPos(v.vertex);
                result.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return result;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 color;
                color = tex2D(_MainTex, i.uv);
                color = color * _Color;
                return color;
            }

            ENDCG
        }
    }
}
