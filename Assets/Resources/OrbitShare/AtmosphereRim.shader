Shader "OrbitShare/AtmosphereRim"
{
    Properties
    {
        _RimColor ("Rim Color", Color) = (0.3, 0.85, 1, 1)
        _SunDirection ("Sun Direction", Vector) = (-0.35, 0.32, -0.88, 0)
        _RimPower ("Rim Power", Range(0.5, 8)) = 2.8
        _Alpha ("Alpha", Range(0, 1)) = 0.52
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend SrcAlpha One
        ZWrite Off
        Cull Back
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _RimColor;
            float4 _SunDirection;
            float _RimPower;
            float _Alpha;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPosition : TEXCOORD1;
            };

            v2f vert(appdata vertexInput)
            {
                v2f output;
                output.position = UnityObjectToClipPos(vertexInput.vertex);
                output.worldNormal = UnityObjectToWorldNormal(vertexInput.normal);
                output.worldPosition = mul(unity_ObjectToWorld, vertexInput.vertex).xyz;
                return output;
            }

            fixed4 frag(v2f input) : SV_Target
            {
                float3 normal = normalize(input.worldNormal);
                float3 viewDirection = normalize(_WorldSpaceCameraPos - input.worldPosition);
                float3 sunDirection = normalize(_SunDirection.xyz);
                float rim = pow(1.0 - saturate(dot(normal, viewDirection)), _RimPower);
                float daySide = saturate(dot(normal, sunDirection) * 0.45 + 0.55);
                float alpha = rim * _Alpha * daySide;
                return fixed4(_RimColor.rgb, alpha);
            }
            ENDCG
        }
    }
    FallBack Off
}
