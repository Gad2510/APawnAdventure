Shader "Custom/EdgeOutlineSurfaceShader"
{
    Properties
    {
        _Color("Main Color", Color) = (1,1,1,1)
        _OutlineColor("Outline Color (with Opacity)", Color) = (0,0,0,1)
        _OutlineThickness("Outline Thickness", Float) = 0.02
    }

        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        // Main Surface Shader
        CGPROGRAM
        #pragma surface surf Lambert alpha:fade

        fixed4 _Color;

        struct Input
        {
            float3 worldNormal;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            o.Albedo = _Color.rgb;
            o.Alpha = _Color.a; // Use alpha from the main color
        }
        ENDCG

            // Outline Pass
            Pass
            {
                Name "Outline"
                Cull Off // Render only backfaces for outline

                ZWrite Off
                ColorMask RGB
                Blend SrcAlpha OneMinusSrcAlpha // Enable transparency blending

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                float _OutlineThickness;
                fixed4 _OutlineColor;

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv:TEXCOORD0;
                    float3 normal : NORMAL;
                };

                struct v2f
                {
                    float4 pos : POSITION;
                    float2 uv:TEXCOORD0;
                };

                // Vertex function that pushes vertices outward
                v2f vert(appdata v)
                {
                    v2f o;
                    float3 norm = normalize(v.normal); // Normalize normal
                    o.pos = UnityObjectToClipPos(v.vertex + norm * _OutlineThickness); // Offset vertices along normal
                    o.uv = v.uv;
                    return o;
                }

                // Fragment shader that sets outline color with transparency
                fixed4 frag(v2f i) : COLOR
                {
                    float2 center = (0.5,0.5);
                    float4 col= _OutlineColor;
                    col.a = sqrt(abs(i.uv-center))/5;
                    return col; // Includes alpha transparency
                }

                ENDCG
            }
    }
        FallBack "Diffuse"
}
