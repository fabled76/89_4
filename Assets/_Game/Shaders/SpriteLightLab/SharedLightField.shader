Shader "DeadAir/Studies/SpriteLightLab/SharedLightField"
{
    Properties
    {
        [PerRendererData] _MainTex("Base RGB / unchanged alpha", 2D) = "white" {}
        _Tint("Pigment tint", Color) = (1,1,1,1)
        _IsSprite("Sprite renderer", Float) = 0
        _Curvature("Authored sprite curvature (not measured normals)", Range(0,2)) = 0
        _ShapeUv("Occupied UV minU/maxU/minV/maxV", Vector) = (0,1,0,1)
        _UvTransform("UV scale XY / offset ZW", Vector) = (1,1,0,0)
        _ZWrite("Depth write", Float) = 1
        _ZTest("Depth test", Float) = 4
        [HideInInspector] _RendererColor("Renderer color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" "RenderType"="Opaque" }
        Pass
        {
            Tags { "LightMode"="Universal2D" }
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            Cull Off
            ZWrite [_ZWrite]
            ZTest [_ZTest]
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            CBUFFER_START(UnityPerMaterial)
                float4 _UvTransform, _Tint, _ShapeUv;
                float _IsSprite, _Curvature;
            CBUFFER_END
            // Per-renderer light field, no global shader/pipeline state.
            float _LabAmbient, _LabMinimum, _WarmDirectionality, _NeonDirectionality;
            float4 _WarmPositionRange, _WarmColorIntensity, _NeonPositionRange, _NeonColorIntensity;
            float4 _LabSpriteFlip;
            struct A { float3 positionOS:POSITION; float3 normal:NORMAL; float2 uv:TEXCOORD0; half4 color:COLOR; UNITY_VERTEX_INPUT_INSTANCE_ID };
            struct V { float4 positionCS:SV_POSITION; float3 world:TEXCOORD0; float2 uv:TEXCOORD1; float3 normal:TEXCOORD2; float3 right:TEXCOORD3; float3 up:TEXCOORD4; half4 tint:COLOR; };
            V Vert(A input)
            {
                UNITY_SETUP_INSTANCE_ID(input);
                float2 flip = float2(1,1);
                half4 tint = _Tint;
                if (_IsSprite > .5) { flip = _LabSpriteFlip.xy; input.positionOS.xy *= flip; }
                V o;
                o.world = TransformObjectToWorld(input.positionOS);
                o.positionCS = TransformWorldToHClip(o.world);
                o.uv = input.uv * _UvTransform.xy + _UvTransform.zw;
                o.normal = TransformObjectToWorldNormal(_IsSprite > .5 ? float3(0,0,-1) : input.normal);
                o.right = normalize(TransformObjectToWorldDir(float3(flip.x,0,0)));
                o.up = normalize(TransformObjectToWorldDir(float3(0,flip.y,0)));
                o.tint = tint;
                return o;
            }
            float3 Contribution(float3 p, float3 n, float4 source, float4 rgbIntensity, float directionality)
            {
                float3 delta = source.xyz - p;
                float distance = length(delta);
                float t = saturate(1 - distance / max(source.w,.01));
                float falloff = t*t*(3-2*t);
                float facing = lerp(1, saturate(.35 + .65 * dot(n, delta / max(distance,.001))), directionality);
                return rgbIntensity.rgb * rgbIntensity.w * falloff * facing;
            }
            half4 Frag(V input):SV_Target
            {
                half4 pigment = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * input.tint;
                float3 n = normalize(input.normal);
                if (_Curvature > 0)
                {
                    float u = saturate((input.uv.x-_ShapeUv.x)/max(.001,_ShapeUv.y-_ShapeUv.x))*2-1;
                    float v = saturate((input.uv.y-_ShapeUv.z)/max(.001,_ShapeUv.w-_ShapeUv.z))*2-1;
                    n = normalize(n + input.right*u*_Curvature + input.up*v*.15*_Curvature);
                }
                float3 illumination = max(_LabAmbient, _LabMinimum).xxx;
                illumination += Contribution(input.world,n,_WarmPositionRange,_WarmColorIntensity,_WarmDirectionality);
                illumination += Contribution(input.world,n,_NeonPositionRange,_NeonColorIntensity,_NeonDirectionality);
                return half4(pigment.rgb * illumination, pigment.a);
            }
            ENDHLSL
        }
    }
}
