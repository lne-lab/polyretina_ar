Shader "LNE/Eye Calibration"
{
    Properties
    {
        _position ("Eye Position", Vector) = (0.5, 0.5, 0, 0)
        _dilation ("Eye Dilation", Float) = 0
        _baseSize ("Base Pupil Size", Float) = 50
        _background ("Background Brightness", Float) = 0.15

        _colour1 ("Colour 1", Color) = (1, 0, 0, 1)
        _colour2 ("Colour 2", Color) = (0, 1, 1, 1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

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

            float4 _colour1;
            float4 _colour2;

            float2 uv;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 drawPoint(float2 position, float dilation, float4 colour)
            {
                float2 toPos = uv - position;
                float dist = dot(toPos, toPos);
                float radius = (2 - dist) / 2;
                radius *= _baseSize;
                radius -= _baseSize - 1;

                return colour * radius;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                uv = i.uv;
                return drawPoint(_position, _dilation, _colour1) + drawPoint(float2(.5, .5), _dilation, _colour2);
            }
            ENDCG
        }
    }
}
