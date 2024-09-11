Shader "Custom/OutlineShader"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1,0,0,1) // 외곽선 색상 (빨간색)
        _OutlineWidth ("Outline Width", Range(0.0, 0.3)) = 0.005 // 외곽선 두께
    }

    SubShader
    {
        // 외곽선을 렌더링하는 Pass
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "UniversalForward" }

            Cull Front  // 앞면을 Cull하여 외곽선만 렌더링
            ZWrite On
            ZTest LEqual
            ColorMask RGB // 색상만 렌더링
            Blend SrcAlpha OneMinusSrcAlpha  // 알파 블렌딩
            
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
                // 노멀 벡터를 사용하여 외곽선 두께 설정
                float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);
                float3 positionWS = TransformObjectToWorld(IN.positionOS);
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
