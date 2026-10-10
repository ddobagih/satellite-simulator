Shader "OrbitShare/EarthDayNight"
{
    Properties
    {
        _MainTex ("Day Texture", 2D) = "white" {}
        _NightTex ("Night Lights", 2D) = "black" {}
        _NormalTex ("Normal Map", 2D) = "bump" {}
        _BumpMap ("Bump Map", 2D) = "bump" {}
        _SpecularTex ("Specular Map", 2D) = "black" {}
        _SunDirection ("Sun Direction", Vector) = (-0.35, 0.32, -0.88, 0)
        _AtmosphereColor ("Atmosphere Tint", Color) = (0.23, 0.78, 1, 1)
        _OceanSpecColor ("Ocean Specular", Color) = (0.55, 0.95, 1, 1)
        _SpecStrength ("Specular Strength", Range(0, 1)) = 0.28
        _NormalStrength ("Normal Strength", Range(0, 2)) = 0.65
        _DaySaturation ("Day Color Saturation", Range(0, 2)) = 1.18
        _NightIntensity ("Night Light Intensity", Range(0, 4)) = 1.35
        _TerminatorSoftness ("Terminator Softness", Range(0.01, 1)) = 0.42
        _AtmosphereStrength ("Atmosphere Strength", Range(0, 1)) = 0.16
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 150

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NightTex;
            sampler2D _NormalTex;
            sampler2D _BumpMap;
            sampler2D _SpecularTex;
            float4 _SunDirection;
            fixed4 _AtmosphereColor;
            fixed4 _OceanSpecColor;
            float _SpecStrength;
            float _NormalStrength;
            float _DaySaturation;
            float _NightIntensity;
            float _TerminatorSoftness;
            float _AtmosphereStrength;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 worldPosition : TEXCOORD2;
                float3 worldTangent : TEXCOORD3;
                float3 worldBinormal : TEXCOORD4;
            };

            v2f vert(appdata vertexInput)
            {
                v2f output;
                output.position = UnityObjectToClipPos(vertexInput.vertex);
                output.uv = TRANSFORM_TEX(vertexInput.uv, _MainTex);
                output.worldNormal = UnityObjectToWorldNormal(vertexInput.normal);
                output.worldPosition = mul(unity_ObjectToWorld, vertexInput.vertex).xyz;
                output.worldTangent = UnityObjectToWorldDir(vertexInput.tangent.xyz);
                output.worldBinormal = cross(output.worldNormal, output.worldTangent) * vertexInput.tangent.w;
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

            float3 ApplyNormalMap(v2f input, float2 earthUv)
            {
                float3 baseNormal = normalize(input.worldNormal);
                float tangentValid = step(0.001, dot(input.worldTangent, input.worldTangent));
                float3 tangent = normalize(lerp(float3(1.0, 0.0, 0.0), input.worldTangent, tangentValid));
                float3 binormal = normalize(lerp(float3(0.0, 1.0, 0.0), input.worldBinormal, tangentValid));

                float3 normalA = UnpackNormal(tex2D(_NormalTex, earthUv));
                float3 normalB = UnpackNormal(tex2D(_BumpMap, earthUv));
                float3 tangentNormal = normalize(float3(normalA.xy + normalB.xy, max(0.001, normalA.z * normalB.z)));
                tangentNormal.xy *= _NormalStrength;
                tangentNormal = normalize(tangentNormal);

                float3 mappedNormal = normalize(tangentNormal.x * tangent + tangentNormal.y * binormal + tangentNormal.z * baseNormal);
                return normalize(lerp(baseNormal, mappedNormal, tangentValid));
            }

            fixed4 frag(v2f input) : SV_Target
            {
                float3 geometryNormal = normalize(input.worldNormal);
                float2 earthUv = EquirectUvFromNormal(geometryNormal);
                float3 normal = ApplyNormalMap(input, earthUv);
                float3 viewDirection = normalize(_WorldSpaceCameraPos - input.worldPosition);
                float3 sunDirection = normalize(_SunDirection.xyz);
                float sunAmount = dot(normal, sunDirection);
                float geoSunAmount = dot(geometryNormal, sunDirection);

                float softness = max(0.01, _TerminatorSoftness);
                float dayMask = 1.0;
                float nightMask = 0.0;

                float3 dayColor = tex2D(_MainTex, earthUv).rgb;
                float dayLuma = dot(dayColor, float3(0.299, 0.587, 0.114));
                dayColor = saturate(lerp(float3(dayLuma, dayLuma, dayLuma), dayColor, _DaySaturation));
                float3 nightColor = tex2D(_NightTex, earthUv).rgb * _NightIntensity;
                float specMap = tex2D(_SpecularTex, earthUv).r;

                float oceanMask = saturate((dayColor.b - max(dayColor.r, dayColor.g) * 0.45) * 3.0);
                float specMask = saturate(max(specMap, oceanMask * (1.0 - specMap)));
                float3 halfVector = normalize(sunDirection + viewDirection);
                float specular = pow(saturate(dot(normal, halfVector)), 96.0) * specMask * dayMask;

                float diffuse = 1.0;
                float3 litDay = dayColor * diffuse;
                float3 color = litDay * dayMask + nightColor * nightMask;

                float rim = pow(1.0 - saturate(dot(geometryNormal, viewDirection)), 3.0);
                float terminator = 0.0;
                float atmosphereSide = 1.0;
                color += _AtmosphereColor.rgb * (rim * atmosphereSide + terminator) * _AtmosphereStrength;
                color += _OceanSpecColor.rgb * specular * _SpecStrength;

                return fixed4(color, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Unlit/Texture"
}
