Shader "Custom/SRPDefaultLit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Main Color", COLOR) = (1,1,1,1)
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque" 
            "LightMode"="SRPDefaultLit"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define MAX_DIRECTIONAL_LIGHT_COUNT 4

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            int _DirectionalLightCount;
            float4 _DirectionalLightColors[MAX_DIRECTIONAL_LIGHT_COUNT];
            float4 _DirectionalLightDirections[MAX_DIRECTIONAL_LIGHT_COUNT];

            struct Light 
            {
                float3 color;
                float3 direction;
            };

            Light GetDirectionalLight(int index) {
                Light light;
                light.color = _DirectionalLightColors[index].rgb;
                light.direction = _DirectionalLightDirections[index].xyz;
                return light;
            }

            float3 GetLighting(float3 normal, Light light)
            {
                return saturate(dot(normal, light.direction)) * light.color;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = v.normal;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col = col * _Color;
                
                float3 n = normalize(i.normal);
                float3 colorWithLight = col;
                for (int i = 0; i < _DirectionalLightCount; i++) 
                {
                    colorWithLight *= GetLighting(n, GetDirectionalLight(i));
                }

                return float4(colorWithLight, col.a);
            }

            ENDCG
        }
    }
}
