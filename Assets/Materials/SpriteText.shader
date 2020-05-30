Shader "Unlit/SpriteCutoutTransparent"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		[PerRendererData] _AlphaTex("Sprite Alpha Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent"}
        Cull Off
		Lighting Off
		ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Pass
        {
            Tags { "LightMode"="Universal2D" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 worldPos : TEXCOORD1;
                float4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
			sampler2D _AlphaTex;
			fixed4 _Color;
			
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos =  v.vertex;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				return tex2D(_MainTex, i.uv) + _TextureSampleAdd * i.color;
			}
            ENDCG
        }
    }
}
