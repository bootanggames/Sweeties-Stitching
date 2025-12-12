Shader "Custom/URPBlur"
{
    Properties
    {
        _BlurSize("Blur Size", Float) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalRenderPipeline"
        }

        Pass
        {
            Name "Blur"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            // Works on all URP versions
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_CameraColorTexture);
            SAMPLER(sampler_CameraColorTexture);

            float _BlurSize;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            // Fullscreen quad vertex shader
            Varyings Vert(Attributes input)
            {
                Varyings output;

                // Transform to clip space
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);

                // Pass through UV
                output.uv = input.uv;

                return output;
            }

            float4 Frag(Varyings i) : SV_Target
            {
                float2 uv = i.uv;

                float2 offset = _BlurSize * _ScreenParams.zw;

                float4 col =
                    SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, uv) +
                    SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, uv + float2(offset.x, 0)) +
                    SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, uv - float2(offset.x, 0)) +
                    SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, uv + float2(0, offset.y)) +
                    SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, uv - float2(0, offset.y));

                return col / 5;
            }

            ENDHLSL
        }
    }
}
