// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Syng/Sprite Mask"
{
    Properties
    {
        [PerRendererData] _MainTex("Texture", 2D) = "white" {}
        [PerRendererData] _Color ("Tint", Color) = (1,1,1,1)
        [PerRendererData] _MaskTex ("Mask Texture", 2D) = "white" {}
        [PerRendererData] _OffsetX("Offset X", Range(0,1)) = 0
        [PerRendererData] _OffsetY("Offset Y", Range(0,1)) = 0
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
            "ForceNoShadowCasting"="True"
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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MaskTex, _MainTex;
            float4 _MaskTex_ST, _FlowVector;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float _OffsetX, _OffsetY;
            float4 _Color;

            float4 frag (v2f i) : SV_Target
            {
                float2 maskUV = i.uv * _MaskTex_ST.xy + _MaskTex_ST.zw + float2(_OffsetX, _OffsetY);

                float4 negMask = 1 - tex2D(_MaskTex, maskUV);
                float4 main = tex2D(_MainTex, i.uv);

                float4 color = negMask * main * _Color;
                color.a = negMask * main.a;

                return color;
            }

            ENDCG
        }
    }
}
