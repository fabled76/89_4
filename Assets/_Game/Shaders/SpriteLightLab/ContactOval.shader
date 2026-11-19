Shader "DeadAir/Studies/SpriteLightLab/ContactOval"
{
    Properties { _Opacity("Opacity", Range(0,.6)) = .24 }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent-10" "RenderType"="Transparent" }
        Pass
        {
            Tags { "LightMode"="Universal2D" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            float _Opacity;
            struct A { float3 positionOS:POSITION; float2 uv:TEXCOORD0; };
            struct V { float4 positionCS:SV_POSITION; float2 uv:TEXCOORD0; };
            V Vert(A i) { V o; o.positionCS=TransformObjectToHClip(i.positionOS);o.uv=i.uv;return o; }
            half4 Frag(V i):SV_Target { float2 q=i.uv*2-1; float a=saturate(1-dot(q,q));return half4(0,0,0,_Opacity*a*a); }
            ENDHLSL
        }
    }
}
