// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Syng/Distortion"
{
    Properties
    {
        _MainTex("Texture", 2D) = "black" {}
        _DistTex("Distortion Texture", 2D) = "grey" {}
        _DistMask("Distortion Mask", 2D) = "white" {}
        _DistStrength("Distortion Strength", float) = 0.01
    }

        SubShader
        {
            Tags {
                "Queue" = "Transparent"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
            }
            ZWrite Off

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

            sampler2D _MainTex;
            sampler2D _DistTex;
            sampler2D _DistMask;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float _DistStrength;

            fixed4 frag(v2f i) : SV_Target
            {
                float2 distScroll = float2(_Time.x, _Time.x);
                fixed2 dist = (tex2D(_DistTex, i.uv + distScroll).rg - 0.5) * 2;
                fixed distMask = tex2D(_DistMask, i.uv)[0];

                fixed4 col = tex2D(_MainTex, i.uv + dist * distMask * _DistStrength);
//                fixed bg = col.a;

                return col;
            }
            ENDCG
        }
    }
}
