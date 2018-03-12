//ddUVOffetRegion shader: Daniel DeEntremont (dandeentremont@gmail.com)
//Specify a rectangular region in a bitmap, which can be U/V offsetted
//independent of the rest of the bitmap! Use the "Display Region" slider to
//show a green rectangle where the effect will take place.
//Warning: Things get weird when the rectangle's min values are larger than the
//max values (You'll know when because the rectangle will become purple).

Shader "Custom/ddUVOffsetRegion" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}

        _DisplayRegion ("DisplayRegion", Range (0.0, 0.5) ) = 1
        _RectMinX ("Rectangle Min X (Percent)", Float) = 25
        _RectMaxX ("Rectangle Max X (Percent)", Float) = 75
        _RectMinY ("Rectangle Min y (Percent)", Float) = 25
        _RectMaxY ("Rectangle Max Y (Percent)", Float) = 75

        _OffsetU ("Offset U", Float) = 0
        _OffsetV ("Offset V", Float) = 0

    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
        Cull Off

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        fixed _DisplayRegion;
        fixed _RectMinX, _RectMaxX;
        fixed _RectMinY, _RectMaxY;
        fixed _OffsetU, _OffsetV;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            //Below divides all rectangle parameters by 100, so they are not
            //super-sensitive in the unity editor
            _RectMinX/=100;
            _RectMaxX/=100;
            _RectMinY/=100;
            _RectMaxY/=100;

            //preRect x and y each represent 1 dimensional min and max ranges
            //for rectangle. When they are multiplied together, they form a white
            //rectangle mask (where they intersect).
            float2 preRect;
            preRect.x = (IN.uv_MainTex.x > _RectMinX) - (IN.uv_MainTex.x > _RectMaxX);
            preRect.y = (IN.uv_MainTex.y > _RectMinY) - (IN.uv_MainTex.y > _RectMaxY);
            half rectMask = preRect.x * preRect.y;

            //uv_OffsetCoord.x and y copy the uv coordinates of the main texture
            //and are offsetted. Then, the old uv coordinates are blended with
            //the new uv coordinates, using the rectangle as a mask.
            float2 uv_OffsetCoord = IN.uv_MainTex;

            //add minimum rectangle limits so the region's lowest UV value is 0
            uv_OffsetCoord.x -= _RectMinX;
            uv_OffsetCoord.y -= _RectMinY;

            //multiply values so the highest UV value of the region is 1. Now the
            //region is normalized to a 0-1 range.
            uv_OffsetCoord.x *= (1 / ( _RectMaxX - _RectMinX ) );
            uv_OffsetCoord.y *= (1 / ( _RectMaxY - _RectMinY ) );

            //Offset the newly normalized coordinates.
            uv_OffsetCoord.x += _OffsetU;
            uv_OffsetCoord.y += _OffsetV;

            //So now, the problem is, offsetting will cause the UV values to go
            //lower than 0 or higher than 1. Fortunately, we can use frac() to
            //continuously repeat the texture (between 0 and 1) forever!
            uv_OffsetCoord.x = frac(uv_OffsetCoord.x);
            uv_OffsetCoord.y = frac(uv_OffsetCoord.y);

            //Below runs the normalization process in reverse
            uv_OffsetCoord.x *= ( _RectMaxX - _RectMinX );
            uv_OffsetCoord.y *= ( _RectMaxY - _RectMinY );

            uv_OffsetCoord.x += _RectMinX;
            uv_OffsetCoord.y += _RectMinY;

            //Blend old uv coordinates with new offsetted uv coordinates, using
            //the rectangle as a mask
            IN.uv_MainTex = (IN.uv_MainTex * (1-rectMask) ) + (uv_OffsetCoord * rectMask);

            //Apply image map to blended UV coordinates
            half4 c = tex2D (_MainTex, IN.uv_MainTex);

            //displays additive green rectangle for setup
            c.g += (rectMask * _DisplayRegion);

            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
