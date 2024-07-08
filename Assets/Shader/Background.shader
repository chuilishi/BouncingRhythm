Shader "Custom/Background"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Aspect ("Aspect", Range(0,2)) = 1
        _Scale ("Scale", Range(0,2)) = 1
        _TimeOffset ("Time_Offset", Range(0,1)) = 1
        _Transparency ("Transparency",Range(0,1)) = 1
        _FirstColor ("FirstColor",Color) = (1,1,1,1)
        _SecondColor ("SecondColor",Color) = (0.5, 0.75, 1.0, 1.0)
    }
    SubShader
    {
        //Opaque
        Tags { "Queue"="Transparent" }
        
        ZWrite off
        Blend SrcAlpha OneMinusSrcAlpha
        
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Aspect;
            float _Scale;
            float _TimeOffset;
            float _Transparency;
            float4 _FirstColor;
            float4 _SecondColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float aspect = _Aspect;
                fixed2 uv = i.uv * _Scale;
                uv -= fixed2(0.5 * aspect, 0.5 * aspect);
                float rot = radians(45.0);
                float2x2 m = float2x2(cos(rot), -sin(rot), sin(rot), cos(rot));
                uv = mul(m, uv);
                uv += fixed2(0.5, 0.5 * aspect);
                uv.y += 0.5 * (1.0 - aspect);
                fixed2 pos = 10.0 * uv;
                fixed2 rep = frac(pos);
                float dist = 2.0 * min(min(rep.x, 1.0 - rep.x), min(rep.y, 1.0 - rep.y));
                float squareDist = length((floor(pos) + fixed2(0.5,0.5)) - fixed2(5.0,5.0));
                float edge = sin(_TimeOffset*4+0.34 - squareDist * 0.5) * 0.5 + 0.5;
                edge = (_TimeOffset*4+0.34 - squareDist * 0.5) * 0.5;
                edge = 2.0 * frac(edge * 0.5);
                float value = frac(dist * 2.0);
                value = lerp(value, 1.0 - value, step(1.0, edge));
                edge = pow(abs(1.0 - edge), 2.0);
                value = smoothstep(edge - 0.05, edge, 0.95 * value);
                value += squareDist * 0.1;
                fixed4 fragColor = lerp(_FirstColor, _SecondColor, value);
                fragColor.a = 0.25 * clamp(value, 0.0, 1.0);
                fragColor.a *= _Transparency * 4;
                
                return fragColor; 
            }
            ENDCG
        }
    }
}