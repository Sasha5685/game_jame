Shader "Hidden/UModelerX_TexturePainter_LayerComposite"
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
            Blend One Zero, One Zero
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
            sampler2D _MaskTex;
            float4 _Color;
            float4 _TilingOffset;
            float _MaskTexType;

            sampler2D _BlendMainTex2;
            float _BlendMode;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float3 Darken(float3 base, float3 blend)
            {
                return min(blend, base);
            }

            float3 Multiply(float3 base, float3 blend)
            {
                return blend * base;
            }            

            float3 ColorBurn(float3 base, float3 blend)
            {
                float3 result = 1 - (1 - base) / (blend + 1e-6);
                result = saturate(result); // 0과 1 사이로 클램핑
                result = lerp(float3(0.0, 0.0, 0.0), result, step(0.0, blend));
                return result;
            }

            float3 LinearBurn(float3 base, float3 blend)
            {
                return saturate(base + blend - 1);
            }

            float3 Lighten(float3 base, float3 blend)
            {
                return max(base, blend);
            }
            
            float3 Screen(float3 base, float3 blend)
            {
                return 1 - (1 - base) * (1 - blend);
            }
            
            float3 ColorDodge(float3 base, float3 blend)
            {
                float3 result = base / (1 - blend + 1e-6);
                return result;
                result = saturate(result); // 0과 1 사이로 클램핑
                result = lerp(result, float3(1.0, 1.0, 1.0), step(1.0 - 1e-6, blend));
                return result;
            }
            
            float3 LinearDodge(float3 base, float3 blend)
            {
                return min(base + blend, 1);
            }
            
            float3 Overlay(float3 base, float3 blend)
            {
                float3 result;
                result.r = base.r <= 0.5 ? 2 * base.r * blend.r : 1 - 2 * (1 - base.r) * (1 - blend.r);
                result.g = base.g <= 0.5 ? 2 * base.g * blend.g : 1 - 2 * (1 - base.g) * (1 - blend.g);
                result.b = base.b <= 0.5 ? 2 * base.b * blend.b : 1 - 2 * (1 - base.b) * (1 - blend.b);
                return result;
            }
            
            float3 SoftLight(float3 base, float3 blend)
            {
                float3 result;
                for(int i = 0; i < 3; i++)
                {
                    if(blend[i] <= 0.5)
                        result[i] = base[i] - (1 - 2 * blend[i]) * base[i] * (1 - base[i]);
                    else
                        result[i] = base[i] + (2 * blend[i] - 1) * (sqrt(base[i]) - base[i]);
                }
                return result;
            }
            
            float3 HardLight(float3 base, float3 blend)
            {
                float3 result;
                for(int i = 0; i < 3; i++)
                {
                    if(blend[i] <= 0.5)
                        result[i] = 2 * base[i] * blend[i];
                    else
                        result[i] = 1 - 2 * (1 - base[i]) * (1 - blend[i]);
                }
                return result;
            }
            
            float3 VividLight(float3 base, float3 blend)
            {
                const float epsilon = 1e-6;
                float3 result;
                for(int i = 0; i < 3; i++)
                {
                    if(blend[i] <= 0.5)
                        result[i] = 1 - (1 - base[i]) / (2 * blend[i] + epsilon);
                    else
                        result[i] = base[i] / (2 * (1 - blend[i]) + epsilon);
                }
                return result;
            }

            float3 LinearLight(float3 base, float3 blend)
            {
                return saturate(base + 2 * blend - 1);
            }

            float3 PinLight(float3 base, float3 blend)
            {
                float3 result;
                for(int i = 0; i < 3; i++)
                {
                    if(blend[i] <= 0.5)
                        result[i] = min(base[i], 2 * blend[i]);
                    else
                        result[i] = max(base[i], 2 * blend[i] - 1);
                }
                return result;
            }
            
            float3 Difference(float3 base, float3 blend)
            {
                return abs(base - blend);
            }
            
            float3 Exclusion(float3 base, float3 blend)
            {
                return base + blend - 2 * base * blend;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv * _TilingOffset.xy + _TilingOffset.zw;
                float mask = _MaskTexType == 1 ? tex2D(_MaskTex, i.uv).r : 1;

                float4 tex = tex2Dlod(_MainTex, float4(uv, 0, 0));
                float4 alphatex = tex.a;

                float alpha = mask * _Color.a;

                // 상단레이어, premultiplied alpha
                float4 blend = tex * _Color * mask;
                float blend_alpha = blend.a;

                // 하단레이어 premultiplied alpha
                float4 base = tex2Dlod(_BlendMainTex2, float4(i.uv, 0, 0));

                if (_BlendMode != 0 && blend_alpha > 0)
                {
                    float base_alpha = base.a;
                    if (base_alpha > 0)
                    {
                        float3 blend_rgb = blend.rgb / blend_alpha;
                        float3 base_rgb = base.rgb / base_alpha;
                        
                        float3 dest_rgb = 0;
                        float dest_alpha = blend_alpha;

                        if (_BlendMode == 10)
                        {
                            dest_rgb = Darken(base_rgb, blend_rgb);
                        }
                        else if (_BlendMode == 11)
                        {
                            dest_rgb = Multiply(base_rgb, blend_rgb);
                        }
                        else if (_BlendMode == 12)
                        {
                            dest_rgb = ColorBurn(base_rgb, blend_rgb);
                        }
                        else if (_BlendMode == 13)
                        {
                            dest_rgb = LinearBurn(base_rgb, blend_rgb);
                        }
                        else if (_BlendMode == 20)
                        {
                            dest_rgb = Lighten(base_rgb, blend_rgb);
                        }
                        else if (_BlendMode == 21)
                        {
                            dest_rgb = Screen(base_rgb, blend_rgb);
                        }
                        else if (_BlendMode == 22)
                        {
                            dest_rgb = ColorDodge(base_rgb, blend_rgb);
                        }
                        else if (_BlendMode == 23)
                        {
                            dest_rgb = LinearDodge(base_rgb, blend_rgb);
                        }
                        else if (_BlendMode == 30)
                        {
                            dest_rgb = Overlay(base_rgb, blend_rgb);
                        }
                        else if (_BlendMode == 31)
                        {
                            dest_rgb = SoftLight(base_rgb, blend_rgb);
                        }
                        else if (_BlendMode == 32)
                        {
                            dest_rgb = HardLight(base_rgb, blend_rgb);
                        }
                        else if (_BlendMode == 33)
                        {
                            dest_rgb = VividLight(base_rgb, blend_rgb);
                        }
                        else if (_BlendMode == 34)
                        {
                            dest_rgb = LinearLight(base_rgb, blend_rgb);
                        }
                        else if (_BlendMode == 35)
                        {
                            dest_rgb = PinLight(base_rgb, blend_rgb);
                        }
                        else if (_BlendMode == 40)
                        {
                            dest_rgb = Difference(base_rgb, blend_rgb);
                        }
                        else if (_BlendMode == 41)
                        {
                            dest_rgb = Exclusion(base_rgb, blend_rgb);
                        }

                        return float4(saturate(dest_rgb), 1) * dest_alpha + base * (1 - dest_alpha);
                    }

                    return base;
                }
                else // normal blend
                {
                    return blend + base * (1 - blend_alpha);
                }
            }
            ENDCG
        }
    }
}
