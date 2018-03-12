// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Syng/Shadow Overlay"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        [PerRendererData] _ShadowColor ("Shadow Color", Color) = (1, 1, 1, 1)
        [PerRendererData] _ShadowTex ("Shadow Texture", 2D) = "white" {}
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 _ShadowColor;
            sampler2D _ShadowTex;
            sampler2D _MainTex, _DistTex;
            sampler2D _MainTex_ST, _DistTex_ST;
            float _DistStrength, _ShadowStrength;

            float4 frag (v2f i) : SV_Target
            {
                // Overlay shadow
                float4 base = tex2D(_MainTex, i.uv);
                base *= tex2D(_ShadowTex, i.uv);

                float4 shadow = (1-tex2D(_ShadowTex, i.uv));
                shadow *= tex2D(_MainTex, i.uv) * _ShadowColor;
                shadow.a = (1 - tex2D(_ShadowTex, i.uv)) * tex2D(_MainTex, i.uv).a;

                return base + shadow;
            }

            ENDCG
        }
    }
}
