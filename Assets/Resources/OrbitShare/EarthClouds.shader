Shader "OrbitShare/EarthClouds"
{
    Properties
    {
        _MainTex ("Cloud Texture", 2D) = "white" {}
        _SunDirection ("Sun Direction", Vector) = (-0.35, 0.32, -0.88, 0)
        _CloudColor ("Cloud Color", Color) = (1, 1, 1, 1)
        _ShadowColor ("Night Side Tint", Color) = (0.18, 0.28, 0.36, 1)
        _Alpha ("Alpha", Range(0, 1)) = 0.45
        _AlphaCutoff ("Soft Alpha Cutoff", Range(0, 1)) = 0.18
        _SunSideStrength ("Sun Side Strength", Range(0, 1)) = 0.75
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _SunDirection;
            fixed4 _CloudColor;
            fixed4 _ShadowColor;
            float _Alpha;
            float _AlphaCutoff;
            float _SunSideStrength;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };

            v2f vert(appdata vertexInput)
            {
                v2f output;
                output.position = UnityObjectToClipPos(vertexInput.vertex);
                output.uv = TRANSFORM_TEX(vertexInput.uv, _MainTex);
                output.worldNormal = UnityObjectToWorldNormal(vertexInput.normal);
                return output;
            }

            float2 EquirectUvFromNormal(float3 normal)
            {
                const float pi = 3.14159265359;
                normal = normalize(normal);
                float longitude = atan2(normal.z, normal.x);
                float latitude = asin(clamp(normal.y, -1.0, 1.0));
                return float2(
                    frac(longitude / (pi * 2.0) + 0.5),
                    saturate(latitude / pi + 0.5));
            }

            fixed4 frag(v2f input) : SV_Target
            {
                float3 normal = normalize(input.worldNormal);
                float2 cloudUv = EquirectUvFromNormal(normal);
                float3 sunDirection = normalize(_SunDirection.xyz);
                float sunAmount = saturate(dot(normal, sunDirection) * 0.5 + 0.5);
                float lighting = 1.0;

                fixed4 cloud = tex2D(_MainTex, cloudUv);
                float cloudMask = dot(cloud.rgb, float3(0.299, 0.587, 0.114));
                float alphaFromRgb = smoothstep(_AlphaCutoff, 1.0, cloudMask);
                float alpha = alphaFromRgb * cloud.a * _Alpha * _CloudColor.a;
                float3 color = _CloudColor.rgb * cloud.rgb;
                return fixed4(color, alpha);
            }
            ENDCG
        }
    }
    FallBack Off
}
