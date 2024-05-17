Shader "LNE/HorizontalLines"
{
    Properties
    {
        _backgroundColour("Background Colour", Color) = (0, 0, 0, 1)
        _borderColour("Border Colour", Color) = (1, 1, 1, 1)
        _lineColour("Line Colour", Color) = (0.5, 0.5, 0.5, 1)

        _leftBorderWidth("Left Border Width", Float) = 0.01
        _rightBorderWidth("Right Border Width", Float) = 0.01
        _topBorderWidth("Top Border Width", Float) = 0.01
        _bottomBorderWidth("Bottom Border Width", Float) = 0.01

        _lineSpacing("Line Spacing", Range(0, 1)) = 0.1
        _lineSpacingOffset("Line Spacing Offset", Range(0, 1)) = 0
        _lineWidth("Line Width", Range(0, 1)) = 0.5
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
            float4 _lineColour;

            float _leftBorderWidth;
            float _rightBorderWidth;
            float _topBorderWidth;
            float _bottomBorderWidth;

            float _lineSpacing;
            float _lineSpacingOffset;
            float _lineWidth;

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
                    float y = fmod(uv.y + _lineSpacingOffset, _lineSpacing) * (1 / _lineSpacing);

                    if (y < _lineWidth)
                    {
                        return _lineColour;
                    }
                    else
                    {
                        return _backgroundColour;
                    }
                }
            }
            ENDCG
        }
    }
}
