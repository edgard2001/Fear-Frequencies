Shader "VertexFragment/SonarCg"
{
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex VertMain
            #pragma fragment FragMain

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;

            sampler2D _WaveData;
            uint _Width;
            uint _Height;
            float4x4 _InverseView;

            struct VertData
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
            };

            struct FragData
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            FragData VertMain(VertData input)
            {
                FragData output;

                output.vertex = float4(input.vertex.xy, 0.0, 1.0);
                output.texcoord = (input.vertex.xy + 1.0) * 0.5;

                // For Direct3D Build
                output.texcoord.y = 1.0 - output.texcoord.y;

                // For Open/WebGL build
                //output.texcoord.y = output.texcoord.y;

                return output;
            }

            float3 CalcColor(float4 wpos)
            {
                float3 outColor;
                for (uint i = 0; i < _Height; i++)
                {
                    float x = tex2D(_WaveData, float2(1.0 * 1.0 / _Width / 2.0, (float)i / _Height + 1.0 / _Height / 2.0)).r;
                    float y = tex2D(_WaveData, float2(3.0 * 1.0 / _Width / 2.0, (float)i / _Height + 1.0 / _Height / 2.0)).r;
                    float z = tex2D(_WaveData, float2(5.0 * 1.0 / _Width / 2.0, (float)i / _Height + 1.0 / _Height / 2.0)).r;
                    
                    float3 origin = float3(x, y, z);

                    float range = tex2D(_WaveData, float2(7.0 * 1.0 / _Width / 2.0, (float)i / _Height + 1.0 / _Height / 2.0)).r;
                    float strength = tex2D(_WaveData, float2(9.0 * 1.0 / _Width / 2.0, (float)i / _Height + 1.0 / _Height / 2.0)).r;

                    float distance1 = length(origin - wpos.xyz) - range + 1000;

                    float3 tempColor = distance1 <= 0
                                    ? float3(0, 0, 0)
                                    : lerp(
                                        //float3(0,0,0),
                                        ///lerp(
                                        float3(0, 0, 0),
                                         float3(1, 1, 1),
                                        clamp(strength, 0, 1)
                                        //),
                                        // clamp(distance1 / waveWidth, 0, 1)
                                    );

                    float distance2 = range - length(origin - wpos.xyz);
                    tempColor = distance2 <= 0 ? float3(0, 0, 0) : tempColor;

                    tempColor *= 0.5;
                    outColor = float3(max(tempColor.x, outColor.x), max(tempColor.y, outColor.y),
                                      max(tempColor.z, outColor.z));
                }
                return outColor;
            }

            float4 FragMain(FragData input) : SV_Target
            {
                const float2 p11_22 = float2(unity_CameraProjection._11, unity_CameraProjection._22);
                const float2 p13_31 = float2(unity_CameraProjection._13, unity_CameraProjection._23);
                const float isOrtho = unity_OrthoParams.w;
                const float near = _ProjectionParams.y;
                const float far = _ProjectionParams.z;

                float d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, input.texcoord);
                #if defined(UNITY_REVERSED_Z)
                d = 1 - d;
                #endif
                float zOrtho = lerp(near, far, d);
                float zPers = near * far / lerp(far, near, d);
                float vz = lerp(zPers, zOrtho, isOrtho);

                float3 vpos = float3((input.texcoord * 2 - 1 - p13_31) / p11_22 * lerp(vz, 1, isOrtho), -vz);
                float4 wpos = mul(_InverseView, float4(vpos, 1));

                float3 color = CalcColor(wpos);

                return float4(color, 1);
            }
            ENDCG
        }
    }
}