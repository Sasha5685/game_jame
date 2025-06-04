Shader "Hidden/UModelerX_TexturePainter_TextureComposite"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Cull off
            ZTest always
            Blend One Zero
            Colormask RGBA

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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            sampler2D _SrcTex;
            float4 _Mask;
            float _ChannelType;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 c1 = tex2D(_MainTex, i.uv);
                float4 c2 = tex2D(_SrcTex, i.uv);
                if (_ChannelType == 1)
                    c1 = c1.xxxx;
                return lerp(c1, c2, 1-_Mask);
            }
            ENDCG
        }
    }
}
