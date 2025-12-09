Shader "UI/UIBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Range(0, 100)) = 2
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

        Pass
        {
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _BlurSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float4 col = float4(0,0,0,0);

                // 9-sample blur
                col += tex2D(_MainTex, uv + float2(-_BlurSize, -_BlurSize) * _MainTex_TexelSize.xy);
                col += tex2D(_MainTex, uv + float2(0, -_BlurSize) * _MainTex_TexelSize.xy);
                col += tex2D(_MainTex, uv + float2(_BlurSize, -_BlurSize) * _MainTex_TexelSize.xy);

                col += tex2D(_MainTex, uv + float2(-_BlurSize, 0) * _MainTex_TexelSize.xy);
                col += tex2D(_MainTex, uv);
                col += tex2D(_MainTex, uv + float2(_BlurSize, 0) * _MainTex_TexelSize.xy);

                col += tex2D(_MainTex, uv + float2(-_BlurSize, _BlurSize) * _MainTex_TexelSize.xy);
                col += tex2D(_MainTex, uv + float2(0, _BlurSize) * _MainTex_TexelSize.xy);
                col += tex2D(_MainTex, uv + float2(_BlurSize, _BlurSize) * _MainTex_TexelSize.xy);

                return col / 9;
            }
            ENDCG
        }
    }
}
