Shader "Hidden/UModelerX_TexturePainter_PaintLineBrush"
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
            Blend[_SrcBlend] OneMinusSrcAlpha,[_SrcAlphaBlend] OneMinusSrcAlpha
            Colormask RGBA

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float3 worldpos : TEXCOORD0;
                float3 worldnormal : TEXCOORD1;
                float2 uv : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            float3 _BrushPositionArray[64];
            float _BrushRadiusArray[64];
            float4 _BrushColorArray[64];
            int _BrushCount;

            float _BrushHardness;
            float4 _BrushColor;
            float _FrontOnly;
            float _BrushShape; // 0 sphere 1 square

            float _UVSpace;
            float2 _TextureSpace;

            float3 _CameraPosition;
            float3 _CameraDirection;
            float _Iso;
            float3 _CameraUp;

            v2f vert(appdata v)
            {
                v2f o;
                o.uv = v.uv;
                o.vertex = float4(v.uv * float2(2,-2) + float2(-1,1), 1, 1);
                o.worldnormal = mul((float3x3)unity_ObjectToWorld, v.normal);
                if (_UVSpace == 1)
                    o.worldpos = float3(v.uv * _TextureSpace, 0);
                else
                    o.worldpos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            float BrushIntensity(float distance, float hardness, float brushradius)
            {
                float w = distance / brushradius;
                float r1 = 0.3 + 0.65 * hardness;
                return 1-saturate((w - r1) / (1-r1));
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 worldpos = i.worldpos;
                float3 normal = i.worldnormal;
                float3 cameradir = lerp(_CameraPosition - worldpos, -_CameraDirection, _Iso);
                float front = dot(cameradir, normal) >= 0 ? 1 : 0;
                float3 right = normalize(cross(_CameraUp, cameradir));
                float3 up = normalize(cross(cameradir, right));

                if (_UVSpace == 1)
                {
                    right = float3(1, 0, 0);
                    up = float3(0, 1, 0);
                }

                float alpha = 0;
                float4 color = 0;

                for (int j = 0; j < _BrushCount; j+=2)
                {
                    float3 v0 = _BrushPositionArray[j].xyz;
                    float brushradius0 = _BrushRadiusArray[j];
                    float4 c0 = _BrushColorArray[j];
    
                    float3 v1 = _BrushPositionArray[j+1].xyz;
                    float brushradius1 = _BrushRadiusArray[j+1];
                    float4 c1 = _BrushColorArray[j+1];

                    float t = saturate(dot(worldpos.xyz - v0, v1 - v0) / dot(v1 - v0, v1 - v0));
                    float3 v = lerp(v0, v1, t) - worldpos.xyz;
                    float brushradius = lerp(brushradius0, brushradius1, t);
                    float a;

                    if (_BrushShape == 1) // 사각형
                    {
                        float3 forward = cross(up, right);
                        float w = max(abs(dot(right, v)), abs(dot(up, v)));
                        a = BrushIntensity(w, _BrushHardness, brushradius) * saturate(1 - (abs(dot(forward, v)) / brushradius - 0.7) / 0.3);
                    }
                    else
                    {
                        float w = length(v);
                        a = BrushIntensity(w, _BrushHardness, brushradius) * saturate(1 - (abs(dot(normal, v)) / brushradius - 0.7) / 0.3);
                    }

                    if (a > alpha)
                    {
                        alpha = a;
                        float4 c = lerp(c0, c1, t);
                        color = float4(c.rgb * c.a, c.a) * a;
                    }
                }
                return color * lerp(1, front, _FrontOnly);
            }
            ENDCG
        }
    }
}
