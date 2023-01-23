Shader "SimpleDrawCanvas/Drawer"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
        _BrashTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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

            sampler2D _MainTex;
            sampler2D _BrashTex;
            float4 _MainTex_ST;
            fixed _BrashSize;
            float2 _Pos;
            fixed4 _Color;
            fixed _Aspect;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 buv = i.uv - _Pos.xy;
                buv *= _BrashSize;
                buv.x *= _Aspect;
                buv += float2(0.5, 0.5);

                fixed4 col = tex2D(_MainTex, i.uv);

                if (buv.x < 0.0 || buv.x > 1.0)
                {
                    return col;
                }

                if (buv.y < 0.0 || buv.y > 1.0)
                {
                    return col;
                }

                fixed4 brash = tex2D(_BrashTex, buv);
                brash.rgb = 1.0 - brash.rgb;

                return lerp(col, (brash * _Color), brash.a);
            }
            ENDCG
        }
    }
}
