﻿Shader "Unlit/Emission from vertex color"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Cutout("Cutout", float) = 0.5
        [HDR]
        _Emission("Emission Color", Color) = (0,0,0,1)
        [HDR]
        _AdditiveColor("Additive Color", Color) = (0,0,0,1)
        [HDR]
        _TintColor("Tint Color", Color) = (1,1,1,1)
        [Toggle]
        _IsALight("Is A Light Or Wall", int) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent"}
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest always
        Pass
        {
            Tags { "LightMode"="Universal2D" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 worldPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _LightTexture;
            float _Cutout;
            float2 LightingOrigin;
            int2 ProbeCounts;
            int PixelsPerUnit;
            
            float4 _TintColor;
            float3 _AdditiveColor;


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xy;
                o.color = v.color;
                return o;
            }
            
            float2 getLightUv(float2 worldPos)
            {
                return (worldPos - LightingOrigin + 0.5) /(ProbeCounts);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb *= tex2D(_LightTexture, getLightUv(i.worldPos)) * i.color.rgb;
                col.rgb *= _TintColor;
                col.rgb += _AdditiveColor;
                col.a *= i.color.a * _TintColor.a;
                return col;
            }
            ENDCG
        }
        
        Pass
        {
            Tags { "LightMode"="Lighting2D" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Cutout;
            
            float3 _Emission;
            int _IsALight;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                clip(col.a - 0.01);
                return float4(_Emission * i.color.rgb * col.a * i.color.a, _IsALight);
            }
            ENDCG
        }
    }
}
