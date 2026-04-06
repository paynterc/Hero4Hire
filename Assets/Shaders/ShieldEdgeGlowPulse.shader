Shader "Custom/URP/ShieldEdgeGlowPulse"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0,0.5,1,0.15)
        _GlowColor ("Glow Color", Color) = (0,1,1,1)

        _FresnelPower ("Fresnel Power", Range(0.1, 10)) = 3
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 1.5

        _PulseSpeed ("Pulse Speed", Range(0, 10)) = 2
        _PulseStrength ("Pulse Strength", Range(0, 2)) = 0.5

        _EdgeSoftness ("Edge Softness", Range(0.01, 1)) = 0.2
        _MaxAlpha ("Max Alpha", Range(0,1)) = 0.4
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
            float _PulseSpeed;
            float _PulseStrength;
            float _EdgeSoftness;
            float _MaxAlpha;

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
                // Fresnel
                float fresnel = 1.0 - saturate(dot(IN.normalWS, IN.viewDirWS));
                fresnel = pow(fresnel, _FresnelPower);

                // Smooth edge (fixes pixelation)
                fresnel = smoothstep(0.0, _EdgeSoftness, fresnel);

                // Pulse
                float pulse = sin(_Time.y * _PulseSpeed) * 0.5 + 0.5;
                pulse = lerp(1.0 - _PulseStrength, 1.0 + _PulseStrength, pulse);

                float glow = fresnel * _GlowIntensity * pulse;

                // Color
                float3 finalColor = _BaseColor.rgb + _GlowColor.rgb * glow;

                // Controlled transparency (don’t let it go opaque)
                float alpha = saturate(_BaseColor.a + fresnel * 0.5);
                alpha = min(alpha, _MaxAlpha);

                return float4(finalColor, alpha);
            }
            ENDHLSL
        }
    }
}
