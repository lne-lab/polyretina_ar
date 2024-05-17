Shader "LNE/Outline"
{
    Properties
    {
        _backgroundColour("Background Colour", Color) = (0, 0, 0, 1)
        _borderColour("Border Colour", Color) = (1, 1, 1, 1)

        _leftBorderWidth("Left Border Width", Float) = 0.01
        _rightBorderWidth("Right Border Width", Float) = 0.01
        _topBorderWidth("Top Border Width", Float) = 0.01
        _bottomBorderWidth("Bottom Border Width", Float) = 0.01
    }

        SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _backgroundColour;
            float4 _borderColour;

            float _leftBorderWidth;
            float _rightBorderWidth;
            float _topBorderWidth;
            float _bottomBorderWidth;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                if (uv.x < 0+ _leftBorderWidth ||
                    uv.x > 1- _rightBorderWidth ||
                    uv.y < 0+ _bottomBorderWidth ||
                    uv.y > 1- _topBorderWidth)
                {
                    return _borderColour;
                }
                else
                {
                    return _backgroundColour * i.color;
                }
            }
            ENDCG
        }
    }
}
