Shader "Custom/Lerp"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
        _SecondTex("SecondTex", 2D) = "white" {}
        _T("Transition", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        Pass
        {
            HLSLPROGRAM

            Texture2D _MainTex;
            Texture2D _SecondTex;
            float _T;
            
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GlobalSamplers.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.texcoord = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 col1 = SAMPLE_TEXTURE2D(_MainTex, sampler_PointClamp, IN.texcoord);
                half4 col2 = SAMPLE_TEXTURE2D(_SecondTex, sampler_PointClamp, IN.texcoord);
                half4 col = lerp(col1, col2, _T);
                return col;
            }
            ENDHLSL
        }
    }
}