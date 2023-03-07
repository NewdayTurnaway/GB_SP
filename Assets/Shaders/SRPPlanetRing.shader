Shader "Custom/SRPPlanetRing"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _CircleSize("Size", Range(0,1)) = 1
        _PlanetSize("Planet Size", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent+200"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "ForceNoShadowCasting" = "True"
            "PreviewType" = "Plane"
        }
    
        LOD 200
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
    
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
    
            #include "UnityCG.cginc" 
            #include "UnityLightingCommon.cginc" 
    
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
                fixed4 diffuse : COLOR0;
            };
    
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _CircleSize;
            float _PlanetSize;
            float4 _Color;
    
            float Circle(float2 xy, float radius)
            {
                float2 dist = xy - float2(0.5, 0.5);
                return 1 - smoothstep(radius - (radius * 0.01), radius + (radius * 0.01), dot(dist, dist) * 4.0);
            }
    
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                o.diffuse = nl * _LightColor0;
                return o;
            }
    
            fixed4 frag(v2f i) : SV_Target
            {
                float outerCircle = Circle(i.uv, _CircleSize);
                float innerCircle = Circle(i.uv, _PlanetSize);
                float ring = outerCircle - innerCircle;
                fixed4 col = tex2D(_MainTex, i.uv) * ring * _Color;
                //col.a = ring * _Color.a;
                //col *= i.diffuse;
                return col;
            }
            ENDCG
        }
    }
}