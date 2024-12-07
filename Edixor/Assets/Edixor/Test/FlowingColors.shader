Shader "Custom/FlowingColors"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {} // Добавили текстуру
        _Color1 ("Color 1", Color) = (1,0,0,1)
        _Color2 ("Color 2", Color) = (1,1,0,1)
        _Speed ("Speed", Float) = 1
        _CustomTime ("Time", Float) = 0 // Добавили параметр времени
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

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            half4 _Color1;
            half4 _Color2;
            float _Speed;
            float _CustomTime;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // Используем _CustomTime для анимации
                float time = _CustomTime * _Speed;
                float blend = (sin(time) + 1.0) / 2.0;

                // Линейная интерполяция между цветами
                return lerp(_Color1, _Color2, blend);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
