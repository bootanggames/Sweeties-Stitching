Shader "UI/FrostedUIBlur"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _TempBlurredTex("Temp Blurred Texture", 2D) = "white" {}
        _BlurSize("Blur Radius", Range(0,50)) = 12
        _BlendAmount("Blur Blend Amount", Range(0,1)) = 1
        _Strength("Blur Strength", Range(1,5)) = 2.5
        _Brightness("Brightness", Range(0,2)) = 1.1
        _Contrast("Contrast", Range(0,2)) = 1.15
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        ZWrite Off

        //---------------------------------------------------------------------
        // PASS 1 — HORIZONTAL BLUR
        //---------------------------------------------------------------------
        Pass
        {
            Name "HBlur"
            Blend One OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragH
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _BlurSize;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

            static const float GAUSS[5] = {0.227027, 0.1945946, 0.1216216, 0.054054, 0.016216};

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 fragH(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float alpha = tex2D(_MainTex, uv).a;

                float4 col = tex2D(_MainTex, uv) * GAUSS[0];
                float stepX = _MainTex_TexelSize.x * _BlurSize;

                for (int k = 1; k < 5; k++)
                {
                    float off = stepX * k;
                    col += tex2D(_MainTex, uv + float2(off,0)) * GAUSS[k];
                    col += tex2D(_MainTex, uv - float2(off,0)) * GAUSS[k];
                }

                col.a = alpha;
                col.rgb *= col.a; // premultiply alpha
                return col;
            }
            ENDCG
        }

        //---------------------------------------------------------------------
        // PASS 2 — VERTICAL BLUR + FROSTED GLASS EFFECT
        //---------------------------------------------------------------------
        Pass
        {
            Name "VBlur"
            Blend One OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragV
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _TempBlurredTex;
            float4 _MainTex_TexelSize;
            float _BlurSize;
            float _BlendAmount;
            float _Strength;
            float _Brightness;
            float _Contrast;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

            static const float GAUSS[5] = {0.227027, 0.1945946, 0.1216216, 0.054054, 0.016216};

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 fragV(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float4 original = tex2D(_MainTex, uv);
                float alpha = original.a;

                // Vertical blur
                float4 blur = tex2D(_TempBlurredTex, uv) * GAUSS[0];
                float stepY = _MainTex_TexelSize.y * _BlurSize;

                for (int k = 1; k < 5; k++)
                {
                    float off = stepY * k;
                    blur += tex2D(_TempBlurredTex, uv + float2(0, off)) * GAUSS[k];
                    blur += tex2D(_TempBlurredTex, uv - float2(0, off)) * GAUSS[k];
                }

                // Stronger blur
                blur = original + (blur - original) * _Strength;

                // Frosted glass adjustments
                blur.rgb = (blur.rgb - 0.5) * _Contrast + 0.5; // contrast
                blur.rgb *= _Brightness;                        // brightness

                // Smooth blend with original
                float blendAmt = saturate(_BlendAmount * 1.5);
                float4 finalColor = lerp(original, blur, blendAmt);

                // Preserve alpha and premultiply
                finalColor.a = alpha;
                finalColor.rgb *= finalColor.a;

                return finalColor;
            }
            ENDCG
        }
    }
}
