// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Syng/Gradient Overlay"
{
    Properties
    {
        [PerRendererData] _MainTex("Texture", 2D) = "white" {}
        [PerRendererData] _TopColor ("Top Color", Color) = (1, 1, 1, 1)
        [PerRendererData] _BottomColor ("Bottom Color", Color) = (1, 1, 1, 1)
        [PerRendererData] _RampTex ("Ramp Texture", 2D) = "white" {}

    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            sampler2D _MainTex;
            fixed4 _TopColor, _BottomColor;
            sampler2D _RampTex;

            float4 frag (v2f i) : SV_Target
            {
                // return tex2D(_MainTex, i.uv) * lerp(_BottomColor, _TopColor, tex2D(_RampTex, i.uv).r);
                return lerp(_BottomColor, _TopColor, tex2D(_RampTex, i.uv).r);
            }

            ENDCG
        }
    }
}
