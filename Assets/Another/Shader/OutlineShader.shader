Shader "Custom/OutlineShader"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1,0,0,1) // 외곽선 색상
        _OutlineWidth ("Outline Width", Range(0.0, 0.03)) = 0.005 // 외곽선 두께
    }

    SubShader
    {
        Tags { "Queue" = "Overlay" }  // 외곽선을 오버레이처럼 마지막에 렌더링

        // 외곽선을 그리는 Pass
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "UniversalForward" }

            // 외곽선이 상자의 외곽에만 그려지도록 앞면을 제거
            Cull Front
            ZWrite On
            ZTest LEqual
            ColorMask RGB
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 color : COLOR;
            };

            float _OutlineWidth;
            float4 _OutlineColor;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                // 노멀 벡터를 사용하여 외곽선 두께만큼 오브젝트 확장
                float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);
                float3 positionWS = TransformObjectToWorld(IN.positionOS);

                // 외곽선 두께만큼 오브젝트 외곽을 확장
                positionWS += normalWS * _OutlineWidth;

                OUT.positionHCS = TransformWorldToHClip(positionWS);
                OUT.color = _OutlineColor;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return IN.color;
            }

            ENDHLSL
        }
    }

    Fallback Off
}
