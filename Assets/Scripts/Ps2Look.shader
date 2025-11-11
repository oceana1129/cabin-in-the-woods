Shader "Custom/Ps2Look"
{
    Properties
    {
        _Downscale("Downscale (pixel size)", Float) = 2
        _ColorSteps("Color Steps (per channel)", Float) = 32
        _DitherStrength("Dither Strength", Float) = 0.75
        _Gamma("Gamma", Float) = 1.2
        _Vignette("Vignette Strength", Float) = 0.2
        _MipBias("Texture Mip Bias", Float) = 1.5
        _MainTex("SourceTex (ignore)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "RenderType"="Opaque" }
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            Name "Ps2Look"
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3.0
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            sampler2D _MainTex;

            float _Downscale;
            float _ColorSteps;
            float _DitherStrength;
            float _Gamma;
            float _Vignette;
            float _MipBias;

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings  { float4 positionHCS : SV_POSITION; float2 uv : TEXCOORD0; float2 uvScreen : TEXCOORD1; };

            // 4x4 Bayer matrix in [-0.5, 0.5]
            static const float4x4 BAYER = float4x4(
                 0,  8,  2, 10,
                12,  4, 14,  6,
                 3, 11,  1,  9,
                15,  7, 13,  5
            ) / 16.0 - 0.5;

            Varyings Vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                o.uvScreen = v.uv; // fullscreen blit UVs
                return o;
            }

            float3 SafePow01(float3 x, float e)
            {
                return pow(saturate(x), e);
            }

            float3 QuantizeDither(float3 c, float2 uv)
            {
                int2 p = (int2)floor(uv * _ScreenParams.xy) & 3;
                float threshold = BAYER[p.y][p.x] * _DitherStrength;

                float steps = max(2.0, _ColorSteps);

                // gamma to linear-ish, quantize with ordered dither, back to display space
                float3 lin = SafePow01(c, 1.0 / _Gamma);
                float3 q   = floor(lin * steps + threshold) / (steps - 1.0);
                return SafePow01(q, _Gamma);
            }

            float4 SampleWithBias(float2 uv, float bias)
            {
                #if defined(SHADER_API_D3D11) || defined(SHADER_API_GLCORE) || defined(UNITY_COMPILER_HLSL)
                    return tex2Dbias(_MainTex, float4(uv, 0, bias));
                #else
                    return tex2D(_MainTex, uv);
                #endif
            }

            float4 Frag(Varyings i) : SV_Target
            {
                // pixelation: snap UVs to downscaled grid
                float2 pixel = _ScreenParams.xy / max(1.0, _Downscale);
                float2 uvStep = 1.0 / pixel;
                float2 uvBlock = floor(i.uvScreen / uvStep) * uvStep + uvStep * 0.5;

                float4 col = SampleWithBias(uvBlock, _MipBias);

                // slight gamma crush
                col.rgb = SafePow01(col.rgb, 1.0 / _Gamma);

                // quantize + ordered dither
                col.rgb = QuantizeDither(col.rgb, i.uvScreen);

                // vignette
                if (_Vignette > 0.0)
                {
                    float2 d = (i.uvScreen - 0.5) * float2(_ScreenParams.x/_ScreenParams.y, 1.0);
                    float vig = 1.0 - _Vignette * smoothstep(0.4, 0.9, dot(d,d));
                    col.rgb *= vig;
                }

                return col;
            }
            ENDHLSL
        }
    }
    Fallback Off
}
