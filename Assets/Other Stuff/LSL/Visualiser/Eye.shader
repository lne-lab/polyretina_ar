Shader "LNE/Eye"
{
    Properties
    {
        _position ("Eye Position", Vector) = (0.5, 0.5, 0, 0)
        _dilation ("Eye Dilation", Float) = 0
        _baseSize ("Base Pupil Size", Float) = 50
        _background ("Background Brightness", Float) = 0.15
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" }

        Pass
        {
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

            float2 _position;
            float _dilation;
            float _baseSize;
            float _background;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                _baseSize *= (1 + _dilation / 3);

                float invBaseSize = 1 / _baseSize;

                _position += float2(0.5, 0.5);

                if (_dilation < 0)
                {
                    _position = float2(0.5, 0.5);
                }

                float dist = distance(i.uv, _position);
                float pupil = clamp((_baseSize - dist) * invBaseSize, 0, 1 - _background);
                bool active = _dilation > 0;

                return float4(
                    _background + pupil,
                    _background + pupil * active,
                    _background + pupil * active,
                1);
            }
            ENDCG
        }
    }
}
