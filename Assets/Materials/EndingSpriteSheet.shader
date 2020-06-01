Shader "Unlit/Ending"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Cutout("Cutout", float) = 0.5
        [HDR]
        _AdditiveColor("Additive Color", Color) = (0,0,0,1)
        [HDR]
        _TintColor("Tint Color", Color) = (1,1,1,1)
        _Width("_Width", int) = 32
        _Fps("_Fps", int) = 30
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
            
            float4 _MainTex_TexelSize;
            
            float4 _TintColor;
            float3 _AdditiveColor;
            int _Width;
            int _Fps;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xy;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                i.uv.x *= _Width * _MainTex_TexelSize.x;
                i.uv.x += _Width * _MainTex_TexelSize.x * floor(_Time.y * _Fps);
                fixed4 col = tex2D(_MainTex, i.uv) * i.color * _TintColor;
                //col.rgb *= _TintColor;
                //col.rgb += _AdditiveColor;
                return col;
            }
            ENDCG
        }
    }
}
