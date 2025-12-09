Shader "UI/SmoothUIBlur_PreserveShade"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _BlurSize("Blur Size", Range(0, 30)) = 12
        _BlendAmount("Blur Blend Amount", Range(0,1)) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100

        Pass
        {
            Name "EnhancedBlur"
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _BlurSize;
            float _BlendAmount; // how much blur to apply on top

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // Original pixel
                float4 original = tex2D(_MainTex, uv);

                // If blur size is zero or blend amount is 0, return original
                if (_BlurSize <= 0.001 || _BlendAmount <= 0.001)
                    return original;

                float4 col = float4(0,0,0,0);

                // 16-directional offsets for smooth merge
                float2 offsets[16] = {
                    float2(1,0), float2(-1,0), float2(0,1), float2(0,-1),
                    float2(1,1), float2(-1,1), float2(1,-1), float2(-1,-1),
                    float2(2,0), float2(-2,0), float2(0,2), float2(0,-2),
                    float2(2,2), float2(-2,2), float2(2,-2), float2(-2,-2)
                };

                // Center sample
                col += tex2D(_MainTex, saturate(uv)) * 0.125;

                // Surrounding samples
                for(int k = 0; k < 16; k++)
                {
                    float2 sampleUV = saturate(uv + offsets[k] * (_BlurSize * _MainTex_TexelSize.xy * 0.5));
                    col += tex2D(_MainTex, sampleUV) * 0.046875;
                }

                // Normalize weight
                float totalWeight = 0.125 + 16 * 0.046875;
                col /= totalWeight;

                // Mix blurred result with original color to preserve shade
                float4 finalColor = lerp(original, col, _BlendAmount);

                return finalColor;
            }
            ENDCG
        }
    }
}
