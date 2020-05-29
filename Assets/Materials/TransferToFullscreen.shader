Shader "Unlit/TransferToFullscreen"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            sampler2D _MainTex;
            float2 _Offset;
            float hysteresis;
            
            sampler2D _PreviousFullScreenTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 probeUv = i.uv + _Offset;
                float2 destUv = i.uv;
                float4 probeColor = tex2D(_MainTex, probeUv);
                float4 oldColor = tex2D(_PreviousFullScreenTex, destUv);
                return float4(lerp(oldColor.rgb, probeColor.rgb, hysteresis) , 1);
            }
            ENDCG
        }
    }
}
