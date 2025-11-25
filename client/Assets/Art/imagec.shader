Shader "Hidden/imagec"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Mask ("Mask", 2D) = "white" {}
        _Mask2("Mask2", 2D) = "white" {}

    }
    SubShader
    {
        // 支持半透明的设置
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Cull Off ZWrite Off ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha // 添加Alpha混合模式

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _Mask;
            sampler2D _Mask2;
            fixed4 _Color;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 mainTex = tex2D(_MainTex, i.uv);
                fixed4 maskTex = tex2D(_Mask, i.uv);
                fixed4 maskTex2 = tex2D(_Mask2, i.uv);
                fixed4 col = lerp(mainTex, _Color, maskTex2.a);
                return col  ;
            }
            ENDCG
        }
    }
}
