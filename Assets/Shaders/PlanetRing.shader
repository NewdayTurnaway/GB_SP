Shader "Custom/PlanetRing"
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
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "ForceNoShadowCasting" = "True"
            "PreviewType" = "Plane"
        }

        LOD 200
        Cull Off

        CGPROGRAM

        #pragma surface surf Standard alpha:blend vertex:vert
        #pragma target 3.0

        struct Input
        {
            float2 texcoord : TEXCOORD0;
        };

        sampler2D _MainTex;
        float _CircleSize;
        float _PlanetSize;

        fixed4 _Color;

        float Circle(float2 xy, float radius)
        {
            float2 dist = xy - float2(0.5, 0.5);
            return 1 - smoothstep(radius - (radius * 0.01), radius + (radius * 0.01), dot(dist, dist) * 4.0);
        }

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.texcoord = v.texcoord;
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float2 xy = IN.texcoord.xy;
            float outerCircle = Circle(xy, _CircleSize);
            float innerCircle = Circle(xy, _PlanetSize);
            float ring = outerCircle - innerCircle;
            o.Albedo = tex2D(_MainTex, IN.texcoord) * ring * _Color;
            o.Alpha = ring * _Color.a;
        }

        ENDCG
    }

    FallBack "Diffuse"
}