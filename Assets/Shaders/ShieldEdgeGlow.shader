Shader "Custom/URP/ShieldEdgeGlow"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0,0.5,1,0.2)
        _GlowColor ("Glow Color", Color) = (0,1,1,1)
        _FresnelPower ("Fresnel Power", Range(0.1, 10)) = 3
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 1.5
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 viewDirWS : TEXCOORD1;
            };

            float4 _BaseColor;
            float4 _GlowColor;
            float _FresnelPower;
            float _GlowIntensity;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(positionWS);

                OUT.normalWS = normalize(TransformObjectToWorldNormal(IN.normalOS));
                OUT.viewDirWS = normalize(_WorldSpaceCameraPos - positionWS);

                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float fresnel = 1.0 - saturate(dot(IN.normalWS, IN.viewDirWS));
                fresnel = pow(fresnel, _FresnelPower);

                float glow = fresnel * _GlowIntensity;

                float4 color = _BaseColor;
                color.rgb += _GlowColor.rgb * glow;

                // Boost alpha slightly at edges
                color.a += glow;

                return color;
            }
            ENDHLSL
        }
    }
}
