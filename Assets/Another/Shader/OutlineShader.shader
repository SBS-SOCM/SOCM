Shader "Custom/OutlineShader"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1,0,0,1) // �ܰ��� ���� (������)
        _OutlineWidth ("Outline Width", Range(0.0, 0.3)) = 0.005 // �ܰ��� �β�
    }

    SubShader
    {
        // �ܰ����� �������ϴ� Pass
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "UniversalForward" }

            Cull Front  // �ո��� Cull�Ͽ� �ܰ����� ������
            ZWrite On
            ZTest LEqual
            ColorMask RGB // ���� ������
            Blend SrcAlpha OneMinusSrcAlpha  // ���� ������
            
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
                // ��� ���͸� ����Ͽ� �ܰ��� �β� ����
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
